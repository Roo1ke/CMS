using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Common
{
    public class DateTimeHelper
    {
        /// <summary>
        /// 时间戳转换为日期（时间戳单位秒）
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string timeStamp)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return start.AddSeconds(long.Parse(timeStamp)).AddHours(8);
        }
    }
}
