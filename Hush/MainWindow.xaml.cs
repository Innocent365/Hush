using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;


namespace Hush
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitNotifyIcon();

            _hotkey = new HotKey(ModifierKeys.Alt, Keys.Y, this);


            _hotkey.HotKeyPressed += (p) =>
            {                
                _notifyIcon.Visible = true;
                //MessageBox.Show("show me");
                var timer = new Timer {Interval = 10000};
                timer.Tick += (sender, args) =>
                {
                    _notifyIcon.Visible = false;
                    timer.Dispose();
                };
                timer.Start();                
            };            
        }

        private NotifyIcon _notifyIcon;
        private readonly HotKey _hotkey;
        private void InitNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();

            this._notifyIcon.Text = "Hush";
            this._notifyIcon.Icon = new System.Drawing.Icon("ico.ico");//new Icon("onesail.ico");//

            this._notifyIcon.Visible = false;            
            this._notifyIcon.DoubleClick += new System.EventHandler(notifyIcon_DoubleClick);
            
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {            
            this.WindowState = WindowState.Normal;
            this.Show();
            this.Activate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;            
            this.Hide();
            _notifyIcon.Visible = false;
        }
    }
}
