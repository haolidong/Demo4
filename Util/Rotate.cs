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
    class Rotate
    {
        private SceneControl m_sceneControl;

        private Point3D point;

        public Rotate(SceneControl sceneControl)
        {
            this.m_sceneControl = sceneControl;
            this.point = Point3D.Empty;

            // 鼠标点击事件监听
            m_sceneControl.MouseClick += new MouseEventHandler(m_sceneControl_Click);
        }

        /// <summary>
        /// 鼠标单击事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_sceneControl_Click(Object sender, MouseEventArgs e)
        {
            // 保存每次最后单击的点的位置
            point = m_sceneControl.Scene.PixelToGlobe(new System.Drawing.Point(e.X, e.Y),
                                PixelToGlobeMode.TerrainAndModel);
        }

        /// <summary>
        /// 绕物旋转
        /// </summary>
        /// <param name="speed">旋转速度</param>
        public void RotateByObject(double speed)
        {
            try
            {
                Selection3D select = m_sceneControl.Scene.FindSelection(true)[0];
                SuperMap.Data.Geometry geo = select.ToRecordset().GetGeometry();
                m_sceneControl.Scene.FlyCircle(geo, speed);
            }
            catch (Exception e)
            {
                GeoPoint3D gp = new GeoPoint3D(point);
                m_sceneControl.Scene.FlyCircle(gp, speed);
            }
        }

    }
}
