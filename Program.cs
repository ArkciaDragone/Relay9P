using System.Net;
using System.Net.Sockets;

public static class MainModule
{
    public const string DST_URL = "master.frozenbyte-online.com";
    public const int DST_PORT = 27300;
    public static string DST_IP = "192.81.220.187";
    public static void Main(string[] args)
    {
        // Parse arguments (optional)
        ArgParse(args);

        // Check hosts file status
        var patcher = new HostsPatcher();
        bool isPatched = patcher.IsPatched();
        if (!isPatched)
        {
            if (AdminUtils.IsAdmin())
            {
                patcher.Patch();
                Console.WriteLine("hosts文件修改成功！下次运行时将不再需要管理员权限。");
                Console.WriteLine("请注意，以后想进行9P线上游戏时均应先运行此程序。");
                Console.WriteLine("Successfully modified hosts file! Next time admin privilege won't be required.");
                Console.WriteLine("Please note that you will NOT be able to play online in 9P without this program until you restore your hosts file.\n");
            }
            else
            {
                Console.WriteLine("hosts文件还未就绪！");
                Console.WriteLine("修改hosts文件需要管理员权限，接下来请允许此程序“对你的设备进行更改”。");
                Console.WriteLine("如果有防火墙弹窗，请允许此程序访问互联网。");
                Console.WriteLine("按回车键继续…");
                Console.WriteLine("Hosts file is not patched!");
                Console.WriteLine("Admin privilege is required to patch hosts file.");
                Console.WriteLine("Please press ENTER and allow this program to modify your computer.");
                Console.WriteLine("If there is a firewall popup, please allow this app to access the Internet.");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                AdminUtils.Elevate();
                System.Environment.Exit(0);
            }
        }

        // Start the server
        RelayProxy rp;
        while (true) // for port-in-use retry
        {
            try
            {
                rp = new RelayProxy();
                break;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                Console.WriteLine($"端口 {MainModule.DST_PORT} 已在使用中。");
                Console.WriteLine("请检查此程序的另一个实例是否已经在运行中，或稍后重试。\n");
                Console.WriteLine("按回车键重试…\n");
                Console.WriteLine($"Port {MainModule.DST_PORT} is already in use.");
                Console.WriteLine("Please check if there is a duplicate instance, or wait before retrying.\n");
                Console.WriteLine("Press ENTER to retry...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("开始中转...");
        Console.WriteLine("Ready to relay...");
        rp.MainLoop();
    }
    public static bool debugMode = false;
    public static void ArgParse(string[] args)
    {
        if (args.Length == 0) return;
        switch (args[0])
        {
            case "debug":
                debugMode = true;
                break;
            case "setip":
                if (args.Length < 2) goto default;
                Console.WriteLine($"Setting master ip to {args[1]}...");
                DST_IP = args[1];
                break;
            case "help":
            default:
                Console.WriteLine("Usage: Relay9P.exe [option]");
                Console.WriteLine("Available options are:");
                Console.WriteLine("    help\tdisplay this message");
                Console.WriteLine("    setip\tmanually setup master server ip, usage: setip x.x.x.x");
                Console.WriteLine("    debug\tbe verbose with outputs");
                System.Environment.Exit(1);
                break;
        }
    }
    public static void DebugPrint(string msg)
    {
        if (debugMode) Console.WriteLine(msg);
    }
}