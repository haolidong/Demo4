using Demo4.Addition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Demo4.Search
{
    /// <summary>
    /// 分页器
    /// </summary>
    class Pagination
    {
        private int m_PAGESIZE;
        private int m_totalPage;
        private int m_current;

        private Panel m_page;
        private Panel m_pageNumber;
        private IEnumerable<Address> m_address;

        /// <summary>
        /// 根据参数中的控件构造一个分页器
        /// </summary>
        /// <param name="page">用作分页器的控件</param>
        /// <param name="pageSize">每页容量</param>
        /// <param name="address">数据源</param>
        public Pagination(Panel page, int pageSize, IEnumerable<Address> address)
        {
            m_page = page;
            m_pageNumber = page.Children[1] as Panel;
            m_PAGESIZE = pageSize;
            m_address = address;

            m_totalPage = (int)Math.Ceiling(m_address.Count() / (double)m_PAGESIZE);
            m_current = 0;

            /** 
             * 激活上一页、下一页按钮
             * 对于上一页、下一页按钮，Selectable.SelectProperty不再代表其是否选中（因为这两个按钮是无法选中的）
             * 而是代表其是否可用
             */
            Selectable.SetSelectStatus(m_page.Children[0], true);
            Selectable.SetSelectStatus(m_page.Children[2], true);
        }

        /// <summary>
        /// 设置当前页
        /// </summary>
        /// <param name="index">当前页码</param>
        /// <returns>当前页的数据</returns>
        public IEnumerable<Address> SetCurrent(int index)
        {
            // 只有一页，不显示分页
            if(m_totalPage <= 1){
                m_page.Visibility = Visibility.Collapsed;
                return m_address;
            }

            // 显示分页
            m_page.Visibility = Visibility.Visible;
            m_pageNumber.Children.Clear(); // 清除页码

            m_current = index;

            if (m_totalPage <= 5) // 少于5页，显示所有页码
            {
                #region
                for (int i = 0; i < m_totalPage; ++i)
                {
                    if (i == index)
                    {
                        m_pageNumber.Children.Add(SelectTextBlock(CreateTextBlock(null, (i + 1).ToString())));
                        continue;
                    }
                    m_pageNumber.Children.Add(CreateTextBlock(null, (i + 1).ToString()));
                }
                #endregion
            }
            else // 大于5页，显示部分页码，加...
            {
                #region
                int begin = index - 2;
                int end = index + 2;

                if (begin <= 0)
                {
                    #region
                    for (int i = 0; i < 4; ++i)
                    {
                        if (i == index)
                        {
                            m_pageNumber.Children.Add(SelectTextBlock(CreateTextBlock(null, (i + 1).ToString())));
                            continue;
                        }
                        m_pageNumber.Children.Add(CreateTextBlock(null, (i + 1).ToString()));
                    }
                    m_pageNumber.Children.Add(CreateTextBlock(null, "..."));
                    #endregion
                }
                else if (end >= m_totalPage - 1)
                {
                    #region
                    m_pageNumber.Children.Add(CreateTextBlock(null, "..."));
                    for (int i = m_totalPage - 4; i < m_totalPage; ++i)
                    {
                        if (i == index)
                        {
                            m_pageNumber.Children.Add(SelectTextBlock(CreateTextBlock(null, (i + 1).ToString())));
                            continue;
                        }
                        m_pageNumber.Children.Add(CreateTextBlock(null, (i + 1).ToString()));
                    }
                    #endregion
                }
                else
                {
                    #region
                    m_pageNumber.Children.Add(CreateTextBlock(null, "..."));
                    for (int i = begin + 1; i < end; ++i)
                    {
                        if (i == index)
                        {
                            m_pageNumber.Children.Add(SelectTextBlock(CreateTextBlock(null, (i + 1).ToString())));
                            continue;
                        }
                        m_pageNumber.Children.Add(CreateTextBlock(null, (i + 1).ToString()));
                    }
                    m_pageNumber.Children.Add(CreateTextBlock(null, "..."));
                    #endregion
                }
                #endregion
            }

            // 根据当前页码禁用上一页、下一页按钮
            if (index <= 0)
                Selectable.SetSelectStatus(m_page.Children[0], false);
            else
                Selectable.SetSelectStatus(m_page.Children[0], true);

            if (index >= m_totalPage - 1)
                Selectable.SetSelectStatus(m_page.Children[2], false);
            else
                Selectable.SetSelectStatus(m_page.Children[2], true);

            // 返回当前页的数据
            return m_address.Skip(index * m_PAGESIZE).Take(m_PAGESIZE);
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <returns>上一页的数据</returns>
        public IEnumerable<Address> Previous()
        {
            // 若已禁止上一页，返回空
            if (!Selectable.GetSelectStatus(m_page.Children[0]))
            {
                return null;
            }
            return SetCurrent(m_current - 1);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <returns>下一页的数据</returns>
        public IEnumerable<Address> Next()
        {
            // 若已禁止一下页，返回空
            if (!Selectable.GetSelectStatus(m_page.Children[2]))
            {
                return null;
            }
            return SetCurrent(m_current + 1);
        }

        /// <summary>
        /// 选中某TextBlock
        /// </summary>
        /// <param name="tb">要选中的TextBlock</param>
        /// <returns>选中后的TextBlock</returns>
        private TextBlock SelectTextBlock(TextBlock tb)
        {
            Selectable.SetSelectStatus(tb, true);
            tb.Background = new SolidColorBrush(Color.FromArgb(200, 5, 147, 211));
            tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

            return tb;
        }

        /// <summary>
        /// 生成具有指定name和text的TextBlock
        /// </summary>
        /// <param name="name">指定生成TextBlock的Name</param>
        /// <param name="text">指定生成TextBlock的Text</param>
        /// <returns>生成的TextBlock</returns>
        private TextBlock CreateTextBlock(string name, string text)
        {
            TextBlock tb = new TextBlock();

            if (name != null && !name.Equals(""))
            {
                tb.Name = name;
            }
            if (text != null && !text.Equals(""))
            {
                tb.Text = text;
            }

            tb.Padding = new Thickness(8, 5, 8, 5);
            tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 5, 147, 211));

            if (!text.Equals("..."))
            {
                tb.MouseEnter += new MouseEventHandler(InPageNum);
                tb.MouseLeave += new MouseEventHandler(OutPageNum);
            }

            return tb;
        }

        /// <summary>
        /// 鼠标进入分页器页码改变背景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InPageNum(object sender, MouseEventArgs e)
        {
            TextBlock tb = e.OriginalSource as TextBlock;
            if (Selectable.GetSelectStatus(tb))
            {
                return;
            }

            tb.Background = new SolidColorBrush(Color.FromArgb(200, 5, 147, 211));
            tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        /// <summary>
        /// 鼠标移出分页器页码，恢复背景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutPageNum(object sender, MouseEventArgs e)
        {
            TextBlock tb = e.OriginalSource as TextBlock;

            if (Selectable.GetSelectStatus(tb))
            {
                return;
            }

            tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 5, 147, 211));
            tb.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

    }
}
