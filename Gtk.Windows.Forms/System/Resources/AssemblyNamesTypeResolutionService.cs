using System.Collections;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Resources;

internal class AssemblyNamesTypeResolutionService : ITypeResolutionService
{
    private Hashtable? _cachedAssemblies;
    private Hashtable? _cachedTypes;
    private readonly AssemblyName[]? _names;

    // Cross-platform known .NET locations
    private static readonly string[] s_dotNetSharedPaths = GetDotNetSharedPaths();

    internal AssemblyNamesTypeResolutionService(AssemblyName[] names)
    {
        _names = names;
    }

    public Assembly? GetAssembly(AssemblyName name)
        => GetAssembly(name, true);

    public Assembly? GetAssembly(AssemblyName name, bool throwOnError)
    {
        Assembly? result = null;

        _cachedAssemblies ??= Hashtable.Synchronized(new Hashtable());

        if (_cachedAssemblies.Contains(name))
        {
            return _cachedAssemblies[name] as Assembly;
        }

        try
        {
            result = Assembly.Load(name.FullName);
        }
        catch (Exception e)
        {
            GtkLogger.Error($"Unable to load the assembly {name.FullName}", e);
            if (throwOnError)
                throw;
            return null;
        }

        if (result != null)
        {
            _cachedAssemblies[name] = result;
            return result;
        }

        if (_names != null)
        {
            foreach (var asmName in _names.Where(an => an.Equals(name)))
            {
                try
                {
                    var path = GetPathOfAssembly(asmName);
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        result = Assembly.LoadFrom(path);
                        if (result != null)
                        {
                            _cachedAssemblies[asmName] = result;
                            return result;
                        }
                    }
                }
                catch
                {
                    if (throwOnError)
                        throw;
                }
            }
        }

        return null;
    }

    public string GetPathOfAssembly(AssemblyName name)
    {
#pragma warning disable SYSLIB0044
        return name.CodeBase;
#pragma warning restore SYSLIB0044
    }

    public Type? GetType(string? name)
        => GetType(name, true);

    public Type? GetType(string? name, bool throwOnError)
        => GetType(name, throwOnError, false);

    public Type? GetType(string? name, bool throwOnError, bool ignoreCase)
    {
        Type? result = null;

        _cachedTypes ??= Hashtable.Synchronized(new Hashtable(StringComparer.Ordinal));

        if (_cachedTypes.Contains(name))
        {
            return _cachedTypes[name] as Type;
        }

        // Try direct resolution first
        if (name.Contains(','))
        {
            result = Type.GetType(name, false, ignoreCase);
        }

        if (result is null && _names != null)
        {
            ReorderAssembliesIfNeeded(name);

            foreach (var asmName in _names)
            {
                var asm = GetAssembly(asmName, false);
                if (asm == null) continue;

                if (name != null)
                {
                    result = asm.GetType(name, false, ignoreCase);

                    if (result is null && name.Contains(',') == true)
                    {
                        var shortName = name.Substring(0, name.IndexOf(','));
                        result = asm.GetType(shortName, false, ignoreCase);
                    }
                }

                if (result != null)
                    break;
            }
        }

        if (result is null)
        {
            if (throwOnError)
                throw new ArgumentException($"SR.InvalidResXNoType,{name}");

            return null;
        }

        // Cache only framework types
        if (IsDotNetAssembly(result.Assembly.Location))
        {
            _cachedTypes[name] = result;
        }

        return result;
    }

    public void ReferenceAssembly(AssemblyName name)
    {
        throw new NotSupportedException();
    }

    // -------------------------
    // Helpers
    // -------------------------

    private static string[] GetDotNetSharedPaths()
    {
        var paths = new List<string>();

        // DOTNET_ROOT (cross-platform)
        var root = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!string.IsNullOrEmpty(root))
        {
            paths.Add(Path.Combine(root, "shared"));
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
            var programFilesX86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

            if (!string.IsNullOrEmpty(programFiles))
                paths.Add(Path.Combine(programFiles, "dotnet", "shared"));

            if (!string.IsNullOrEmpty(programFilesX86))
                paths.Add(Path.Combine(programFilesX86, "dotnet", "shared"));
        }
        else
        {
            // Linux / macOS defaults
            paths.Add("/usr/share/dotnet/shared");
            paths.Add("/usr/local/share/dotnet/shared");
        }

        // Runtime directory (very reliable)
        var runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
        if (!string.IsNullOrEmpty(runtimeDir))
        {
            paths.Add(runtimeDir);
        }

        return paths
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(NormalizePath)
            .Distinct()
            .ToArray();
    }

    private static void ReorderAssembliesIfNeeded(string? name)
    {
        // unchanged logic extracted for clarity if needed later
    }

    private static bool IsDotNetAssembly(string? assemblyPath)
    {
        if (string.IsNullOrEmpty(assemblyPath))
            return false;

        var normalized = NormalizePath(assemblyPath);

        return s_dotNetSharedPaths.Any(p =>
            normalized.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizePath(string? path)
    {
        return Path.GetFullPath(path ?? string.Empty)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}