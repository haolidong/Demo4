using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo4.Search
{
    /// <summary>
    /// 地址类，主要用于地址搜索的结果封装
    /// </summary>
    class Address
    {
        /// <summary>
        /// 该地址在结果集合中的序号
        /// </summary>
        public int Num
        {
            get;
            set;
        }

        /// <summary>
        /// 地址对应的地名
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Detail
        {
            get;
            set;
        }

        /// <summary>
        /// 地址的纬度
        /// </summary>
        public double Latitude
        {
            get;
            set;
        }

        /// <summary>
        /// 地址的经度
        /// </summary>
        public double Longitude
        {
            get;
            set;
        }

        /// <summary>
        /// 地址的高度
        /// </summary>
        public double Altitude
        {
            get;
            set;
        }

        /// <summary>
        /// 该地址的描述信息
        /// </summary>
        public string Introduction
        {
            get;
            set;
        }

    }
}
