using System.Diagnostics;
using System.Reflection;

namespace RF.WPF
{
    public static class Startup
    {
        public static void AddToStartup(string args = "")
        {
            Microsoft.Win32.RegistryKey? key = null;
            try
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                var proc = Process.GetCurrentProcess();
                string name = proc.ProcessName;
                string location = proc.MainModule.FileName;
                key.SetValue(name, $"\"{location}\" {args}");
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
                Assembly curAssembly = Assembly.GetExecutingAssembly();
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
