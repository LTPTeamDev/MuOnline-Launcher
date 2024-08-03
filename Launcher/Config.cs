using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Launcher
{
    class Config
    {
        public int SiteAddressActive = 1;

        #region Properties

        private static Config _instance;

        public string StartFile { get; set; }

        public string RssLink { get; set; }

        public string ServerIp { get; set; }

        public string GsPort { get; set; }

        public string CsPort { get; set; }

        public int TimeZone { get; set; }

        public string SiteAddress { get; set; }

        #endregion

        public static Config GetConfigs()
        {
            if (_instance == null)
            {
                _instance = new Config();
            }
            return _instance;
        }

        protected Config()
        {

        }

        public void LoadLocalConfig(string filePath, string encryptKey)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show(LanguageClass.GetText(0), @"Launcher");
                    Environment.Exit(0);
                }
                string[] configsList = Regex.Split(SecureStringManager.Decrypt(File.ReadAllText(filePath), encryptKey), "\r\n");
                _instance.RssLink = configsList[0];
                _instance.ServerIp = configsList[1];
                _instance.GsPort = configsList[2];
                _instance.CsPort = configsList[3];
                _instance.StartFile = configsList[4];
                _instance.TimeZone = Convert.ToInt32(configsList[5]);
                SiteAddressActive = Convert.ToInt32(configsList[6]);
                if (SiteAddressActive == 1)
                {
                    _instance.SiteAddress = configsList[7];
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
