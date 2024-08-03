using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Launcher
{
    public static class LanguageClass
    {
        //public static List<string> LanguagesList = new List<string>();

        public static SortedDictionary<string, SortedDictionary<int, string>> LanguageList = new SortedDictionary<string, SortedDictionary<int, string>>();

        public static string CurrentLanguage;

        public static void LoadLanguages(string path)
        {
            try
            {
                var files = Directory.GetFiles(path);

                LanguageList.Clear();
                GetCurrentLanguage();

                foreach (var file in files)
                {
                    var dictionary = new SortedDictionary<int, string>();

                    XDocument doc = XDocument.Load(file);
                    if (doc.Root == null) continue;
                    foreach (XElement el in doc.Root.Elements())
                    {
                        var index = Convert.ToInt32(el.Attribute("Index")?.Value);
                        var value = Convert.ToString(el.Attribute("Value")?.Value);

                        dictionary.Add(index, value);
                    }

                    LanguageList.Add(Path.GetFileNameWithoutExtension(file),dictionary);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        public static void GetCurrentLanguage()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

            CurrentLanguage = key?.GetValue("LauncherLang", "English").ToString();
        }

        public static string GetText(int index, params object[] values)
        {
            foreach (var pair in LanguageList[CurrentLanguage].Where(pair => pair.Key == index))
            {
                return values != null ? string.Format(pair.Value, values) : pair.Value;
            }

            return $"Text with '{index}' index not found...";
        }
    }
}
