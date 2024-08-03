using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.Xml.Linq;


namespace Launcher
{
    public partial class Options
    {
        private readonly DoubleAnimation _oa;
        private bool _isPressed;

        public static SortedDictionary<int, string> ResolutionsInfo = new SortedDictionary<int, string>();

        public static void load_resolutions(string path)
        {
            ResolutionsInfo.Clear();
            XDocument doc = XDocument.Load(path);
            if (doc.Root == null) return;
            foreach (XElement el in doc.Root.Elements())
            {
                var index = Convert.ToInt32(el.Attribute("Index")?.Value);
                var name = Convert.ToString(el.Attribute("Name")?.Value);

                ResolutionsInfo.Add(index, name);
            }
        }

        public Options()
        {
            InitializeComponent();

            this.MouseDown += MainBgr_MouseDown;
            this.PreviewMouseDown += MainBgr_PreviewMouseDown;
            this.PreviewMouseUp += MainBgr_PreviewMouseUp;

            _oa = new DoubleAnimation {From = 0, To = 1, Duration = new Duration(TimeSpan.FromMilliseconds(800d))};

            BeginAnimation(OpacityProperty, _oa);
        }

        private void MainBgr_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isPressed = true;
            }
            else
            {
                _isPressed = false;
            }
        }

        private void MainBgr_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPressed = false;
        }

        private void MainBgr_MouseDown(object sender, MouseEventArgs e)
        {
            if (_isPressed)
            {
                this.DragMove();
            }
        }

        public void Close_Option_Window()
        {
            _oa.From = 1;
            _oa.To = 0;
            _oa.Completed += (send, ea) =>
            {
                this.Close();
            };
            _oa.Duration = new Duration(TimeSpan.FromMilliseconds(800d));

            BeginAnimation(OpacityProperty, _oa);
        }

        private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close_Option_Window();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close_Option_Window();
        }

        private void SaveRegistryConfigs()
        {
            try
            {
                var pair = ((KeyValuePair<int, string>)((ComboBoxItem)Resolution.Items[Resolution.SelectedIndex]).Tag);

                var key = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

                if (key == null) return;

                key.SetValue("Resolution", pair.Key, RegistryValueKind.DWord);

                key.SetValue("LangSelection", Language.SelectedIndex == 1 ? "Rus" : "Eng", RegistryValueKind.String);

                key.SetValue("ColorDepth", Color32.IsChecked == true ? 1 : 0, RegistryValueKind.DWord);

                key.SetValue("SoundOnOff", Effects.IsChecked == true ? "1" : "0", RegistryValueKind.DWord);

                key.SetValue("MusicOnOff", Music.IsChecked == true ? "1" : "0", RegistryValueKind.DWord);

                key.SetValue("WindowMode", WinMode.IsChecked == true ? "1" : "0", RegistryValueKind.DWord);

                key.SetValue("ID", AccountBox.Text, RegistryValueKind.String);

                key.SetValue("VolumeLevel", (Effects.IsChecked == true ? Convert.ToDouble(10) : 0), RegistryValueKind.DWord);

                string currentLanguage = ((ComboBoxItem) Language.SelectedItem).Tag.ToString();

                key.SetValue("LauncherLang", currentLanguage, RegistryValueKind.String);

                if (LanguageClass.CurrentLanguage != currentLanguage)
                {
                    System.Windows.Forms.Application.Restart();
                    Application.Current.Shutdown();
                }

                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadRegistryConfigs()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

                if (key == null) return;

                var login = key.GetValue("ID").ToString();
                AccountBox.Text = login;

                Language.SelectedIndex = 0;

                if (key.GetValue(@"SoundOnOff").ToString() == "1")
                    Effects.IsChecked = true;

                if (key.GetValue(@"MusicOnOff").ToString() == "1")
                    Music.IsChecked = true;

                if (key.GetValue(@"WindowMode").ToString() == "1")
                    WinMode.IsChecked = true;

                if (key.GetValue(@"ColorDepth").ToString() == "0")
                    Color16.IsChecked = true;

                if (key.GetValue(@"ColorDepth").ToString() == "1")
                    Color32.IsChecked = true;

                int resValue = int.Parse(key.GetValue(@"Resolution").ToString());

                if (!ResolutionsInfo.ContainsKey(resValue) || resValue < 0)
                    resValue = 0;

                for (int i = 0; i < Resolution.Items.Count; i++)
                {
                    var pair = ((KeyValuePair<int, string>) ((ComboBoxItem) Resolution.Items[i]).Tag);

                    if (pair.Key == resValue)
                        Resolution.SelectedIndex = i;
                }

                string langValue = key.GetValue(@"LauncherLang").ToString();

                for (int i = 0; i < Language.Items.Count; i++)
                {
                    if (((ComboBoxItem)Language.Items[i]).Tag.ToString() == langValue)
                        Language.SelectedIndex = i;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var pair in ResolutionsInfo)
                    Resolution.Items.Add(new ComboBoxItem {Content = pair.Value, Tag = pair});

                foreach (var lang in LanguageClass.LanguageList)
                    Language.Items.Add(new ComboBoxItem {Content = lang.Key.ToUpper(), Tag = lang.Key});

                LoadRegistryConfigs();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveRegistryConfigs();
            Close_Option_Window();
        }
    }
}
