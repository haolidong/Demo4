using SuperMap.Data;
using SuperMap.Mapping;
using SuperMap.Realspace;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using System.Collections;
using Demo4.Util;
using Demo4.Controller;

namespace Demo4
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CoreController m_controller;
        private SceneControl m_sceneControl;

        public MainWindow()
        {
            InitializeComponent();

            m_sceneControl = new SceneControl();
            this.controlForm.Child = m_sceneControl;

            m_controller = new CoreController(m_sceneControl);
        }

        // 关闭窗体
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_controller.Close();
        }

        // 允许、禁止弹框
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

        // search address
        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            this.searchResult.ItemsSource = null;
            this.searchResult.ItemsSource = m_controller.SearchAddress(this.searchText.Text);
            this.searchText.Text = "";
        }


        // 飞行 
        public void TestSceneFly(Scene sceneObject)
        {
            Camera old = sceneObject.Camera;

            //构造一个相机对象，并飞行到该相机对象
            Camera camera = new Camera(0, -60, 25000000, AltitudeMode.RelativeToGround);
            sceneObject.Fly(camera, 1);

            sceneObject.Fly(old, 2000);
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

    }
}
