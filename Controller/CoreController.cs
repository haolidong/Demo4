using Demo4.Search;
using Demo4.Util;
using SuperMap.Data;
using SuperMap.Realspace;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo4.Controller
{
    /// <summary>
    /// 控制器，调用各种业务逻辑工具
    /// </summary>
    class CoreController
    {
        private SceneControl m_sceneControl;
        private Workspace m_workspace;

        private Camera m_position;

        private Measure3D m_measure;
        private Location m_location;
        private BubblePop m_bubble;
        private Rotate m_rotate;
        private SearchAddress m_searchAddress;

        /// <summary>
        /// 构造核心控制器
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public CoreController(SceneControl sceneControl)
        {
            this.m_sceneControl = sceneControl;

            Init();
            SceneFlyIn();
        }

        /// <summary>
        /// 初始化，打开地图，创建工具
        /// </summary>
        private void Init()
        {
            // 打开地图
            Open();

            // 保存最开始的位置
            m_position = m_sceneControl.Scene.Camera;

            // 创建三维量算工具
            m_measure = new Measure3D(m_sceneControl);
            // 创建打地标工具
            m_location = new Location(m_sceneControl);
            // 创建气泡弹框工具
            m_bubble = new BubblePop(m_sceneControl);
            // 创建绕物旋转工具
            m_rotate = new Rotate(m_sceneControl);
            // 创建地址搜索工具
            m_searchAddress = new SearchAddress(m_sceneControl);
        }

        /// <summary>
        /// 打开相关资源
        /// </summary>
        private void Open()
        {
            // 打开地图
            m_workspace = new Workspace();
            WorkspaceConnectionInfo conninfo = new WorkspaceConnectionInfo();
            conninfo.Type = WorkspaceType.SMWU;
            string file =
                @"E:\super_map\SMO_DotNET_802_13626_55470_CHS_Zip\SampleData\OlympicGreen\OlympicGreen.smwu";
            conninfo.Server = file; // 这里只能是绝对路径（怎么使用相对路径呢？？？）

            m_workspace.Open(conninfo);
            m_sceneControl.Scene.Workspace = m_workspace;
            m_sceneControl.Scene.Open(m_workspace.Scenes[0]);
            m_sceneControl.Scene.Refresh();
        }
        
        /// <summary>
        /// 飞入场景动画
        /// </summary>
        public void SceneFlyIn()
        {
            //构造一个相机对象，并飞行到该相机对象
            Camera camera = new Camera(0, -60, 25000000, AltitudeMode.RelativeToGround);
            m_sceneControl.Scene.Fly(camera, 1);
            // 飞到原位
            m_sceneControl.Scene.Fly(m_position, 2000);
        }

        /// <summary>
        /// 回到原点
        /// </summary>
        public void ToOldPosition()
        {
            // 飞到原位
            m_sceneControl.Scene.Fly(m_position, 2000);
        }

        /// <summary>
        /// 量算距离
        /// </summary>
        public void MeasureDistance()
        {
            m_measure.MeasureDistance();
        }

        /// <summary>
        /// 量算面积
        /// </summary>
        public void MeasureArea()
        {
            m_measure.MeasureArea();
        }

        /// <summary>
        /// 结束量算
        /// </summary>
        public void EndMeasure()
        {
            m_measure.EndOneMeasure();
            m_measure.ClearAction();
        }

        /// <summary>
        /// 清空量算结果
        /// </summary>
        public void ClearMeasure()
        {
            m_measure.ClearMeasure();
        }

        /// <summary>
        /// 打地标
        /// </summary>
        /// <returns>当前是否允许打地标</returns>
        public bool Location()
        {
            // 先禁止弹框
            m_bubble.CloseBubble();

            return m_location.ToggleLocation();
        }

        /// <summary>
        /// 气泡弹框
        /// </summary>
        /// <returns>当前是否允许弹框</returns>
        public bool Bubble()
        {
            // 先禁止打地标
            m_location.CloseLocation();

            return m_bubble.ToogleBubble();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            m_sceneControl.Dispose();
            m_workspace.Close();
            m_workspace.Dispose();
        }

        /// <summary>
        /// 经纬网显示开关
        /// </summary>
        /// <returns>经纬网开、关状态</returns>
        public bool LatLon()
        {
            return Display.ToogleLatLon(m_sceneControl);
        }

        /// <summary>
        /// 帧率显示开关
        /// </summary>
        /// <returns>帧率开、关状态</returns>
        public bool FPS()
        {
            return Display.ToogleFPS(m_sceneControl);
        }

        /// <summary>
        /// 绕物旋转
        /// </summary>
        /// <param name="speed">旋转速度</param>
        public void Rotate(double speed)
        {
            m_rotate.RotateByObject(speed);
        }

        /// <summary>
        /// 地址搜索
        /// </summary>
        /// <param name="searchText">搜索关键字</param>
        /// <returns>搜索结果</returns>
        public ObservableCollection<Address> SearchAddress(string searchText)
        {
            return m_searchAddress.Search(searchText);
            //List<string> results = new List<string>();
            //results.Add("北京");
            //results.Add("怀柔");
            //results.Add("昌平");
            //results.Add("水立方");
            //results.Add("盘古大观");
            //return results;
        }

    } // end class CoreController

} // end namespace
