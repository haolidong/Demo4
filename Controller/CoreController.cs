using Demo4.Util;
using SuperMap.Data;
using SuperMap.Realspace;
using SuperMap.UI;
using System;
using System.Collections.Generic;
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
        private MainWindow m_window;

        private Measure3D m_measure;
        private Location m_location;
        private BubblePop m_bubble;

        /// <summary>
        /// 构造核心控制器
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        /// <param name="window">主窗体</param>
        public CoreController(SceneControl sceneControl, MainWindow window)
        {
            this.m_sceneControl = sceneControl;
            this.m_window = window;

            Init();
        }

        /// <summary>
        /// 初始化地图，即打开地图
        /// </summary>
        private void Init()
        {
            // 打开地图
            Open();

            // 创建三维量算工具
            m_measure = new Measure3D(m_sceneControl);
            // 创建打地标工具
            m_location = new Location(m_sceneControl);
            // 创建气泡弹框工具
            m_bubble = new BubblePop(m_sceneControl);
        }

        /// <summary>
        /// 打开相关资源
        /// </summary>
        private void Open()
        {
            // 打开地图
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

            // 将三维场景控件显示到主窗体上
            m_window.controlForm.Child = m_sceneControl;
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


    } // end class CoreController

} // end namespace
