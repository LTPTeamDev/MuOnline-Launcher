using Microsoft.Win32;

namespace Launcher
{
    public static class Helper
    {
        public static void check_registry()
        {
            check_and_create_registry("LauncherLang", "English", RegistryValueKind.String);
            check_and_create_registry("LangSelection", "Eng", RegistryValueKind.String);
            check_and_create_registry("ID", "", RegistryValueKind.String);

            check_and_create_registry("WindowMode", 0, RegistryValueKind.DWord);
            check_and_create_registry("Resolution", 0, RegistryValueKind.DWord);
            check_and_create_registry("ColorDepth", 1, RegistryValueKind.DWord);
            check_and_create_registry("SoundOnOff", 0, RegistryValueKind.DWord);
            check_and_create_registry("MusicOnOff", 0, RegistryValueKind.DWord);
            check_and_create_registry("VolumeLevel", 0, RegistryValueKind.DWord);
        }

        public static void check_and_create_registry(string name, object defaultValue, RegistryValueKind type)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true) ??
                      Registry.CurrentUser.CreateSubKey(@"Software\Webzen\Mu\Config");

            if (key?.GetValue(name) == null)
                key?.SetValue(name, defaultValue, type);

            key?.Close();
        }
    }
}
