using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Demo4.Addition
{
    /// <summary>
    /// 附加属性类，代表控件是否可被选择，模拟控件的选中和未选中两种状态
    /// </summary>
    class Selectable :DependencyObject
    {
        public static bool GetSelectStatus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectProperty);
        }

        public static void SetSelectStatus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectProperty, value);
        }

        public static readonly DependencyProperty SelectProperty =
            DependencyProperty.RegisterAttached("SelectStatus", typeof(bool), 
                                typeof(Selectable), new UIPropertyMetadata(false));
    }
}
