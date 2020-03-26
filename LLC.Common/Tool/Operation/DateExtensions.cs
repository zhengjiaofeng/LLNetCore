using System;
using System.Collections.Generic;
using System.Text;

namespace LLC.Common.Tool.Operation
{
    public static class DateExtensions
    {
        /// <summary>
        ///     将时间转换为yyyy-MM-dd格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>yyyy-MM-dd字符串</returns>
        public static string ToShortDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        /// <summary>
        ///     转换为yyyy-MM-dd HH:mm:ss格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>yyyy-MM-dd HH:mm:ss字符串</returns>
        public static string ToLongDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
