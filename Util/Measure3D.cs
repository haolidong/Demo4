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
    /// 三维量算，包括距离量算和面积量算
    /// </summary>
    class Measure3D
    {
        // 三维场景控件
        private SceneControl m_sceneControl;

        // 量算点
        private Point3Ds m_point3Ds = new Point3Ds();
        // 量算过程中鼠标位置点
        private Point3D m_temPoint = Point3D.Empty;

        /// <summary>
        /// 构造三维量算工具类
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public Measure3D(SceneControl sceneControl)
        {
            this.m_sceneControl = sceneControl;

            // 正在进行量算
            m_sceneControl.Tracking += new Tracking3DEventHandler(TrackingHandler);
            // 一次量算完成
            m_sceneControl.Tracked += new Tracked3DEventHandler(TrackedHandler);
            // 鼠标松开
            m_sceneControl.MouseUp += new MouseEventHandler(m_sceneControl_MouseUp);
        }

        /// <summary>
        /// 开始量算距离
        /// </summary>
        public void MeasureDistance()
        {
            m_sceneControl.Action = Action3D.MeasureDistance;
        }

        /// <summary>
        /// 开始量算面积
        /// </summary>
        public void MeasureArea()
        {
            m_sceneControl.Action = Action3D.MeasureArea;
        }

        /// <summary>
        /// 结束量算
        /// </summary>
        public void EndOneMeasure()
        {
            m_point3Ds.Clear();
            m_temPoint = Point3D.Empty;
        }

        /// <summary>
        /// 清空量算结果
        /// </summary>
        public void ClearMeasure()
        {
            EndOneMeasure();
            m_sceneControl.Scene.TrackingLayer.Clear();
            ClearAction();
        }

        /// <summary>
        /// 恢复鼠标状态为平移状态
        /// </summary>
        public void ClearAction()
        {
            m_sceneControl.Action = Action3D.Pan;
        }

        /// <summary>
        /// 鼠标松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_sceneControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                EndOneMeasure();
            }
            else
            {
                Point3D p3 = m_sceneControl.Scene.PixelToGlobe(new System.Drawing.Point(e.X, e.Y),
                                PixelToGlobeMode.TerrainAndModel);
                m_point3Ds.Add(p3);

                // 存储最后点击的点的坐标
                //lastPoint = p3;
            }
        }

        /// <summary>
        /// 正在进行量算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackingHandler(object sender, Tracking3DEventArgs e)
        {
            // 跟踪鼠标当前位置
            m_temPoint = new Point3D(e.X, e.Y, e.Z);

            if (m_sceneControl.Action == Action3D.MeasureDistance)  // 距离量算
            {
                // 先删除原来显示“当前长度”的文字
                int i = m_sceneControl.Scene.TrackingLayer.IndexOf("curL");
                if (i >= 0)
                {
                    m_sceneControl.Scene.TrackingLayer.Remove(i);
                }

                double curL = e.CurrentLength;
                //this.currentResult.Text = "当前线段长" + curL + "米";

                if (m_point3Ds.Count > 0)
                {
                    Point3Ds p3s = new Point3Ds();
                    p3s.Add(m_point3Ds[m_point3Ds.Count - 1]);
                    p3s.Add(m_temPoint);
                    GeoLine3D gl = new GeoLine3D(p3s);

                    GeoText3D text3D = CreateText3DMessage("当前线段长" + curL + "米", gl.BoundingBox.Center);
                    m_sceneControl.Scene.TrackingLayer.Add(text3D, "curL");
                }

            }
            else  // 面积量算
            {
                // 先删除原来显示的“当前面积”
                int i = m_sceneControl.Scene.TrackingLayer.IndexOf("curA");
                if (i >= 0)
                {
                    m_sceneControl.Scene.TrackingLayer.Remove(i);
                }

                double curA = e.TotalArea;
                //this.currentResult.Text = "当前面积" + curA + "平方米";

                if (m_point3Ds.Count > 1)
                {
                    Point3Ds p3s = m_point3Ds.Clone();
                    p3s.Add(m_temPoint);
                    GeoRegion3D gr = new GeoRegion3D(p3s);

                    GeoText3D text3D = CreateText3DMessage("当前面积" + curA + "平方米", gr.BoundingBox.Center);
                    m_sceneControl.Scene.TrackingLayer.Add(text3D, "curA");
                }
            }
        }

        /// <summary>
        /// 完成量算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackedHandler(object sender, Tracked3DEventArgs e)
        {
            if (m_sceneControl.Action == Action3D.MeasureDistance)
            {
                #region
                if (m_point3Ds.Count > 1)
                {
                    // 绘制量算线
                    GeoStyle3D gl3d = GeoLine3DStyle();
                    //GeoLine3D gl = new GeoLine3D(m_point3Ds);
                    GeoLine3D gl = e.Geometry as GeoLine3D;
                    gl.Style3D = gl3d;
                    m_sceneControl.Scene.TrackingLayer.Add(gl, "measureLine");

                    // 绘制量算点
                    GeoStyle3D gp3d = GeoPoint3DStyle();
                    for (int i = 0; i < gl.PartCount; ++i)
                    {
                        for (int j = 0; j < gl[i].Count; ++j)
                        {
                            GeoPoint3D gp = new GeoPoint3D(gl[i][j]);
                            gp.Style3D = gp3d;
                            m_sceneControl.Scene.TrackingLayer.Add(gp, "measurePoint");
                        }
                    }

                    // 删除显示“当前长度”的文字
                    int k = m_sceneControl.Scene.TrackingLayer.IndexOf("curL");
                    if (k >= 0)
                    {
                        m_sceneControl.Scene.TrackingLayer.Remove(k);
                    }

                    double totL = e.Length;
                    //this.totalResult.Text = "总长" + totL + "米";
                    m_sceneControl.Scene.TrackingLayer.Add(
                        CreateText3DMessage("总长" + totL + "米", gl.BoundingBox.Center), "totL");
                }
                #endregion
            }
            else
            {
                if (m_point3Ds.Count > 2)
                {
                    // 绘制量算面
                    GeoRegion3D gr = e.Geometry as GeoRegion3D;
                    GeoStyle3D gs3d = GeoRegion3DStyle();
                    gr.Style3D = gs3d;
                    m_sceneControl.Scene.TrackingLayer.Add(gr, "measureArea");

                    // 绘制量算点
                    GeoStyle3D gp3d = GeoPoint3DStyle();
                    for (int i = 0; i < gr.PartCount; ++i)
                    {
                        for (int j = 0; j < gr[i].Count; ++j)
                        {
                            GeoPoint3D gp = new GeoPoint3D(gr[i][j]);
                            gp.Style3D = gp3d;
                            m_sceneControl.Scene.TrackingLayer.Add(gp, "measurePoint");
                        }
                    }

                    // 删除显示的“当前面积”
                    int k = m_sceneControl.Scene.TrackingLayer.IndexOf("curA");
                    if (k >= 0)
                    {
                        m_sceneControl.Scene.TrackingLayer.Remove(k);
                    }

                    double totA = e.Area;
                    //this.totalResult.Text = "面积" + totA + "平方米";

                    GeoText3D text3D = CreateText3DMessage("总面积" + totA + "平方米", gr.BoundingBox.Center);
                    m_sceneControl.Scene.TrackingLayer.Add(text3D, "totA");
                }
            }
        }

        /// <summary>
        /// 创建三维文本几何对象
        /// </summary>
        /// <param name="str">文本内容</param>
        /// <param name="p3d">文本位置</param>
        /// <returns>三维文本几何对象</returns>
        private GeoText3D CreateText3DMessage(string str, Point3D p3d)
        {
            TextPart3D textPart3D = new TextPart3D();
            textPart3D.AnchorPoint = p3d;
            textPart3D.Text = str;

            TextStyle style = new TextStyle();
            style.ForeColor = System.Drawing.Color.White;
            style.IsSizeFixed = true;
            style.FontHeight = 5;
            style.Shadow = true;

            GeoText3D text3D = new GeoText3D(textPart3D, style);
            GeoStyle3D geoStyle3D = new GeoStyle3D();
            geoStyle3D.AltitudeMode = AltitudeMode.RelativeToGround;
            text3D.Style3D = geoStyle3D;

            return text3D;
        }

        /// <summary>
        /// 量算点样式
        /// </summary>
        /// <returns>量算点样式</returns>
        private GeoStyle3D GeoPoint3DStyle()
        {
            GeoStyle3D g3d = new GeoStyle3D();
            g3d.AltitudeMode = AltitudeMode.Absolute;
            g3d.MarkerColor = System.Drawing.Color.Red;
            g3d.MarkerSize = 4;

            return g3d;
        }

        /// <summary>
        /// 量算线样式
        /// </summary>
        /// <returns>量算线样式</returns>
        private GeoStyle3D GeoLine3DStyle()
        {
            GeoStyle3D g3d = new GeoStyle3D();
            g3d.AltitudeMode = AltitudeMode.Absolute;
            g3d.LineColor = System.Drawing.Color.Yellow;
            g3d.LineWidth = 2;

            return g3d;
        }

        /// <summary>
        ///  量算面样式
        /// </summary>
        /// <returns> 量算面样式</returns>
        private GeoStyle3D GeoRegion3DStyle()
        {
            GeoStyle3D g3d = new GeoStyle3D();
            g3d.FillForeColor = System.Drawing.Color.FromArgb(100, 255, 255, 0);
            g3d.AltitudeMode = AltitudeMode.Absolute;

            return g3d;
        }
    
    }
}
