using SuperMap.Realspace;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo4.Util
{
    /// <summary>
    /// 显示控制工具类
    /// </summary>
    class Display
    {
        /// <summary>
        /// 显示、关闭帧率
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        /// <returns>当前帧率显示状态，true代表显示，false代表关闭</returns>
        public static bool ToogleFPS(SceneControl sceneControl)
        {
            return sceneControl.IsFPSVisible = !sceneControl.IsFPSVisible;
        }

        /// <summary>
        /// 显示帧率
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public static void ShowFPS(SceneControl sceneControl)
        {
            sceneControl.IsFPSVisible = true;
        }

        /// <summary>
        /// 关闭帧率
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public static void HideFPS(SceneControl sceneControl)
        {
            sceneControl.IsFPSVisible = false;
        }

        /// <summary>
        /// 显示、关闭经纬网
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        /// <returns>当前经纬网显示状态，true代表显示，false代表关闭</returns>
        public static bool ToogleLatLon(SceneControl sceneControl)
        {
            LatLonGrid latlon = sceneControl.Scene.LatLonGrid;
            return latlon.IsVisible = !latlon.IsVisible;
        }

        /// <summary>
        /// 显示经纬网
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public static void ShowLatLon(SceneControl sceneControl)
        {
            sceneControl.Scene.LatLonGrid.IsVisible = true;
        }

        /// <summary>
        /// 关闭经纬网
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public static void HideLatLon(SceneControl sceneControl)
        {
            sceneControl.Scene.LatLonGrid.IsVisible = false;
        }

    }
}
