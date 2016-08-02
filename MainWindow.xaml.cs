using Demo4.Addition;
using Demo4.Animation;
using Demo4.Controller;
using Demo4.Search;
using Demo4.WindowSetting;
using SuperMap.Data;
using SuperMap.Realspace;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Demo4
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CoreController m_controller;
        private SceneControl m_sceneControl;

        // 当前选中的TabTitle
        private Panel m_currentTabTitle;
        private Panel m_currentTabContent;
        // 当前显示的TabContent
        private Panel m_anotherTabTitle;
        private Panel m_anotherTabContent;

        private ObservableCollection<Address> results;

        /// <summary>
        /// 窗体风格设置类，将MainWindow作参数传进去，进而设置窗体风格
        /// 现在暂时不用。。。
        /// </summary>
        //private Demo4.WindowSetting.WindowStyle m_windowStyle;

        public MainWindow()
        {
            InitializeComponent();

            // 窗体风格控制相关（代码实现在MainWindowStyle.cs中）
            this.SourceInitialized += MainWindow_SourceInitialized;
            this.StateChanged += MainWindow_StateChanged;
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            //m_windowStyle = new WindowSetting.WindowStyle(this);

            // 搜索结果绑定
            results = new ObservableCollection<Address>();
            this.searchResult.ItemsSource = results;

            // 地图显示控件
            m_sceneControl = new SceneControl();
            this.controlForm.Child = m_sceneControl;

            m_controller = new CoreController(m_sceneControl);

            // 图层Tab下的工作空间树
            WorkspaceTree wt = new WorkspaceTree();
            wt.Workspace = m_sceneControl.Scene.Workspace;
            this.layer.Child = wt;

            // 当前选中的Tab
            m_currentTabTitle = this.searchTabTitle;
            m_currentTabContent = this.searchTabContent;
            // 当前未选中的Tab
            m_anotherTabTitle = this.layerTabTitle;
            m_anotherTabContent = this.layerTabContent;
        }

        /// <summary>
        /// 关闭窗体，释放相关资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_controller.Close();
        }

        /// <summary>
        /// 点击展开、关闭工具菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMenu(object sender, MouseEventArgs e)
        {
            //double width = this.toolbar.Width.Value == 50 ? 150 : 50;
            //this.toolbar.Width = new GridLength(width);

            if (Grid.GetColumnSpan(this.toolbar) == 1)
            {
                Grid.SetColumnSpan(this.toolbar, 2);
            }
            else
            {
                Grid.SetColumnSpan(this.toolbar, 1);
            }
        }

        /// <summary>
        /// 点击展开三维量算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeasureClick(object sender, MouseEventArgs e)
        {
            ToolClick(sender, e);

            StackPanel sp = sender as StackPanel;

            // 展开动画、收起动画
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            
            // 更换展开图标
            Image expandImg = sp.Children[2] as Image;  
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            if (Selectable.GetSelectStatus(sp))
            {
                // 更改图片、设置动画的起、止值
                bi.UriSource = new Uri("Images/Tool/expanded.png", UriKind.Relative);
                // 展开动画
                da.From = 0;
                da.To = this.measureTool.ActualHeight;
            }
            else
            {
                bi.UriSource = new Uri("Images/Tool/expand.png", UriKind.Relative);
                // 收起动画
                da.From = this.measureTool.ActualHeight;
                da.To = 0;
            }
            bi.EndInit();
            expandImg.Source = bi;
            this.measureTool.BeginAnimation(HeightProperty, da); // 执行动画
        }

        /// <summary>
        /// 量算选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeasureSelect(object sender, MouseEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            int select = IndexOfParent(sp); // 选择的量算工具的索引
            switch (select)
            {
                case 0:     // 距离量算
                    MeasureTool(this.measureArea, false);
                    MeasureTool(sp, true);
                    m_controller.MeasureDistance();
                    break;
                case 1:     // 面积量算
                    MeasureTool(this.measureDistance, false);
                    MeasureTool(sp, true);
                    m_controller.MeasureArea();
                    break;
                case 2:     // 结束量算
                    MeasureTool(this.measureDistance, false);
                    MeasureTool(this.measureArea, false);
                    m_controller.EndMeasure();
                    break;
                case 3:     // 清空量算结果
                    MeasureTool(this.measureDistance, false);
                    MeasureTool(this.measureArea, false);
                    m_controller.ClearMeasure();
                    break;
                default:
                    MessageBox.Show("Error!!!");
                    break;
            }

        }
        /// <summary>
        /// 将控件的选中状态设置为指定值
        /// </summary>
        /// <param name="sp">要设置的控件</param>
        /// <param name="select">要设置的选中状态</param>
        private void MeasureTool(StackPanel sp, bool select)
        {
            if (select)
            {
                if (!Selectable.GetSelectStatus(sp))
                {
                    ToolClick(sp, null);
                }
            }
            else
            {
                if (Selectable.GetSelectStatus(sp))
                {
                    ToolClick(sp, null);
                }
            }
        }
        /// <summary>
        /// 查找控件在父元素中的索引
        /// </summary>
        /// <param name="control">待查找的控件</param>
        /// <returns>控件在父元素中的索引，返回-1时代表查找失败</returns>
        private int IndexOfParent(FrameworkElement control)
        {
            FrameworkElement parent = control.Parent as FrameworkElement;
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
                {
                    if (VisualTreeHelper.GetChild(parent, i) == control)
                        return i;
                }
            }
            return -1; 
        }

        /// <summary>
        /// 点击弹框按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BubbleShow(object sender, MouseEventArgs e)
        {
            ToolClick(sender, e);
            m_controller.Bubble();
        }

        /// <summary>
        /// 点击加地标按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocationShow(object sender, MouseEventArgs e)
        {
            ToolClick(sender, e);
            m_controller.Location();
        }

        /// <summary>
        /// 地址搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchAddress(object sender, MouseEventArgs e)
        {
            this.searchResult.ItemsSource = null;
            this.searchResult.ItemsSource = m_controller.SearchAddress(this.searchText.Text);
            this.searchText.Text = "";
        }

        /// <summary>
        /// 绕物旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rotate(object sender, MouseEventArgs e)
        {
            m_controller.Rotate(3);
        }

        /// <summary>
        /// 显示、隐藏经纬网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LatlonShow(object sender, MouseEventArgs e)
        {
            ToolClick(sender, e);
            m_controller.LatLon();
        }

        /// <summary>
        /// 显示、隐藏帧率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FPSShow(object sender, MouseEventArgs e)
        {
            ToolClick(sender, e);
            m_controller.FPS();
        }

        /// <summary>
        /// 我迷路了，返回原点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IMLost(object sender, MouseEventArgs e)
        {
            m_controller.ToOldPosition();
        }


        /// <summary>
        /// 点击可选中的工具栏，改变控件的选中状态（即Selectable.SelectProperty）
        /// 并根据改变后的选中状态，显示相应的背景色来突出其状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolClick(object sender, MouseEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            // toogle选中状态
            bool select = !Selectable.GetSelectStatus(sp);
            Selectable.SetSelectStatus(sp, select);
            // 根据选中状态设置不同背景色
            Color c = select ? Color.FromArgb(100, 229, 229, 229) : Color.FromArgb(0, 0, 0, 0);
            sp.Background = new SolidColorBrush(c);
        }

        /// <summary>
        /// 鼠标进入按钮，改变背景色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InBtn(object sender, MouseEventArgs e)
        {
            Panel p = sender as Panel;

            // 若是已被选中，不改变其背景色，直接退出
            if (Selectable.GetSelectStatus(p))
            {
                return;
            }

            SolidColorBrush scb = p.Background as SolidColorBrush;

            // 颜色改变动画
            ColorAnimation ca = new ColorAnimation();
            ca.From = scb.Color;
            ca.To = Color.FromArgb(100, 229, 229, 229);
            ca.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));

            scb.BeginAnimation(SolidColorBrush.ColorProperty, ca);
        }

        /// <summary>
        /// 鼠标移出按钮，改变背景色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutBtn(object sender, MouseEventArgs e)
        {
            Panel p = sender as Panel;

            // 若是已被选中，不改变其背景色，直接退出
            if (Selectable.GetSelectStatus(p))
            {
                return;
            }

            SolidColorBrush scb = p.Background as SolidColorBrush;

            // 颜色改变动画
            ColorAnimation ca = new ColorAnimation();
            ca.From = scb.Color;
            ca.To = Color.FromArgb(0, 0, 0, 0);
            ca.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));

            scb.BeginAnimation(SolidColorBrush.ColorProperty, ca);
        }

        /// <summary>
        /// 切换Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchTab(object sender, MouseButtonEventArgs e)
        {
            // 如果点击当前选中Tab，直接退出
            if (m_currentTabTitle == sender)
            {
                return;
            }

            // 交换选中与未选中的Tab
            Panel temp = m_currentTabTitle;
            m_currentTabTitle = m_anotherTabTitle;
            m_anotherTabTitle = temp;
            temp = m_currentTabContent;
            m_currentTabContent = m_anotherTabContent;
            m_anotherTabContent = temp;

            // 切换Tab后配置不同外观
            TabSetting();
        }

        /// <summary>
        /// 切换Tab后，配置不同Tab的显示外观
        /// </summary>
        private void TabSetting()
        {
            // 更换TabTitle背景色
            m_currentTabTitle.Background =
                new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            m_anotherTabTitle.Background =
                new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            // 更换TabTitle文字颜色
            ((m_anotherTabTitle.Children[0] as Panel).Children[1] as TextBlock).Foreground =
                new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            ((m_currentTabTitle.Children[0] as Panel).Children[1] as TextBlock).Foreground =
                new SolidColorBrush(Color.FromArgb(255, 5, 147, 211));

            // 更换TabTitle的图片
            Image anotherTabImg = (m_anotherTabTitle.Children[0] as Panel).Children[0] as Image;
            Image currentTabImg = (m_currentTabTitle.Children[0] as Panel).Children[0] as Image;
            if (m_currentTabTitle == this.searchTabTitle)
            {
                currentTabImg.Source = CreateBitmapImage("Images/RightSide/search_20_b.png");
                anotherTabImg.Source = CreateBitmapImage("Images/RightSide/layer_20.png");
            }
            else
            {
                currentTabImg.Source = CreateBitmapImage("Images/RightSide/layer_20_b.png");
                anotherTabImg.Source = CreateBitmapImage("Images/RightSide/search_20.png");
            }

            // 显示选中的TabContent
            m_anotherTabContent.Visibility = System.Windows.Visibility.Hidden;
            m_currentTabContent.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// 根据相对路径，生成图片对象
        /// </summary>
        /// <param name="realPath">图片相对路径</param>
        /// <returns>生成的BitmapImage对象</returns>
        private BitmapImage CreateBitmapImage(string realPath)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(realPath, UriKind.Relative);
            bi.EndInit();

            return bi;
        }

    }
}
