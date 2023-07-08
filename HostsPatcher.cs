public class HostsPatcher
{
    string hostfile;
    public HostsPatcher()
    {
        var OSInfo = Environment.OSVersion;
        string pathpart = "hosts";
        if (OSInfo.Platform == PlatformID.Win32NT)
        {
            // windows NT
            pathpart = "system32\\drivers\\etc\\hosts";
        }
        hostfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), pathpart);
    }
    public bool IsPatched()
    {
        var lines = File.ReadAllLines(hostfile);
        foreach (string s in lines)
        {
            string stripped = s.Trim();
            if (!stripped.StartsWith("#") && stripped.Contains(MainModule.DST_URL))
            {
                return true;
            }
        }
        return false;
    }
    public void Patch()
    {
        using (var sw = File.AppendText(hostfile))
        {
            sw.WriteLine("\n127.0.0.1 " + MainModule.DST_URL);
        }
    }
}