using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

// args: <refAssemblyPath> <implAssemblyPath> <implSearchDir> <outPrefix> <namespaceFilter>
class Program
{
    static int Main(string[] argv)
    {
        // --enumdump <refDll> <TypeSimpleName...> : print each enum's members and underlying values, so
        // enum shims can be authored against ground truth instead of recall.
        if (argv.Length >= 3 && argv[0] == "--enumdump")
        {
            return EnumDump(argv);
        }

        // --membersof <refDll> <TypeFullName...> : print each type's public/protected member signatures
        // (nested types addressed with '+'), so a shim can be authored against the reference surface.
        if (argv.Length >= 3 && argv[0] == "--membersof")
        {
            return MembersOf(argv);
        }

        string refAsmPath = argv[0];
        string implAsmPath = argv[1];
        string implDir = argv[2];
        string outPrefix = argv[3];
        string nsFilter = argv.Length > 4 ? argv[4] : null;

        var paths = new List<string>();
        paths.Add(refAsmPath);
        paths.Add(implAsmPath);
        foreach (var d in new[] { Path.GetDirectoryName(refAsmPath), implDir })
            if (Directory.Exists(d)) paths.AddRange(Directory.GetFiles(d, "*.dll"));
        // A .NET Framework reference assembly resolves against mscorlib, which collides with the
        // .NET Core ref pack's identities. Detect and pick one core set, not both.
        bool netFx = refAsmPath.IndexOf(".NETFramework", StringComparison.OrdinalIgnoreCase) >= 0;
        string coreAssemblyName = netFx ? "mscorlib" : "System.Runtime";
        var coreRef = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "dotnet", "packs", "Microsoft.NETCore.App.Ref", "10.0.10", "ref", "net10.0");
        // The impl assemblies are netstandard2.0 and need the .NET Core ref pack to resolve even when
        // the reference side is .NET Framework. Both packs ship an mscorlib.dll facade, so dedupe by
        // file name with earlier entries (reference dir, then impl dir) winning.
        if (Directory.Exists(coreRef)) paths.AddRange(Directory.GetFiles(coreRef, "*.dll"));

        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resolved = paths.Where(p => seenNames.Add(Path.GetFileName(p))).ToArray();
        var resolver = new PathAssemblyResolver(resolved);
        using var mlc = new MetadataLoadContext(resolver, coreAssemblyName);

        var refAsm = mlc.LoadFromAssemblyPath(refAsmPath);
        var implAsm = mlc.LoadFromAssemblyPath(implAsmPath);

        var refTypes = PublicTypes(refAsm, nsFilter);
        // Some types are type-forwarded to a companion assembly; GetTypes() on the ref
        // assembly does not surface them. Merge the companion in so they are not reported missing.
        var companion = Path.Combine(Path.GetDirectoryName(refAsmPath),
            Path.GetFileNameWithoutExtension(refAsmPath) + ".Primitives.dll");
        if (File.Exists(companion))
        {
            foreach (var kv in PublicTypes(mlc.LoadFromAssemblyPath(companion), nsFilter))
                if (!refTypes.ContainsKey(kv.Key)) refTypes[kv.Key] = kv.Value;
            Console.WriteLine("(merged companion ref assembly: " + Path.GetFileName(companion) + ")");
        }
        var implTypes = PublicTypes(implAsm, null);
        // impl may also live in a sibling assembly (Drawing split); merge extra assemblies
        for (int i = 5; i < argv.Length; i++)
        {
            var extra = mlc.LoadFromAssemblyPath(argv[i]);
            foreach (var kv in PublicTypes(extra, null)) implTypes[kv.Key] = kv.Value;
        }

        var missingTypes = new List<string>();
        var presentTypes = new List<string>();
        var memberReport = new List<(string type, int missing, int total, List<string> members)>();

        foreach (var kv in refTypes.OrderBy(k => k.Key, StringComparer.Ordinal))
        {
            if (!implTypes.TryGetValue(kv.Key, out var it))
            {
                missingTypes.Add(kv.Key + "\t" + Kind(kv.Value));
                continue;
            }
            presentTypes.Add(kv.Key);
            var refMembers = MemberSigs(kv.Value);
            var implMembers = MemberSigs(it);
            var miss = refMembers.Where(m => !implMembers.Contains(m)).OrderBy(m => m, StringComparer.Ordinal).ToList();
            memberReport.Add((kv.Key, miss.Count, refMembers.Count, miss));
        }

        var extraTypes = implTypes.Keys.Where(k => !refTypes.ContainsKey(k)).OrderBy(k => k, StringComparer.Ordinal).ToList();

