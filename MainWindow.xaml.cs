﻿using SuperMap.Data;
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

namespace Demo4
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //private MapControl m_mapControl;
        private Workspace m_workspace;
        private SceneControl m_sceneControl;
        //private BubbleControl m_bubbleControl;
        private WPFBubbleControl m_wpfBubbleControl;
        private ElementHost m_bubbleControl;

        // 搜索结果
        private List<string> results = new List<string>();

        // 是否允许弹框
        private bool m_canBubble = false;
        // 是否允许加地标
        private bool m_canLocation = false;
        // 点图层
        private Layer3D m_pointLayer = null;

        public MainWindow()
        {
            InitializeComponent();

            open();

            createDataset();

            //results.Add("the first data!");

            // 搜索结果数据绑定
            this.searchResult.ItemsSource = results;

            //TestSceneFly(m_sceneControl.Scene);
        }


        private void createDataset()
        {
            // 创建点数据集
            Datasource datasource = m_workspace.Datasources[0];

            datasource.Datasets.Delete("point");
            DatasetVectorInfo pointLayerInfo = new DatasetVectorInfo("point", DatasetType.Point3D);
            DatasetVector pointDataset = datasource.Datasets.Create(pointLayerInfo);

            //设置矢量数据集在三维场景中的显示风格，并进行显示
            Layer3DSettingVector layer3DSettingVector = new Layer3DSettingVector();
            //GeoStyle3D geoStyle3D = new GeoStyle3D();
            //geoStyle3D.MarkerColor = System.Drawing.Color.Red;
            //geoStyle3D.AltitudeMode = AltitudeMode.Absolute;
            //geoStyle3D.MarkerSize = 100;
            //layer3DSettingVector.Style = geoStyle3D;

            m_pointLayer = m_sceneControl.Scene.Layers.Add(pointDataset,
                           layer3DSettingVector, true);
        }

        private void open()
        {
            m_wpfBubbleControl = new WPFBubbleControl();
            m_sceneControl = new SceneControl();
            this.mapControlForm.Child = m_sceneControl;

            m_workspace = new Workspace();
            WorkspaceConnectionInfo conninfo = new WorkspaceConnectionInfo();
            conninfo.Type = WorkspaceType.SXWU;
            string file =
                @"E:\super_map\SMO_DotNET_802_13626_55470_CHS_Zip\SampleData\Analysis3D\Analysis3D.sxwu";
            conninfo.Server = file;

            m_workspace.Open(conninfo);
            m_sceneControl.Scene.Workspace = m_workspace;
            m_sceneControl.Scene.Open(m_workspace.Scenes[0]);
            m_sceneControl.Scene.Refresh();

            // 鼠标点击事件
            m_sceneControl.MouseDown += new MouseEventHandler(m_sceneControl_MouseDown);
        }


        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_sceneControl.Dispose();
            m_workspace.Close();
            m_workspace.Dispose();
        }

        // 允许、禁止弹框
        private void bubble_Click(object sender, RoutedEventArgs e)
        {
            // 先禁止加地标
            if (m_canLocation)
            {
                location_Click(null, null);
            }

            m_canBubble = !m_canBubble;
            if (m_canBubble)
            {
                this.bubble.Content = "禁止弹框";
            }
            else
            {
                this.bubble.Content = "允许弹框";
            }
        }

        private void bubbleShow(Point3D p3)
        {

            m_sceneControl.Bubbles.Clear();
            m_wpfBubbleControl = new WPFBubbleControl();

            m_bubbleControl = new ElementHost();
            m_bubbleControl.Width = 300;
            m_bubbleControl.Height = 300;
            m_bubbleControl.Child = m_wpfBubbleControl;
            m_sceneControl.Controls.Add(m_bubbleControl);


            Bubble bubble = new Bubble();
            m_sceneControl.Bubbles.Add(bubble);

            //bubble.Title = "这是一个气泡";
            bubble.BackColor = System.Drawing.Color.FromArgb(200, 255, 255, 255);
            bubble.RoundQuality = 50;
            bubble.ClientWidth = m_bubbleControl.Width;
            bubble.ClientHeight = m_bubbleControl.Height;
            bubble.Pointer = p3;
            m_bubbleControl.Location =
                new System.Drawing.Point(bubble.ClientLeft, bubble.ClientTop);
            m_sceneControl.Scene.Refresh();

            // 注册气泡初始化的事件
            m_sceneControl.BubbleInitialize +=
                new BubbleInitializeEventHandler(m_sceneControl_BubbleInitialize);

            // 注册气泡位置变化的事件
            m_sceneControl.BubbleResize +=
                new BubbleResizeEventHandler(m_sceneControl_BubbleResize);

            // 注册关闭气泡的事件
            m_sceneControl.BubbleClose +=
                new BubbleCloseEventHandler(m_sceneControl_BubbleClose);

            //Point3D p = bubble.Pointer;
            //System.Windows.MessageBox.Show("x:" + p.X + " y:" + p.Y + " z:" + p.Z);
        }

        void m_sceneControl_BubbleClose(object sender, BubbleEventArgs e)
        {
            //关闭气泡，将气泡控件设置为不可见
            m_bubbleControl.Visible = false;
        }

        void m_sceneControl_BubbleResize(object sender, BubbleEventArgs e)
        {
            // 定义气泡控件的位置
            System.Drawing.Point point =
                new System.Drawing.Point(e.Bubble.ClientLeft, e.Bubble.ClientTop);
            m_bubbleControl.Location = point;
            // 将气泡控件设置为可见
            m_bubbleControl.Visible = true;

        }
        void m_sceneControl_BubbleInitialize(object sender, BubbleEventArgs e)
        {
            // 定义气泡控件的位置
            System.Drawing.Point point =
                new System.Drawing.Point(e.Bubble.ClientLeft, e.Bubble.ClientTop);
            m_bubbleControl.Location = point;
            // 将气泡控件设置为可见
            m_bubbleControl.Visible = true;
        }

        // 点击弹出气泡框
        void m_sceneControl_MouseDown(object sender, MouseEventArgs e)
        {
            // 将屏幕点转换为三维点对象，
            // 指定求交类型为：地形和模型都参与求交点，返回距离视点最近的交点。
            Point3D p3 = m_sceneControl.Scene.PixelToGlobe(new System.Drawing.Point(e.X, e.Y),
                PixelToGlobeMode.TerrainAndModel);

            if (m_canBubble)
            {
                bubbleShow(p3);
            }
            else if (m_canLocation)
            {
                locationShow(p3);
            }
        }

        // 点击加地标按钮
        private void location_Click(object sender, RoutedEventArgs e)
        {
            // 先禁止弹框
            if (m_canBubble)
            {
                bubble_Click(null, null);
            }

            m_canLocation = !m_canLocation;
            if (m_canLocation)
            {
                this.location.Content = "禁止加地标";
            }
            else
            {
                this.location.Content = "允许加地标";
            }
        }

        private void locationShow(Point3D p3)
        {
            DatasetVector vector =
                m_workspace.Datasources[0].Datasets["point"] as DatasetVector;

            // P3中的Z值为0，用GetHeight()来获得Z值
            GeoPoint3D gp3 = new GeoPoint3D(p3.X, p3.Y,
                m_sceneControl.Scene.GetHeight(p3.X, p3.Y));

            Recordset recordset = vector.GetRecordset(true, CursorType.Dynamic);
            recordset.AddNew(gp3);
            recordset.Update();


            //设置矢量数据集在三维场景中的显示风格，并进行显示
            Layer3DSettingVector layer3DSettingVector = new Layer3DSettingVector();
            GeoStyle3D geoStyle3D = new GeoStyle3D();
            geoStyle3D.MarkerColor = System.Drawing.Color.Red;
            geoStyle3D.AltitudeMode = AltitudeMode.Absolute;
            //geoStyle3D.MarkerSize = 100;
            geoStyle3D.MarkerFile = @"E:\super_map\location2.png";
            geoStyle3D.MarkerAnchorPoint = new Point2D(0.5, 0);
            layer3DSettingVector.Style = geoStyle3D;

            Layer3DDataset layer3DDatasetPoint =
                m_sceneControl.Scene.Layers.Add(vector, layer3DSettingVector, true);
            layer3DDatasetPoint.UpdateData();



            //m_sceneControl.Scene.Layers.Add(vector, new Layer3DSettingVector(), true);
            m_sceneControl.Scene.Refresh();
            recordset.Dispose();
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            // 未输入任何查询条件，不进行查询
            if (this.searchText.Text.Equals("") || this.searchText.Text == null)
            {
                System.Windows.MessageBox.Show("请输入查询条件");
                return;
            }

            results.Clear();

            // 查询
            DatasetVector dv = null;
            QueryParameter qp = null;

            for (int i = 0; i < m_workspace.Datasources[0].Datasets.Count; ++i)
            {
                dv = m_workspace.Datasources[0].Datasets[i] as DatasetVector;

                if (dv == null) continue;

                qp = new QueryParameter();
                qp.HasGeometry = true;
                qp.CursorType = CursorType.Static;

                Recordset recordset = dv.Query(qp);

                FieldInfos infos = recordset.GetFieldInfos();

                while (!recordset.IsEOF)
                {
                    if (!Contains(infos, "NAME"))
                    {
                        recordset.MoveNext();
                        continue;
                    }
                    string name = recordset.GetFieldValue("NAME").ToString();
                    if (name.Contains(this.searchText.Text))
                    {
                        results.Add("Name : " + name);
                    }
                    recordset.MoveNext();
                }

                recordset.Dispose();
            }

            this.searchResult.ItemsSource = null;
            this.searchResult.ItemsSource = results;
            this.searchText.Text = "";
        }


        private bool Contains(FieldInfos fis, string name)
        {
            foreach (FieldInfo fi in fis)
            {
                if (fi.Name.Equals(name)) return true;
            }
            return false;
        }


        public void TestSceneFly(Scene sceneObject)
        {
            //构造一个相机对象，并飞行到该相机对象
            Camera camera = new Camera(102, 31, 8000000, AltitudeMode.RelativeToGround);
            sceneObject.Fly(camera);
        }


    }
}