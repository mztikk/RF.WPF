using System.Reflection;

namespace RF.WPF
{
    public static class Startup
    {
        public static void AddToStartup()
        {
            Microsoft.Win32.RegistryKey? key = null;
            try
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetEntryAssembly();
                string name = curAssembly.GetName().Name;
                key.SetValue(name, curAssembly.Location);
            }
            finally
            {
                key?.Close();
                key?.Dispose();
            }
        }

        public static void RemoveFromStartup()
        {
            Microsoft.Win32.RegistryKey? key = null;
            try
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetEntryAssembly();
                string name = curAssembly.GetName().Name;
                if (key.GetValue(name) is { })
                {
                    key.DeleteValue(name);
                }
            }
            finally
            {
                key?.Close();
                key?.Dispose();
            }
        }
    }
}
