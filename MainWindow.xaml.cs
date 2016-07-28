using Demo4.Controller;
using Demo4.WindowSetting;
using SuperMap.Data;
using SuperMap.Realspace;
using SuperMap.UI;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Demo4
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly int customBorderThickness = 5;


        /// <summary>  
        /// Corner width used in HitTest  
        /// </summary>  
        private readonly int cornerWidth = 6;

        /// <summary>  
        /// Mouse point used by HitTest  
        /// </summary>  
        private Point mousePoint = new Point();  


        private CoreController m_controller;
        private SceneControl m_sceneControl;

        public MainWindow()
        {
            InitializeComponent();

            this.SourceInitialized += MainWindow_SourceInitialized;
            this.StateChanged += MainWindow_StateChanged;
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            m_sceneControl = new SceneControl();
            this.controlForm.Child = m_sceneControl;

            m_controller = new CoreController(m_sceneControl);
        }

        // 关闭窗体
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_controller.Close();
        }

        // 点击弹框按钮
        private void bubble_Click(object sender, RoutedEventArgs e)
        {
            this.location.Content = "允许打地标";
            if (m_controller.Bubble())
            {
                this.bubble.Content = "禁止弹框";
            }
            else
            {
                this.bubble.Content = "允许弹框";
            }
        }

        // 点击加地标按钮
        private void location_Click(object sender, RoutedEventArgs e)
        {
            this.bubble.Content = "允许弹框";
            if (m_controller.Location())
            {
                this.location.Content = "禁止加地标";
            }
            else
            {
                this.location.Content = "允许加地标";
            }
        }

        // 地址搜索 
        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            this.searchResult.ItemsSource = null;
            this.searchResult.ItemsSource = m_controller.SearchAddress(this.searchText.Text);
            this.searchText.Text = "";
        }

        // 量算选择
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox cb = sender as System.Windows.Controls.ComboBox;
            int select = cb.SelectedIndex;
            switch(select)
            {
                case 0:     // 距离量算
                    m_controller.MeasureDistance();
                    break;
                case 1:     // 面积量算
                    m_controller.MeasureArea();
                    break;
                case 2:     // 结束量算
                    m_controller.EndMeasure();
                    break;
                case 3:     // 清空量算结果
                    m_controller.ClearMeasure();
                    break;
            }
        }
        
        // 显示、隐藏帧率
        private void fpsDisplay_Click(object sender, RoutedEventArgs e)
        {
            m_controller.FPS();
        }
        
        // 显示、隐藏经纬网
        private void latlonDisplay_Click(object sender, RoutedEventArgs e)
        {
            m_controller.LatLon();
        }
        
        // 绕物旋转
        private void rotate_Click(object sender, RoutedEventArgs e)
        {
            m_controller.Rotate(3);
        }




        /************窗体风格设置************/

        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource.Equals(this.titleBar))
            {
                WindowInteropHelper wih = new WindowInteropHelper(this);
                Win32.SendMessage(wih.Handle, Win32.WM_NCLBUTTONDOWN, (int)Win32.HitTest.HTCAPTION, 0);
                return;
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            switch (msg)
            {
                case Win32.WM_GETMINMAXINFO: // WM_GETMINMAXINFO message  
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                case Win32.WM_NCHITTEST: // WM_NCHITTEST message  
                    return WmNCHitTest(lParam, ref handled);
            }

            return IntPtr.Zero;
        }

        private IntPtr WmNCHitTest(IntPtr lParam, ref bool handled)
        {
            if (this.WindowState == WindowState.Maximized)
                return IntPtr.Zero;
            // Update cursor point  
            // The low-order word specifies the x-coordinate of the cursor.  
            // #define GET_X_LPARAM(lp) ((int)(short)LOWORD(lp))  
            this.mousePoint.X = (int)(short)(lParam.ToInt32() & 0xFFFF);
            // The high-order word specifies the y-coordinate of the cursor.  
            // #define GET_Y_LPARAM(lp) ((int)(short)HIWORD(lp))  
            this.mousePoint.Y = (int)(short)(lParam.ToInt32() >> 16);

            // Do hit test  
            handled = true;
            if (Math.Abs(this.mousePoint.Y - this.Top) <= this.cornerWidth
                && Math.Abs(this.mousePoint.X - this.Left) <= this.cornerWidth)
            { // Top-Left  
                return new IntPtr((int)Win32.HitTest.HTTOPLEFT);
            }
            else if (Math.Abs(this.ActualHeight + this.Top - this.mousePoint.Y) <= this.cornerWidth
                && Math.Abs(this.mousePoint.X - this.Left) <= this.cornerWidth)
            { // Bottom-Left  
                return new IntPtr((int)Win32.HitTest.HTBOTTOMLEFT);
            }
            else if (Math.Abs(this.mousePoint.Y - this.Top) <= this.cornerWidth
                && Math.Abs(this.ActualWidth + this.Left - this.mousePoint.X) <= this.cornerWidth)
            { // Top-Right  
                return new IntPtr((int)Win32.HitTest.HTTOPRIGHT);
            }
            else if (Math.Abs(this.ActualWidth + this.Left - this.mousePoint.X) <= this.cornerWidth
                && Math.Abs(this.ActualHeight + this.Top - this.mousePoint.Y) <= this.cornerWidth)
            { // Bottom-Right  
                return new IntPtr((int)Win32.HitTest.HTBOTTOMRIGHT);
            }
            else if (Math.Abs(this.mousePoint.X - this.Left) <= this.customBorderThickness)
            { // Left  
                return new IntPtr((int)Win32.HitTest.HTLEFT);
            }
            else if (Math.Abs(this.ActualWidth + this.Left - this.mousePoint.X) <= this.customBorderThickness)
            { // Right  
                return new IntPtr((int)Win32.HitTest.HTRIGHT);
            }
            else if (Math.Abs(this.mousePoint.Y - this.Top) <= this.customBorderThickness)
            { // Top  
                return new IntPtr((int)Win32.HitTest.HTTOP);
            }
            else if (Math.Abs(this.ActualHeight + this.Top - this.mousePoint.Y) <= this.customBorderThickness)
            { // Bottom  
                return new IntPtr((int)Win32.HitTest.HTBOTTOM);
            }
            else
            {
                handled = false;
                return IntPtr.Zero;
            }
        }

        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            Image maxBtnImg = this.maxImg;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            if (WindowState == WindowState.Maximized)
            {
                //this.BorderThickness = new System.Windows.Thickness(0);
                // 更换最大化按钮的图片
                bi.UriSource = new Uri("Images/Window/restore_nb.png", UriKind.Relative);
                bi.EndInit();
                maxBtnImg.Source = bi;
            }
            else
            {
                //this.BorderThickness = new System.Windows.Thickness(customBorderThickness);
                // 更换最大化按钮的图片
                bi.UriSource = new Uri("Images/Window/max_nb.png", UriKind.Relative);
                bi.EndInit();
                maxBtnImg.Source = bi;
            }
        }

        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source == null)
                // Should never be null  
                throw new Exception("Cannot get HwndSource instance.");

            source.AddHook(new HwndSourceHook(this.WndProc));
        }

        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            // MINMAXINFO structure  
            Win32.MINMAXINFO mmi = (Win32.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(Win32.MINMAXINFO));

            // Get handle for nearest monitor to this window  
            WindowInteropHelper wih = new WindowInteropHelper(this);
            IntPtr hMonitor = Win32.MonitorFromWindow(wih.Handle, Win32.MONITOR_DEFAULTTONEAREST);

            // Get monitor info  
            Win32.MONITORINFOEX monitorInfo = new Win32.MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            Win32.GetMonitorInfo(new HandleRef(this, hMonitor), monitorInfo);

            // Get HwndSource  
            HwndSource source = HwndSource.FromHwnd(wih.Handle);
            if (source == null)
                // Should never be null  
                throw new Exception("Cannot get HwndSource instance.");
            if (source.CompositionTarget == null)
                // Should never be null  
                throw new Exception("Cannot get HwndTarget instance.");

            // Get transformation matrix  
            Matrix matrix = source.CompositionTarget.TransformFromDevice;

            // Convert working area  
            Win32.RECT workingArea = monitorInfo.rcWork;
            Point dpiIndependentSize =
                matrix.Transform(new Point(
                        workingArea.Right - workingArea.Left,
                        workingArea.Bottom - workingArea.Top
                        ));

            // Convert minimum size  
            Point dpiIndenpendentTrackingSize = matrix.Transform(new Point(
                this.MinWidth,
                this.MinHeight
                ));

            // Set the maximized size of the window  
            mmi.ptMaxSize.x = (int)dpiIndependentSize.X;
            mmi.ptMaxSize.y = (int)dpiIndependentSize.Y;

            // Set the position of the maximized window  
            mmi.ptMaxPosition.x = 0;
            mmi.ptMaxPosition.y = 0;

            // Set the minimum tracking size  
            mmi.ptMinTrackSize.x = (int)dpiIndenpendentTrackingSize.X;
            mmi.ptMinTrackSize.y = (int)dpiIndenpendentTrackingSize.Y;

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private void MaxClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void MinClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InBtn(object sender, MouseEventArgs e)
        {
            StackPanel sp = e.OriginalSource as StackPanel;
            sp.Background = new SolidColorBrush(Color.FromArgb(100, 229, 229, 229));
        }

        private void OutBtn(object sender, MouseEventArgs e)
        {
            StackPanel sp = e.OriginalSource as StackPanel;
            sp.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void InClose(object sender, MouseEventArgs e)
        {
            StackPanel sp = e.OriginalSource as StackPanel;
            sp.Background = new SolidColorBrush(Color.FromArgb(255, 232, 17, 35));
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            // 获取窗体句柄 

            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;



            // 获得窗体的 样式 

            long oldstyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE);



            // 更改窗体的样式为无边框窗体 

            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE, oldstyle & ~NativeMethods.WS_CAPTION);



            // SetWindowLong(hwnd, GWL_EXSTYLE, oldstyle & ~WS_EX_LAYERED); 

            // 1 | 2 << 8 | 3 << 16  r=1,g=2,b=3 详见winuse.h文件 

            // 设置窗体为透明窗体 

            NativeMethods.SetLayeredWindowAttributes(hwnd, 1 | 2 << 8 | 3 << 16, 0, NativeMethods.LWA_ALPHA);
        }

        // 展开、关闭工具菜单
        private void ToolBarDisplay()
        {
            if (this.toolbar.Width.Value == 50)
            {
                this.toolbar.Width = new GridLength(150);
            }
            else
            {
                this.toolbar.Width = new GridLength(50);
            }
        }
        // 点击工具菜单
        private void ShowMenu(object sender, MouseEventArgs e)
        {
            ToolBarDisplay();
        }


    }
}
