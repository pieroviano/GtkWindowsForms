using System.Runtime.InteropServices;

namespace Gtk.Windows.Forms.Controls.ControlBase;

internal class FuncLoader
{

    public static bool IsWindows, IsOSX, IsLinux;

    private const int RTLD_GLOBAL = 0x0100;

    private const int RTLD_LAZY = 0x0001;

    static FuncLoader()
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
                IsWindows = true;
                break;
            case PlatformID.MacOSX:
                IsOSX = true;
                break;
            case PlatformID.Unix:
                try
                {
                    var buf = Marshal.AllocHGlobal(8192);
                    if (uname(buf) == 0 && Marshal.PtrToStringAnsi(buf) == "Darwin")
                        IsOSX = true;
                    if (uname(buf) == 0 && Marshal.PtrToStringAnsi(buf) == "Linux")
                        IsLinux = true;

                    Marshal.FreeHGlobal(buf);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug($"Unexpected exception in FuncLoader", ex);
                }
                break;
        }
    }

    public static IntPtr GetProcAddress(IntPtr library, string function)
    {
        var ret = IntPtr.Zero;

        if (IsWindows)
            ret = Windows.GetProcAddress(library, function);
        else if (IsOSX)
            ret = OSX.dlsym(library, function);
        else if (IsLinux)
            ret = Linux.dlsym(library, function);
        else
            ret = Unix.dlsym(library, function);

        return ret;
    }

    public static T? LoadFunction<T>(IntPtr procaddress)
    {
        if (procaddress == IntPtr.Zero)
            return default;

        return Marshal.GetDelegateForFunctionPointer<T>(procaddress);
    }

    public static IntPtr LoadLibrary(string libname)
    {
        if (IsWindows)
            return Windows.LoadLibraryW(libname);

        if (IsOSX)
            return OSX.dlopen(libname, RTLD_GLOBAL | RTLD_LAZY);

        if (IsLinux)
            return Linux.dlopen(libname, RTLD_GLOBAL | RTLD_LAZY);

        return Unix.dlopen(libname, RTLD_GLOBAL | RTLD_LAZY);
    }

    [DllImport("libc")]
    private static extern int uname(IntPtr buf);

    private class Linux
    {
        [DllImport("libdl.so.2")]

        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("libdl.so.2")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

    }

    private class OSX
    {
        [DllImport("/usr/lib/libSystem.dylib")]

        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("/usr/lib/libSystem.dylib")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

    }

    private class Unix
    {
        [DllImport("libc")]

        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("libc")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

    }

    private class Windows
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]

        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryW(string lpszLib);

    }
}