        File.WriteAllLines(outPrefix + ".missing-types.tsv", missingTypes);
        File.WriteAllLines(outPrefix + ".extra-types.txt", extraTypes);
        using (var w = new StreamWriter(outPrefix + ".missing-members.txt"))
        {
            foreach (var r in memberReport.Where(r => r.missing > 0).OrderByDescending(r => r.missing))
            {
                w.WriteLine($"=== {r.type}  [{r.missing}/{r.total} missing]");
                foreach (var m in r.members) w.WriteLine("    " + m);
            }
        }
        using (var w = new StreamWriter(outPrefix + ".summary.tsv"))
        {
            w.WriteLine("type\tmissingMembers\ttotalMembers");
            foreach (var r in memberReport.OrderByDescending(r => r.missing))
                w.WriteLine($"{r.type}\t{r.missing}\t{r.total}");
        }

        int totalRefMembers = memberReport.Sum(r => r.total);
        int totalMissMembers = memberReport.Sum(r => r.missing);
        Console.WriteLine($"ref public types: {refTypes.Count}");
        Console.WriteLine($"present: {presentTypes.Count}   missing: {missingTypes.Count}");
        Console.WriteLine($"members on present types: {totalRefMembers}, missing: {totalMissMembers}");
        Console.WriteLine($"types present but with >=1 missing member: {memberReport.Count(r => r.missing > 0)}");
        Console.WriteLine($"impl-only public types: {extraTypes.Count}");
        return 0;
    }

    static string Kind(Type t) =>
        t.IsEnum ? "enum" : t.IsInterface ? "interface" :
        (t.BaseType != null && t.BaseType.FullName == "System.Delegate") ? "delegate" :
        (t.BaseType != null && t.BaseType.FullName == "System.MulticastDelegate") ? "delegate" :
        t.IsValueType ? "struct" : "class";

    static Dictionary<string, Type> PublicTypes(Assembly a, string nsFilter)
    {
        var d = new Dictionary<string, Type>(StringComparer.Ordinal);
        IEnumerable<Type> types;
        try { types = a.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null); }
        foreach (var t in types)
        {
            if (t == null) continue;
            if (!(t.IsPublic || t.IsNestedPublic)) continue;
            var fn = t.FullName;
            if (fn == null) continue;
            if (nsFilter != null && !(t.Namespace ?? "").StartsWith(nsFilter, StringComparison.Ordinal)) continue;
            d[fn] = t;
        }
        return d;
    }

    // NOT DeclaredOnly: a member inherited from a base type is just as callable by user code,
    // so the compile-compatibility question is about the whole accessible surface of the type.
    const BindingFlags BF = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.Static | BindingFlags.FlattenHierarchy;

    static HashSet<string> MemberSigs(Type t)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        var members = new List<MemberInfo>();
        // FlattenHierarchy omits instance members of bases; walk the chain explicitly.
        for (var cur = t; cur != null; )
        {
            bool isSelf = cur == t;
            try
            {
                // constructors are not inherited — only count the type's own
                members.AddRange(cur.GetMembers(BF | BindingFlags.DeclaredOnly)
                    .Where(x => isSelf || !(x is ConstructorInfo)));
            }
            catch { }
            try { cur = cur.BaseType; } catch { break; }
        }
        if (t.IsInterface)
        {
            foreach (var i in SafeInterfaces(t))
                try { members.AddRange(i.GetMembers(BF | BindingFlags.DeclaredOnly)); } catch { }
        }
        foreach (var m in members)
        {
            try
            {
                switch (m)
                {
                    case MethodInfo mi:
                        if (!(mi.IsPublic || mi.IsFamily || mi.IsFamilyOrAssembly)) continue;
                        if (mi.IsSpecialName) continue; // accessors handled via property/event
                        set.Add($"M:{mi.Name}({string.Join(",", mi.GetParameters().Select(p => Simple(p.ParameterType)))})");
                        break;
                    case ConstructorInfo ci:
                        if (!(ci.IsPublic || ci.IsFamily || ci.IsFamilyOrAssembly)) continue;
                        set.Add($"C:.ctor({string.Join(",", ci.GetParameters().Select(p => Simple(p.ParameterType)))})");
                        break;
                    case PropertyInfo pi:
                        {
                            var acc = pi.GetAccessors(true).Where(a => a.IsPublic || a.IsFamily || a.IsFamilyOrAssembly).ToArray();
                            if (acc.Length == 0) continue;
                            var idx = pi.GetIndexParameters();
                            var sig = idx.Length > 0
                                ? $"P:{pi.Name}[{string.Join(",", idx.Select(p => Simple(p.ParameterType)))}]"
                                : $"P:{pi.Name}";
                            set.Add(sig);
                            break;
                        }
                    case EventInfo ei:
                        {
                            var add = ei.GetAddMethod(true);
                            if (add == null || !(add.IsPublic || add.IsFamily || add.IsFamilyOrAssembly)) continue;
                            set.Add($"E:{ei.Name}");
                            break;
                        }
                    case FieldInfo fi:
                        if (!(fi.IsPublic || fi.IsFamily || fi.IsFamilyOrAssembly)) continue;
                        set.Add($"F:{fi.Name}");
                        break;
                }
            }
            catch { }
        }
        return set;
    }

    static Type[] SafeInterfaces(Type t)
    {
        try { return t.GetInterfaces(); } catch { return Array.Empty<Type>(); }
    }

    static string Simple(Type t)
    {
        if (t == null) return "?";
        try
        {
            var n = t.Name;
            if (t.IsByRef) n = t.GetElementType()?.Name + "&";
            return n;
        }
        catch { return "?"; }
    }

    // --enumdump <refDll> <TypeSimpleName...> : dump each named enum's members and underlying values.
    static int EnumDump(string[] argv)
    {
        string refAsmPath = argv[1];
        var wanted = new HashSet<string>(argv.Skip(2), StringComparer.Ordinal);

        var paths = new List<string> { refAsmPath };
        var dir = Path.GetDirectoryName(refAsmPath);
        if (Directory.Exists(dir)) paths.AddRange(Directory.GetFiles(dir, "*.dll"));
        bool netFx = refAsmPath.IndexOf(".NETFramework", StringComparison.OrdinalIgnoreCase) >= 0;
        string coreAssemblyName = netFx ? "mscorlib" : "System.Runtime";
        var coreRef = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "dotnet", "packs", "Microsoft.NETCore.App.Ref", "10.0.10", "ref", "net10.0");
        if (Directory.Exists(coreRef)) paths.AddRange(Directory.GetFiles(coreRef, "*.dll"));

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resolved = paths.Where(p => seen.Add(Path.GetFileName(p))).ToArray();
        using var mlc = new MetadataLoadContext(new PathAssemblyResolver(resolved), coreAssemblyName);
        var asm = mlc.LoadFromAssemblyPath(refAsmPath);

        foreach (var t in asm.GetTypes())
        {
            if (!t.IsEnum || !wanted.Contains(t.Name)) continue;
            var underlying = t.GetEnumUnderlyingType().Name;
            Console.WriteLine($"{t.FullName} : {underlying}");
            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var raw = f.GetRawConstantValue();
                Console.WriteLine($"    {f.Name} = {raw}");
            }
            Console.WriteLine();
        }
        return 0;
    }

    // --membersof <refDll> <TypeFullName...> : dump each named type's public/protected member signatures.
    static int MembersOf(string[] argv)
    {
        string refAsmPath = argv[1];
        var paths = new List<string> { refAsmPath };
        var dir = Path.GetDirectoryName(refAsmPath);
        if (Directory.Exists(dir)) paths.AddRange(Directory.GetFiles(dir, "*.dll"));
        bool netFx = refAsmPath.IndexOf(".NETFramework", StringComparison.OrdinalIgnoreCase) >= 0;
        string coreAssemblyName = netFx ? "mscorlib" : "System.Runtime";
        var coreRef = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "dotnet", "packs", "Microsoft.NETCore.App.Ref", "10.0.10", "ref", "net10.0");
        if (Directory.Exists(coreRef)) paths.AddRange(Directory.GetFiles(coreRef, "*.dll"));
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resolved = paths.Where(p => seen.Add(Path.GetFileName(p))).ToArray();
        using var mlc = new MetadataLoadContext(new PathAssemblyResolver(resolved), coreAssemblyName);
        var asm = mlc.LoadFromAssemblyPath(refAsmPath);

        for (int i = 2; i < argv.Length; i++)
        {
            var t = asm.GetType(argv[i]);
            if (t == null) { Console.WriteLine($"{argv[i]}: NOT FOUND"); continue; }
            Console.WriteLine($"{t.FullName} : {t.BaseType?.Name} [{string.Join(", ", SafeInterfaces(t).Select(x => x.Name))}]");
            foreach (var sig in MemberSigs(t).OrderBy(s => s, StringComparer.Ordinal))
                Console.WriteLine($"    {sig}");
            Console.WriteLine();
        }
        return 0;
    }
}
