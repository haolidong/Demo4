using SuperMap.Data;
using SuperMap.Realspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo4.Util
{
    class Rotate
    {
        // 绕物旋转
        public static void RotateByObject(Scene scene, Geometry geo, double speed)
        {
            scene.FlyCircle(geo, speed);
        }
    }
}
