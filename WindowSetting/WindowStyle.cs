using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Demo4.WindowSetting
{
    /// <summary>
    /// 窗体风格设置
    /// 无边框、可拖动等
    /// 代码来自网络
    /// </summary>
    class WindowStyle
    {
        private MainWindow m_window;

        /// <summary>  
        /// Mouse point used by HitTest  
        /// </summary>  
        private Point mousePoint = new Point();

        /// <summary>  
        /// Corner width used in HitTest  
        /// </summary>  
        private readonly int cornerWidth = 6;

        private readonly int customBorderThickness = 5;

        public WindowStyle(MainWindow window)
        {
            m_window = window;

            // 窗体风格控制相关
            m_window.SourceInitialized += MainWindow_SourceInitialized;
            m_window.StateChanged += MainWindow_StateChanged;
            m_window.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            m_window.Loaded += m_window_Loaded;

            // 窗体右上角三个操作按钮的按下操作
            m_window.minBtn.MouseLeftButtonUp += MinClick;
            m_window.maxBtn.MouseLeftButtonUp += MaxClick;
            m_window.closeBtn.MouseLeftButtonUp += CloseClick;
        }



        /************窗体风格设置(以下大部分代码来自网络)---无边框，鼠标拖动，最大、最小化，关闭************/

        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource.Equals(m_window.titleBar))
            {
                WindowInteropHelper wih = new WindowInteropHelper(m_window);
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
            if (m_window.WindowState == WindowState.Maximized)
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
            if (Math.Abs(this.mousePoint.Y - m_window.Top) <= this.cornerWidth
                && Math.Abs(this.mousePoint.X - m_window.Left) <= this.cornerWidth)
            { // Top-Left  
                return new IntPtr((int)Win32.HitTest.HTTOPLEFT);
            }
            else if (Math.Abs(m_window.ActualHeight + m_window.Top - this.mousePoint.Y) <= this.cornerWidth
                && Math.Abs(this.mousePoint.X - m_window.Left) <= this.cornerWidth)
            { // Bottom-Left  
                return new IntPtr((int)Win32.HitTest.HTBOTTOMLEFT);
            }
            else if (Math.Abs(this.mousePoint.Y - m_window.Top) <= this.cornerWidth
                && Math.Abs(m_window.ActualWidth + m_window.Left - this.mousePoint.X) <= this.cornerWidth)
            { // Top-Right  
                return new IntPtr((int)Win32.HitTest.HTTOPRIGHT);
            }
            else if (Math.Abs(m_window.ActualWidth + m_window.Left - this.mousePoint.X) <= this.cornerWidth
                && Math.Abs(m_window.ActualHeight + m_window.Top - this.mousePoint.Y) <= this.cornerWidth)
            { // Bottom-Right  
                return new IntPtr((int)Win32.HitTest.HTBOTTOMRIGHT);
            }
            else if (Math.Abs(this.mousePoint.X - m_window.Left) <= this.customBorderThickness)
            { // Left  
                return new IntPtr((int)Win32.HitTest.HTLEFT);
            }
            else if (Math.Abs(m_window.ActualWidth + m_window.Left - this.mousePoint.X) <= this.customBorderThickness)
            { // Right  
                return new IntPtr((int)Win32.HitTest.HTRIGHT);
            }
            else if (Math.Abs(this.mousePoint.Y - m_window.Top) <= this.customBorderThickness)
            { // Top  
                return new IntPtr((int)Win32.HitTest.HTTOP);
            }
            else if (Math.Abs(m_window.ActualHeight + m_window.Top - this.mousePoint.Y) <= this.customBorderThickness)
            { // Bottom  
                return new IntPtr((int)Win32.HitTest.HTBOTTOM);
            }
            else
            {
                handled = false;
                return IntPtr.Zero;
            }
        }

        // 当窗口状态改变时，更换最大化按钮的图片
        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            Image maxBtnImg = m_window.maxImg;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            // 更改最大化按钮的图片
            bi.UriSource = m_window.WindowState == WindowState.Maximized ?
                           new Uri("Images/Window/restore_nb.png", UriKind.Relative) :
                           new Uri("Images/Window/max_nb.png", UriKind.Relative);

            #region
            //if (WindowState == WindowState.Maximized)
            //{
            //    this.BorderThickness = new System.Windows.Thickness(0);
            //}
            //else
            //{
            //    this.BorderThickness = new System.Windows.Thickness(customBorderThickness);
            //}
            #endregion

            bi.EndInit();
            maxBtnImg.Source = bi;
        }

        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(m_window).Handle);
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
            WindowInteropHelper wih = new WindowInteropHelper(m_window);
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
                m_window.MinWidth,
                m_window.MinHeight
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

        // 点击最大化
        private void MaxClick(object sender, MouseButtonEventArgs e)
        {
            if (m_window.WindowState == WindowState.Maximized)
                m_window.WindowState = WindowState.Normal;
            else
                m_window.WindowState = WindowState.Maximized;
        }

        // 点击最小化
        private void MinClick(object sender, MouseButtonEventArgs e)
        {
            m_window.WindowState = System.Windows.WindowState.Minimized;
        }

        // 点击关闭
        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            m_window.Close();
        }

        private void m_window_Loaded(object sender, RoutedEventArgs e)
        {
            // 获取窗体句柄 
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(m_window).Handle;

            // 获得窗体的 样式 
            long oldstyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE);

            // 更改窗体的样式为无边框窗体 
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE, oldstyle & ~NativeMethods.WS_CAPTION);

            // SetWindowLong(hwnd, GWL_EXSTYLE, oldstyle & ~WS_EX_LAYERED); 

            // 1 | 2 << 8 | 3 << 16  r=1,g=2,b=3 详见winuse.h文件 

            // 设置窗体为透明窗体 
            NativeMethods.SetLayeredWindowAttributes(hwnd, 1 | 2 << 8 | 3 << 16, 0, NativeMethods.LWA_ALPHA);
        }


    }
}
