
using Microsoft.Win32;
using System.Security.AccessControl;

class Program
{

    private const string LockScreen = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\PersonalizationCSP";

    static void Main(string[] args)
    {
        OpenRegKey(LockScreen, reg =>
        {
            var security = reg.GetAccessControl();
            security.AddAccessRule(new RegistryAccessRule("Users", RegistryRights.FullControl, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));
            reg.SetAccessControl(security);
        }, true);
    }


    private static void OpenRegKey(string path, Action<RegistryKey> callback, bool root = false)
    {
        var reg = (root ? Registry.LocalMachine: Registry.CurrentUser).CreateSubKey(path);
        if (reg != null)
        {
            callback(reg);
        }
        reg?.Close ();
    }
}