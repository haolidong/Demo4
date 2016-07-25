using SuperMap.Data;
using SuperMap.Realspace;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Demo4.Util
{
    /// <summary>
    /// 气泡弹框工具类
    /// </summary>
    class BubblePop
    {
        private SceneControl m_sceneControl;

        private WPFBubbleControl m_wpfBubbleControl;
        private ElementHost m_bubbleControl;

        private bool m_canBubble = false;

        /// <summary>
        /// 构造气泡弹框工具类
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public BubblePop(SceneControl sceneControl)
        {
            this.m_sceneControl = sceneControl;

            // 鼠标点击事件
            m_sceneControl.MouseDown += new MouseEventHandler(m_sceneControl_MouseDown);
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_sceneControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_canBubble)
            {
                // 将屏幕点转换为三维点对象，
                // 指定求交类型为：地形和模型都参与求交点，返回距离视点最近的交点。
                Point3D p3 = m_sceneControl.Scene.PixelToGlobe(new System.Drawing.Point(e.X, e.Y),
                    PixelToGlobeMode.TerrainAndModel);

                BubbleShow(p3);
            }
        }

        /// <summary>
        /// 气泡弹框开关
        /// </summary>
        /// <returns>当前是否允许弹框</returns>
        public bool ToogleBubble()
        {
            return m_canBubble = !m_canBubble;
        }

        /// <summary>
        /// 禁止弹框
        /// </summary>
        public void CloseBubble()
        {
            m_canBubble = false;
        }

        /// <summary>
        /// 弹出气泡框
        /// </summary>
        /// <param name="p3">气泡框指向的位置</param>
        public void BubbleShow(Point3D p3)
        {
            m_sceneControl.Bubbles.Clear();
            m_wpfBubbleControl = new WPFBubbleControl();

            Point3D np3 = new Point3D(p3.X, p3.Y, m_sceneControl.Scene.GetHeight(p3.X, p3.Y));
            m_wpfBubbleControl.x_position.Text = ((float)(np3.X)).ToString();
            m_wpfBubbleControl.y_position.Text = ((float)(np3.Y)).ToString();
            m_wpfBubbleControl.z_position.Text = ((float)(np3.Z)).ToString();

            m_bubbleControl = new ElementHost();
            m_bubbleControl.Width = 200;
            m_bubbleControl.Height = 200;
            m_bubbleControl.Child = m_wpfBubbleControl;
            m_sceneControl.Controls.Add(m_bubbleControl);

            Bubble bubble = new Bubble();
            m_sceneControl.Bubbles.Add(bubble);

            //气泡相关设置
            bubble.BackColor = System.Drawing.Color.FromArgb(200, 255, 255, 255);
            bubble.RoundQuality = 0;
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

        }

        /// <summary>
        /// 关闭气泡的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_sceneControl_BubbleClose(object sender, BubbleEventArgs e)
        {
            //关闭气泡，将气泡控件设置为不可见
            m_bubbleControl.Visible = false;
        }

        /// <summary>
        /// 气泡位置变化的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_sceneControl_BubbleResize(object sender, BubbleEventArgs e)
        {
            // 定义气泡控件的位置
            System.Drawing.Point point =
                new System.Drawing.Point(e.Bubble.ClientLeft, e.Bubble.ClientTop);
            m_bubbleControl.Location = point;
            // 将气泡控件设置为可见
            m_bubbleControl.Visible = true;

        }

        /// <summary>
        /// 气泡初始化的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_sceneControl_BubbleInitialize(object sender, BubbleEventArgs e)
        {
            // 定义气泡控件的位置
            System.Drawing.Point point =
                new System.Drawing.Point(e.Bubble.ClientLeft, e.Bubble.ClientTop);
            m_bubbleControl.Location = point;
            // 将气泡控件设置为可见
            m_bubbleControl.Visible = true;
        }

    }
}
