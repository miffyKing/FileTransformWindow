using Newtonsoft.Json;
using System.IO;

namespace checkFileContent.NonFormClasses
{
    public class SettingsManager
    {
        private const string SettingsFilePath = "appsettings.json";
        private AppSettings settings;

        public SettingsManager()
        {
            LoadSettings();
        }

        public AppSettings Settings => settings;

        private void LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                string json = File.ReadAllText(SettingsFilePath);
                settings = JsonConvert.DeserializeObject<AppSettings>(json);
            }
            else
            {
                settings = new AppSettings();
            }
        }

        public void SaveSettings()
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }
    }
}
