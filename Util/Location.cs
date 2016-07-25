using SuperMap.Data;
using SuperMap.Realspace;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo4.Util
{
    /// <summary>
    /// 打地标工具类
    /// </summary>
    class Location
    {
        private SceneControl m_sceneControl;
        private Workspace m_workspace;

        // 是否打地标的控制开关
        private bool m_canLocation = false;
        //public bool CanLocation
        //{
        //    get
        //    {
        //        return m_canLocation;
        //    }
        //}

        /// <summary>
        /// 构造打地标工具类
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public Location(SceneControl sceneControl)
        {
            this.m_sceneControl = sceneControl;
            this.m_workspace = sceneControl.Scene.Workspace;

            // 创建数据集
            CreatePointDataset();

            // 鼠标点击事件
            m_sceneControl.MouseDown += new MouseEventHandler(m_sceneControl_MouseDown);
        }

        /// <summary>
        /// 创建点数据集，为打地标准备
        /// </summary>
        private void CreatePointDataset()
        {
            // 创建点数据集
            Datasource datasource = m_workspace.Datasources[0];

            datasource.Datasets.Delete("point");
            DatasetVectorInfo pointLayerInfo = new DatasetVectorInfo("point", DatasetType.Point3D);
            DatasetVector pointDataset = datasource.Datasets.Create(pointLayerInfo);

            //设置矢量数据集在三维场景中的显示风格，并进行显示
            Layer3DSettingVector layer3DSettingVector = new Layer3DSettingVector();

            m_sceneControl.Scene.Layers.Add(pointDataset, layer3DSettingVector, true);
        }

        /// <summary>
        /// 打地标开关
        /// </summary>
        /// <returns>当前是否允许打地标</returns>
        public bool ToggleLocation()
        {
            return m_canLocation = !m_canLocation;
        }

        /// <summary>
        /// 禁止打地标
        /// </summary>
        public void CloseLocation()
        {
            m_canLocation = false;
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_sceneControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_canLocation)
            {
                // 将屏幕点转换为三维点对象，
                // 指定求交类型为：地形和模型都参与求交点，返回距离视点最近的交点。
                Point3D p3 = m_sceneControl.Scene.PixelToGlobe(new System.Drawing.Point(e.X, e.Y),
                    PixelToGlobeMode.TerrainAndModel);

                LocationShow(p3);
            }
        }

        /// <summary>
        /// 加地标
        /// </summary>
        /// <param name="p3">地标位置</param>
        public void LocationShow(Point3D p3)
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
            geoStyle3D.MarkerFile = @"E:\super_map\location2.png";
            geoStyle3D.MarkerAnchorPoint = new Point2D(0.5, 0);
            layer3DSettingVector.Style = geoStyle3D;

            Layer3DDataset layer3DDatasetPoint =
                m_sceneControl.Scene.Layers.Add(vector, layer3DSettingVector, true);
            layer3DDatasetPoint.UpdateData();

            m_sceneControl.Scene.Refresh();
            recordset.Dispose();
        }

    }
}
