using SuperMap.Data;
using SuperMap.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private List<string> results;

        /// <summary>
        ///  构造地址搜索工具
        /// </summary>
        /// <param name="sceneControl">三维场景控件</param>
        public SearchAddress(SceneControl sceneControl)
        {
            this.m_sceneControl = sceneControl;
            this.m_workspace = sceneControl.Scene.Workspace;

            results = new List<string>();
        }

        /// <summary>
        /// 查找地址
        /// </summary>
        /// <param name="searchText">查找词</param>
        /// <returns>查找结果</returns>
        public List<string> Search(string searchText)
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

            for (int i = 0; i < m_workspace.Datasources[0].Datasets.Count; ++i)
            {
                dv = m_workspace.Datasources[0].Datasets[i] as DatasetVector;

                if (dv == null) continue;

                qp = new QueryParameter();
                qp.HasGeometry = true;
                qp.CursorType = CursorType.Static;

                Recordset recordset = dv.Query(qp);

                FieldInfos infos = recordset.GetFieldInfos();

                while (!recordset.IsEOF)
                {
                    if (!Contains(infos, "NAME"))
                    {
                        recordset.MoveNext();
                        continue;
                    }
                    string name = recordset.GetFieldValue("NAME").ToString();
                    if (name.Contains(searchText))
                    {
                        results.Add("Name : " + name);
                    }
                    recordset.MoveNext();
                }

                recordset.Dispose();
            }

            return results;
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
