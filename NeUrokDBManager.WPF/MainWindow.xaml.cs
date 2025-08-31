using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Microsoft.Win32;
using NeUrokDBManager.WPF.Views.Pages;

namespace NeUrokDBManager.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool _isWindowMoving = false;
        private bool _isCanResize = false;
        private bool _resizeInProcess = false;
        private Point _mousePosition;

        public Frame MFrame => MainFrame;

        public MainWindow()
        {
            InitializeComponent();
            SetAutoRunValue(true, Assembly.GetExecutingAssembly().Location);
            MyNavigationService.MainWindow = this;
            MyNavigationService.MainFrame = MainFrame;
            MyNavigationService.DbContext = new Infrastructure.ApplicationDbContext();
            MyNavigationService.NavigateTo<MenuPage>();

        }
        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            UpdateWindowSize();
        }

        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            if (!_isCanResize) return;
            Rectangle? senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                _resizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            Rectangle? senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                _resizeInProcess = false; ;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (_resizeInProcess)
            {
                double temp = 0;
                Rectangle? senderRect = sender as Rectangle;
                Window mainWindow = this;
                if (senderRect != null)
                {
                    double width = e.GetPosition(mainWindow).X;
                    double height = e.GetPosition(mainWindow).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name.Contains("right", StringComparison.OrdinalIgnoreCase))
                    {
                        width += 5;
                        if (width > 0)
                            mainWindow.Width = width;
                    }
                    if (senderRect.Name.Contains("left", StringComparison.OrdinalIgnoreCase))
                    {
                        width -= 5;
                        temp = mainWindow.Width - width;
                        if ((temp > mainWindow.MinWidth) && (temp < mainWindow.MaxWidth))
                        {
                            mainWindow.Width = temp;
                            mainWindow.Left += width;
                        }
                    }
                    if (senderRect.Name.Contains("bottom", StringComparison.OrdinalIgnoreCase))
                    {
                        height += 5;
                        if (height > 0)
                            mainWindow.Height = height;
                    }
                    if (senderRect.Name.ToLower().Contains("top", StringComparison.OrdinalIgnoreCase))
                    {
                        height -= 5;
                        temp = mainWindow.Height - height;
                        if ((temp > mainWindow.MinHeight) && (temp < mainWindow.MaxHeight))
                        {
                            mainWindow.Height = temp;
                            mainWindow.Top += height;
                        }
                    }

                }
            }
        }


        private void UpdateWindowSize()
        {
            if (MainFrame.Content is Page page)
            {
                Width = page.Width;
                Height = page.Height + ControlPanel.Height;
                //PageContainer.Width = page.Width;
                //PageContainer.Height = page.Height;

                PageContainer.Effect = new DropShadowEffect
                {
                    Color = Colors.Purple,
                    BlurRadius = 20,
                    ShadowDepth = 0,
                    Opacity = 0.7
                };

                WindowName.Text = page.Title;
                MinHeight = page?.MinHeight + ControlPanel.Height ?? 200;

                _isCanResize = page is not MenuPage;
            }
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void CloseBtn_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void FullScreenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isWindowMoving) return;

            Point currentPosition = PointToScreen(e.GetPosition(this));
            Vector delta = currentPosition - new Point(_mousePosition.X + Left, _mousePosition.Y + Top);

            Left = currentPosition.X - _mousePosition.X;
            Top = currentPosition.Y - _mousePosition.Y;

            if ((delta.X != 0 || delta.Y != 0) && WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;

                // Корректируем позицию после выхода из максимизированного состояния
                Left = _mousePosition.X;
                Top = _mousePosition.Y;
                _isWindowMoving = false;
            }
        }

        private void ControlPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isWindowMoving = true;
            _mousePosition = e.GetPosition(this);
        }

        private void ControlPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isWindowMoving = false;

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private bool SetAutoRunValue(bool autoRun, string path)
        {
            const string name = "NeUrokDBManager";
            string exePath = path;
            RegistryKey reg;

            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            try
            {
                if (autoRun)
                    reg.SetValue(name, exePath);
                else
                    reg.DeleteValue(name);

                reg.Flush();
                reg.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

    }
}