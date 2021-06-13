using System;
using System.IO;
using LMSAnnouncements.Properties;
using Newtonsoft.Json;
namespace LMSAnnouncements
{
    class Settings
    {
        public string[] credentials { get; set; }
        public string TelegramToken { get; set; }
        public long ChatId { get; set; }
        public bool DebugMode { get; set; }
        public long DebugChatId { get; set; }
    }
    class Config
    {
        public static Settings LoadCfg()
        {
            if (File.Exists("config.json"))
            {
                var settings =  JsonConvert.DeserializeObject<Settings>(File.ReadAllText("config.json"));
                if(settings == null)
                {
                    File.WriteAllText("config.json", Resources.Settings);
                    return LoadCfg();
                }
                return settings;
            }
            else
            {
                File.WriteAllText("config.json", Resources.Settings);
                return LoadCfg();
            }
        }
    }
}
