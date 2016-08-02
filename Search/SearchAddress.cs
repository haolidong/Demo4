using SuperMap.Data;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Demo4.Search
{
    /// <summary>
    /// 地址搜索
    /// </summary>
    class SearchAddress
    {
        private SceneControl m_sceneControl;
        private Workspace m_workspace;

        // 搜索结果
        //private List<string> results;
        private ObservableCollection<Address> results;

        /// <summary>
        ///  构造地址搜索工具
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public SearchAddress(SceneControl sceneControl)
        {
            this.m_sceneControl = sceneControl;
            this.m_workspace = sceneControl.Scene.Workspace;

            results = new ObservableCollection<Address>();
        }

        /// <summary>
        /// 查找地址
        /// </summary>
        /// <param name="searchText">查找词</param>
        /// <returns>查找结果</returns>
        public ObservableCollection<Address> Search(string searchText)
        {
            // 未输入任何查询条件，不进行查询
            if (searchText.Equals("") || searchText == null)
            {
                System.Windows.MessageBox.Show("请输入查询条件");
                return null;
            }

            results.Clear();

            // 查询
            DatasetVector dv = null;
            QueryParameter qp = null;

            // 在Building数据集中查找地址
            dv = m_workspace.Datasources[0].Datasets["Building"] as DatasetVector;
            if (dv == null)
            {
                System.Windows.MessageBox.Show("数据集为空");
            }

            qp = new QueryParameter();
            qp.HasGeometry = true;
            qp.CursorType = CursorType.Static;
            Recordset recordset = dv.Query(qp);
            Object ob = null;

            // 遍历记录集
            while (!recordset.IsEOF)
            {
                ob = recordset.GetFieldValue("Name");
                // 如果Name为空，退出
                if (ob == null) 
                {
                    recordset.MoveNext();
                    continue; 
                }

                string name = ob.ToString();
                // 当记录集中的该记录的Name字段包含搜索关键字，则当做匹配成功
                if (name.Contains(searchText))
                {
                    results.Add(CreateAddress(recordset));
                }
                recordset.MoveNext();
            }

            recordset.Dispose();
            return results;
        }

        /// <summary>
        /// 根据记录集中的当前记录，构造一个Address对象
        /// </summary>
        /// <param name="recordeset">记录集</param>
        /// <returns>地址对象</returns>
        private Address CreateAddress(Recordset recordset)
        {
            Address address = new Address();

            string name = recordset.GetFieldValue("Name") as string;
            string detail = recordset.GetFieldValue("Address") as string;
            string introduction = recordset.GetFieldValue("Introduction") as string;
            double latitude  = (double)recordset.GetFieldValue("Latitude");
            double longitude  = (double)recordset.GetFieldValue("Longitude");
            double altitude  = (double)recordset.GetFieldValue("Altitude");

            address.Num = results.Count + 1;
            address.Name = name == null ? "无地名" : name;
            address.Detail = detail == null ? "无详细地址" : detail;
            address.Introduction = introduction == null ? "无详细信息" : introduction;
            address.Latitude = latitude;
            address.Longitude = longitude;
            address.Altitude = altitude;

            return address;
        }

        /// <summary>
        /// 查找是否存在指定字段
        /// </summary>
        /// <param name="fis">字段信息</param>
        /// <param name="name">要查询的字段</param>
        /// <returns>查询结果，true代表存在，false代表不存在</returns>
        private bool Contains(FieldInfos fis, string name)
        {
            foreach (FieldInfo fi in fis)
            {
                if (fi.Name.Equals(name)) return true;
            }
            return false;
        }

    }
}
