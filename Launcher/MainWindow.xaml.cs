using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Xml;
using Launcher.Properties;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;
using Winforms = System.Windows.Forms;

namespace Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string[,] _rssData;

        private const string SiteAddress = "https://";

        private int _iUpdFileCnt;
        private double _totalFileSize;
        private const uint Key = 99;

        public Mutex LauncherMutex { get; }
        public Mutex LauncherClientMutex { get; private set; }

        private readonly Winforms.NotifyIcon _ni = new Winforms.NotifyIcon();
        private readonly BackgroundWorker _backgroundWorker1 = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundWorker2 = new BackgroundWorker();
        private readonly BackgroundWorker _backgroundWorker3 = new BackgroundWorker();
        private readonly ManualResetEvent _busy = new ManualResetEvent(false);
        private readonly Winforms.Timer _timerTime = new Winforms.Timer();
        private readonly DoubleAnimation _oa;
        private readonly Config _configs;

        private bool _isLauncherStarted;

        private bool _isPressed;

        public MainWindow()
        {
            InitializeComponent();

            _configs = Config.GetConfigs();
            _configs.LoadLocalConfig("Data/Launcher/lconfig.bmd", "28755");

            //ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };

            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 |
                                                   (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            _ni.Icon = Properties.Resources.favicon;
            _ni.DoubleClick += delegate
                { Show(); WindowState = WindowState.Normal; _ni.Visible = false; };
            MouseDown += MainBgr_MouseDown;
            PreviewMouseDown += MainBgr_PreviewMouseDown;
            PreviewMouseUp += MainBgr_PreviewMouseUp;

            Helper.check_registry();

            LanguageClass.LoadLanguages(Path.GetDirectoryName("Data/Launcher/Language/"));
            Options.load_resolutions("Data/Launcher/resolutions.xml");

            LauncherMutex = new Mutex(true, "LTPLAUNCHER_2.0", out var firstInstance);
            if (!firstInstance)
            {
                MessageBox.Show(LanguageClass.GetText(1), "Launcher");
                Close();
            }

            CheckMinimized();

            _backgroundWorker1.WorkerSupportsCancellation = true;
            _backgroundWorker2.WorkerSupportsCancellation = true;
            _backgroundWorker3.WorkerSupportsCancellation = true;

            _backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            _backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // backgroundWorker2
            // 
            _backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            _backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            //
            // backgroundworker3 
            //
            _backgroundWorker3.DoWork += backgroundWorker3_DoWork;
            _backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
            //backgroundWorker3.RunWorkerAsync();
            // ----
            Progressbar1.Width = 356;
            Progressbar2.Width = 356;
            Label1.Content = "";

            try
            {
                var bi = new BitmapImage();
                using (var fs = new FileStream("Data/Launcher/Image_1.png", FileMode.Open, FileAccess.Read))
                {
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = fs;
                    bi.EndInit();
                }

                CoverImage.Source = bi;
            }
            catch (Exception)
            {
                MessageBox.Show(LanguageClass.GetText(2), "Error");
            }

            RssDate1Lb.Content = DateTime.Now.ToString("F");


            if (_configs.RssLink.Length > 0)
            {
                SetRss();
            }

            CheckPort(_configs.ServerIp, int.Parse(_configs.CsPort), LabelCsState);
            CheckPort(_configs.ServerIp, int.Parse(_configs.GsPort), LabelGsState);

            _oa = new DoubleAnimation {From = 0, To = 1, Duration = new Duration(TimeSpan.FromMilliseconds(800d))};

            BeginAnimation(OpacityProperty, _oa);

            _timerTime.Tick += (timer1_Tick);
            _timerTime.Interval = 100;
            _timerTime.Start();
            LabelTimeValue.Foreground = Brushes.Green;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var time = DateTime.UtcNow;
            time = time.AddHours(_configs.TimeZone);
            LabelTimeValue.Content = time.ToString("HH:mm:ss");
        }

        public static string XOR_EncryptDecrypt(string str)
        {
            var f = str.ToCharArray();
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = (char)(f[i] ^ Key);
            }
            return new string(f);
        }

        private void CheckMinimized()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

            if (key == null) return;

            int windowMode = int.Parse(key.GetValue("WindowMode").ToString());

            WinMode.IsChecked = Convert.ToBoolean(windowMode);
        }

        private void MainBgr_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isPressed = e.ChangedButton == MouseButton.Left;
        }

        private void MainBgr_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPressed = false;
        }

        private void MainBgr_MouseDown(object sender, MouseEventArgs e)
        {
            if (_isPressed)
                DragMove();
        }

        private void button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _oa.From = 1;
            _oa.To = 0;
            _oa.Completed += (send, ea) =>
            {
                Environment.Exit(0);
            };
            _oa.Duration = new Duration(TimeSpan.FromMilliseconds(800d));

            BeginAnimation(OpacityProperty, _oa);            
        }

        private void WinMode_Click(object sender, RoutedEventArgs e)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Webzen\Mu\Config", true);

            key?.SetValue("WindowMode", Convert.ToInt32(WinMode.IsChecked), RegistryValueKind.DWord);
        }

        private void OptionBtn_Click(object sender, RoutedEventArgs e)
        {
            Options optionForm = new Options {Owner = this, ShowInTaskbar = false};
            optionForm.Show();
        }

        private void Minimaze_btn_Click(object sender, RoutedEventArgs e)
        {
            Minimaze();
        }

        private void Minimaze()
        {
            WindowState = WindowState.Minimized;
            if (WindowState.Minimized != WindowState) return;

            _ni.Visible = true;
            Hide();
            _ni.ShowBalloonTip(1000, "Launcher", LanguageClass.GetText(3), Winforms.ToolTipIcon.Info);
        }

        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebClient webClient = new WebClient();
                Label1.Content = LanguageClass.GetText(4);
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;

                try
                {
                    //MiniUpdate
                    webClient.DownloadFileCompleted += MiniUpdDownloadFileCompleted;
                    webClient.DownloadFileAsync(new Uri(SiteAddress + "update.info"), "update.info", "update.info");
                    _flag = 1;
                }
                catch (Exception webEx)
                {
                    MessageBox.Show(webEx.Message);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SetTextLabel1(string s)
        {
            if (Label1 != null)
            {
                Label1.Content = s;
            }
        }
        public void SetProgBar(int value)
        {
            TaskbarItemInfo.ProgressValue = (double) value / 100;
            Progressbar1.Height = 10;
            Progressbar1.Width = (int)(value * 3.56);
        }
        public void SetProgBar2(int value)
        {
            if (value <= 0)
            {
                Progressbar2.Height = 10;
                Progressbar2.Width = 0;
            }
            else
            {
                Progressbar2.Height = 10;
                Progressbar2.Width = (int)(value * 3.56);
            }
           
        }
        public void SetPictureBox4(string s)
        {

        }
        public void SetPictureBox7(string s)
        {

        }

        private void CheckPort(string ip, int port, Label lb)
        {
            TcpClient tcpScan = new TcpClient();
            var result = tcpScan.BeginConnect(ip, port, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            if (!success)
            {
                lb.Foreground = Brushes.Red;
                lb.Content = LanguageClass.GetText(5);
            }
            else
            {
                lb.Foreground = Brushes.Green;
                lb.Content = LanguageClass.GetText(6);
            }
        }

        private void SetTextLabelInvoke(string s, int type)
        {
            if (!Dispatcher.CheckAccess())
            {
                switch (type)
                {
                    case 1:
                        Dispatcher.Invoke(new Action<string>(SetTextLabel1), s);
                        break;
                    case 2:
                        Dispatcher.Invoke(new Action<string>(SetPictureBox4), s);
                        break;
                    case 3:
                        Dispatcher.Invoke(new Action<string>(SetPictureBox7), s);
                        break;
                    case 4:
                        Dispatcher.Invoke(new Action<int>(SetProgBar), int.Parse(s));
                        break;
                    case 5:
                        Dispatcher.Invoke(new Action<int>(SetProgBar2), int.Parse(s));
                        break;
                    case 6:
                        Dispatcher.Invoke(new Action<int>(SetLabelUpdDw), int.Parse(s));
                        break;
                    case 7:
                        Dispatcher.Invoke(new Action<int>(SetLabelTotalDw), int.Parse(s));
                        break;
                }
            }
            else

                switch (type)
                {
                    case 1:
                        SetTextLabel1(s);
                        break;
                    case 2:
                        SetPictureBox4(s);
                        break;
                    case 3:
                        SetPictureBox7(s);
                        break;
                    case 4:
                        SetProgBar(int.Parse(s));
                        break;
                    case 5:
                        SetProgBar2(int.Parse(s));
                        break;
                    case 6:
                        SetLabelUpdDw(int.Parse(s));
                        break;
                    case 7:
                        SetLabelTotalDw(int.Parse(s));
                        break;
                }
        }

        private void MiniUpdDownloadFileCompleted(object sender, AsyncCompletedEventArgs evA)
        {
            if (evA.Error != null)
            {
                string filename = (string)evA.UserState;
                Label1.Content = LanguageClass.GetText(7);
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                File.Delete(filename);
            }
            else
            {
                if (_backgroundWorker3.IsBusy != true)
                {
                    _backgroundWorker3.RunWorkerAsync();
                    
                }
            }
        }

        private void _DownloadCheckSumFileCompleted(object sender, AsyncCompletedEventArgs evA)
        {
            try
            {
                _progbytes += _newbytes;
                _newbytes = 0;
                _bytesold = 0;
                if (evA.Error != null)
                {
                   
                    string filename = (string)evA.UserState;
                    SetTextLabelInvoke(LanguageClass.GetText(7), 1);
                    File.Delete(filename);
                }

                _busy.Set();
            }
            catch (Exception)
            {
                //MessageBox.Show("Failed to download! [" + (string)evA.UserState + "]");
                MessageBox.Show(LanguageClass.GetText(8, (string)evA.UserState));
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (File.Exists("client.info"))
                {
                    FileStream clientInfo = new FileStream("client.info", FileMode.Open);
                    BinaryReader binRead = new BinaryReader(clientInfo);

                    _iUpdFileCnt = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                    int iCurFile = 0;

                    SetStateLabel(LanguageClass.GetText(9));

                    while (binRead.PeekChar() > 0)
                    {
                        if (_backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        if (iCurFile <= _iUpdFileCnt)
                        {
                            var iPerc = (iCurFile * 100) / _iUpdFileCnt;
                            SetTextLabelInvoke("" + iPerc, 3);
                            iCurFile++;
                        }

                        string fileDir = XOR_EncryptDecrypt(binRead.ReadString());
                        string fileHash = XOR_EncryptDecrypt(binRead.ReadString());

                        //Create directory if doesn't exist
                        FileInfo file = new FileInfo(fileDir);
                        file.Directory?.Create();

                        SetTextLabelInvoke(LanguageClass.GetText(10, fileDir), 1);

                        //Get CRC32
                        try
                        {
                            Crc32 crc32 = new Crc32();
                            string hash = string.Empty;

                            if (File.Exists(fileDir))
                            {
                                var fs = File.ReadAllBytes(fileDir);
                                foreach (byte b in crc32.ComputeHash(fs))
                                {
                                    hash += b.ToString("x2").ToUpper();
                                }
                            }
                            else
                            {
                                hash = null;
                            }

                            //UpdDir(Full)
                            if (hash == null || hash != fileHash)
                            {
                                if (hash != fileHash)
                                {
                                    WebClient checkSumDownload = new WebClient();
                                    //_CheckSumDownload.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                    checkSumDownload.DownloadFileAsync(new Uri(SiteAddress + "FullUpdate/" + fileDir), fileDir, fileDir);
                                    checkSumDownload.DownloadFileCompleted += _DownloadCheckSumFileCompleted;
                                    SetTextLabelInvoke(LanguageClass.GetText(11, fileDir), 1);
                                    SetStateLabel(LanguageClass.GetText(12));
                                    _busy.WaitOne();
                                    _busy.Reset();
                                }
                            }
                        }
                        catch (Exception myExc)
                        {
                            MessageBox.Show(myExc.Message);
                            Environment.Exit(0);
                        }
                    }

                    binRead.Close();

                    SetTextLabelInvoke("true", 2);
                    //setTextLabelInvoke("true", 4);
                    SetTextLabelInvoke("100", 3);
                    SetTextLabelInvoke(LanguageClass.GetText(13), 1);
                }
                else
                {
                    SetTextLabelInvoke("true", 2);
                    SetTextLabelInvoke(LanguageClass.GetText(7), 1);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (File.Exists("update.info"))
                {
                    FileStream clientInfo = new FileStream("update.info", FileMode.Open);
                    BinaryReader binRead = new BinaryReader(clientInfo);

                    _iUpdFileCnt = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                    Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                    int iCurFile = 0;

                    SetStateLabel(LanguageClass.GetText(9));

                    while (binRead.PeekChar() > 0)
                    {
                        if (_backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        if (iCurFile <= _iUpdFileCnt)
                        {
                            var iPerc = (iCurFile * 100) / _iUpdFileCnt;
                            SetTextLabelInvoke("" + iPerc, 3);
                            iCurFile++;
                        }

                        string fileDir = XOR_EncryptDecrypt(binRead.ReadString());
                        string fileHash = XOR_EncryptDecrypt(binRead.ReadString());
                        double fileSize = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));

                        //Create directory if doesn't exist
                        FileInfo file = new FileInfo(fileDir);
                        file.Directory?.Create();

                        SetTextLabelInvoke(LanguageClass.GetText(10,fileDir), 1);

                        //Get CRC32
                        try
                        {
                            Crc32 crc32 = new Crc32();
                            string hash = string.Empty;

                            if (File.Exists(fileDir))
                            {
                                var fs = File.ReadAllBytes(fileDir);
                                foreach (byte b in crc32.ComputeHash(fs))
                                {
                                    hash += b.ToString("x2").ToUpper();
                                }
                            }
                            else
                            {
                                hash = null;
                            }
                            //UpdDir(Mini)
                            if (hash == null || hash != fileHash)
                            {
                                if (hash != fileHash)
                                {
                                    WebClient checkSumDownload = new WebClient();
                                    string fileName = fileDir;
                                    if (fileDir == "Launcher.exe") { fileName = "NewLauncher.exe"; }

                                    //File.AppendAllText("log.txt", SiteAdress + "MiniUpdate/" + fileDir + "\t Size = " + FileSize.ToString() + Environment.NewLine);

                                    checkSumDownload.DownloadProgressChanged += client_DownloadProgressChanged;
                                    checkSumDownload.DownloadFileAsync(new Uri(SiteAddress + fileDir), fileName, fileName);
                                    checkSumDownload.DownloadFileCompleted += _DownloadCheckSumFileCompleted;
                                    SetTextLabelInvoke(LanguageClass.GetText(11,fileDir), 1);
                                    SetStateLabel(LanguageClass.GetText(12));
                                    _busy.WaitOne();
                                    _busy.Reset();
                                }
                            }
                        }
                        catch (Exception myExc)
                        {
                            MessageBox.Show(myExc.Message);
                            Environment.Exit(0);
                        }
                    }

                    binRead.Close();

                    SetTextLabelInvoke("true", 2);
                    SetTextLabelInvoke("100", 3);
                    SetTextLabelInvoke(LanguageClass.GetText(13), 1);
                }
                else
                {
                    SetTextLabelInvoke("true", 2);
                    SetTextLabelInvoke(LanguageClass.GetText(7), 1);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void SetLabelTotalDw(int value)
        {
            LabelTotalDw.Content = $"{value}%";
        }

        private void SetLabelUpdDw(int value)
        {
            LabelUpdDw.Content = $"{value}%";
        }

        private double _newbytes;
        private double _bytesold;
        private double _progbytes;
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            SetTextLabelInvoke(int.Parse(Math.Truncate(percentage).ToString()).ToString(), 5);
            SetTextLabelInvoke(int.Parse(Math.Truncate(percentage).ToString()).ToString(), 6);

            double totalbytes = _totalFileSize;
            double newpercent = (bytesIn + _progbytes) / totalbytes * 100;
            double bytes = bytesIn;
            bytesIn -= _bytesold;
            _bytesold = bytes;
            _newbytes += bytesIn;

            SetTextLabelInvoke(int.Parse(Math.Truncate(newpercent).ToString()).ToString(), 4);
            SetTextLabelInvoke(int.Parse(Math.Truncate(newpercent).ToString()).ToString(), 7);

        }

        private int _flag;

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                SelfUpdate();

                var clientInfoPath = _executablePath + @"\client.info";

                if (File.Exists(clientInfoPath))
                {
                    File.Delete(clientInfoPath);
                }

                var updateInfoPath = _executablePath + @"\update.info";

                if (File.Exists(updateInfoPath))
                {
                    File.Delete(updateInfoPath);
                }

                SetStateLabel(LanguageClass.GetText(14));

                if (_flag != 1) return;

                try
                {
                    //CreateConnectionKeys();
                    if (_isLauncherStarted == false)
                    {
                        LauncherClientMutex = new Mutex(false, "LTPLAUNCHERSTART", out _isLauncherStarted);
                    }

                    Process.Start(Winforms.Application.StartupPath + "\\" + _configs.StartFile, "Updater");
                    //Environment.Exit(0);
                    Minimaze();
                }
                catch (Exception ma)
                {
                    MessageBox.Show(ma.Message);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                SelfUpdate();

                var clientInfoPath = _executablePath + @"\client.info";

                if (File.Exists(clientInfoPath))
                {
                    File.Delete(clientInfoPath);
                }

                var updateInfoPath = _executablePath + @"\update.info";

                if (File.Exists(updateInfoPath))
                {
                    File.Delete(updateInfoPath);
                }

                SetStateLabel("Updating complete.");

                //TaskbarProgress.SetState(hwnd,TaskbarProgress.TaskbarStates.NoProgress);
                TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;

                if (_flag != 1) return;

                try
                {
                    //CreateConnectionKeys();
                    if (_isLauncherStarted == false)
                    {
                        LauncherClientMutex = new Mutex(false, "LTPLAUNCHERSTART", out _isLauncherStarted);
                    }

                    Process.Start(Winforms.Application.StartupPath + "\\" + _configs.StartFile, "Updater");
                    Environment.Exit(0);
                }
                catch (Exception ma)
                {
                    MessageBox.Show(ma.Message);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (!File.Exists("update.info")) return;

                FileStream clientInfo = new FileStream("update.info", FileMode.Open);
                BinaryReader binRead = new BinaryReader(clientInfo);

                _iUpdFileCnt = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));
                double totalFileSize1 = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));

                while (binRead.PeekChar() > 0)
                {
                    if (_backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    string fileDir = XOR_EncryptDecrypt(binRead.ReadString());
                    string fileHash = XOR_EncryptDecrypt(binRead.ReadString());
                    double fileSize = Convert.ToInt32(XOR_EncryptDecrypt(binRead.ReadString()));

                    //Create directory if doesn't exist
                    FileInfo file = new FileInfo(fileDir);
                    file.Directory?.Create();

                    //Get CRC32
                    try
                    {
                        Crc32 crc32 = new Crc32();
                        string hash = string.Empty;

                        if (File.Exists(fileDir))
                        {
                            var fs = File.ReadAllBytes(fileDir);
                            foreach (byte b in crc32.ComputeHash(fs))
                            {
                                hash += b.ToString("x2").ToUpper();
                            }
                        }
                        else
                        {
                            hash = null;
                        }
                        //UpdDir(Mini)
                        if (hash == null || hash != fileHash)
                        {
                            if (hash != fileHash)
                            {
                                _totalFileSize += fileSize;
                                //MessageBox.Show(String.Format("Total={0} Size={1}", TotalFileSize, FileSize));
                            }
                        }
                    }
                    catch (Exception myExc)
                    {
                        MessageBox.Show(myExc.Message);
                        Environment.Exit(0);
                    }
                }

                binRead.Close();

            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (_backgroundWorker2.IsBusy != true)
                {
                    _backgroundWorker2.RunWorkerAsync();

                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //----

        private void SetStateLabel(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Winforms.MethodInvoker(() =>
                {
                    StateLabel.Content = text;
                }));
            }
            else
            {
                StateLabel.Content = text;
            }
        }

        private readonly string _executablePath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

        private void RunUpdater()
        {
            var isNewLauncherPresent = CheckIsNewLauncherPresent(_executablePath + @"\NewLauncher.exe");

            if (!isNewLauncherPresent) return;

            //log: MessageBox.Show("NewLauncher is present.", "MuLauncher");
            var process = new Process
            {
                StartInfo = {Verb = "runas", FileName = _executablePath + @"\MuUpdater.exe"}
            };
            process.Start();
            Environment.Exit(0);
        }

        private void SelfUpdate()
        {
            try
            {
                var isUpdated = GetSelfUpdateState();
                if (isUpdated)
                {
                    RunUpdater();
                }
                else
                {
                    RunUpdater();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SelfUpdate() {ex.Message}");
            }
        }

        private static bool GetSelfUpdateState()
        {
            try
            {
                var isSelfUpdatePresent = Settings.Default.IsSelfUpdatePresent;
                //log: MessageBox.Show(string.Format("IsSelfUpdatePresent: {0}", IsSelfUpdatePresent));
                return isSelfUpdatePresent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetSelfUpdateState(). {ex.Message}");
                return false;
            }
        }

        private static void SetSelfUpdateState(bool state)
        {
            try
            {
                Settings.Default.IsSelfUpdatePresent = state;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SetSelfUpdateState() {ex.Message}");
            }
        }

        private static bool CheckIsNewLauncherPresent(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    SetSelfUpdateState(true);
                    return true;
                }

                SetSelfUpdateState(false);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CheckIsNewLauncherPresent() {ex.Message}");
                SetSelfUpdateState(false);
                return false;
            }
        }

        private static string[,] GetRssData(string channel)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(channel);
                myRequest.UserAgent = "LTPTeam_Launcher";
                WebResponse myResponse = myRequest.GetResponse();

                Stream rssStream = myResponse.GetResponseStream();
                XmlDocument rssDoc = new XmlDocument();

                rssDoc.Load(rssStream ?? throw new InvalidOperationException());

                XmlNodeList rssItems = rssDoc.SelectNodes("rss/channel/item");

                var tempRssData = new string[100, 3];

                if (rssItems == null) return tempRssData;

                for (int i = 0; i < rssItems.Count; i++)
                {
                    var rssNode = rssItems.Item(i)?.SelectSingleNode("title");
                    if (rssNode != null)
                    {
                        tempRssData[i, 0] = rssNode.InnerText;
                    }
                    else
                    {
                        tempRssData[i, 0] = "";
                    }

                    rssNode = rssItems.Item(i)?.SelectSingleNode("pubDate");
                    if (rssNode != null)
                    {
                        tempRssData[i, 1] = rssNode.InnerText;
                    }
                    else
                    {
                        tempRssData[i, 1] = "";
                    }

                    rssNode = rssItems.Item(i)?.SelectSingleNode("link");
                    if (rssNode != null)
                    {
                        tempRssData[i, 2] = rssNode.InnerText;
                    }
                    else
                    {
                        tempRssData[i, 2] = "";
                    }
                }

                return tempRssData;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                var tempRssData = new string[100, 3];
                return tempRssData;
            }
        }

        private void SetRss()
        {
            Rss1Lb.Content = "";
            Rss2Lb.Content = "";
            Rss3Lb.Content = "";

            RssDate1Lb.Content = "";
            RssDate2Lb.Content = "";
            RssDate3Lb.Content = "";

            Rss1Lb.Visibility = Visibility.Hidden;
            Rss2Lb.Visibility = Visibility.Hidden;
            Rss3Lb.Visibility = Visibility.Hidden;

            RssDate1Lb.Visibility = Visibility.Hidden;
            RssDate2Lb.Visibility = Visibility.Hidden;
            RssDate3Lb.Visibility = Visibility.Hidden;
            // ----
            _rssData = GetRssData(_configs.RssLink);
            for (int i = 0; i < 3; i++)
            {
                if (_rssData[i, 0] == null) continue;

                switch (i)
                {
                    case 0:
                        Rss1Lb.Visibility = Visibility.Visible;
                        RssDate1Lb.Visibility = Visibility.Visible;

                        Rss1Lb.Content = _rssData[i, 0];
                        RssDate1Lb.Content = _rssData[i, 1];
                        break;
                    case 1:
                        Rss2Lb.Visibility = Visibility.Visible;
                        RssDate2Lb.Visibility = Visibility.Visible;

                        Rss2Lb.Content = _rssData[i, 0];
                        RssDate2Lb.Content = _rssData[i, 1];
                        break;
                    case 2:
                        Rss3Lb.Visibility = Visibility.Visible;
                        RssDate3Lb.Visibility = Visibility.Visible;

                        Rss3Lb.Content = _rssData[i, 0];
                        RssDate3Lb.Content = _rssData[i, 1];
                        break;
                }
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetStateLabel(LanguageClass.GetText(15));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void RSS1_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (_rssData[0, 2] == null)
            {
                return;
            }
            Process.Start(_rssData[0, 2]);
        }

        private void RSS2_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (_rssData[1, 2] == null)
            {
                return;
            }
            Process.Start(_rssData[1, 2]);
        }

        private void RSS3_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (_rssData[2, 2] == null)
            {
                return;
            }
            Process.Start(_rssData[2, 2]);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //BeginAnimation(OpacityProperty, _oa);
        }
    }
}
