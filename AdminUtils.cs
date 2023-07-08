using System.Diagnostics;
using System.Security.Principal;

public static class AdminUtils
{
    public static bool IsAdmin()
    {
        Debug.Assert(OperatingSystem.IsWindows());
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    public static void Elevate()
    {
        Debug.Assert(OperatingSystem.IsWindows());
        var SelfProc = new ProcessStartInfo
        {
            UseShellExecute = true,
            WorkingDirectory = Environment.CurrentDirectory,
            FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName,
            Verb = "runas"
        };
        try
        {
            Process.Start(SelfProc);
        }
        catch
        {
            Console.WriteLine("未能获得管理员权限！");
            Console.WriteLine("Unable to elevate!");
            System.Environment.Exit(-1);
        }
    }
}