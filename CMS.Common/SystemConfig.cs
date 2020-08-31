using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Common
{
    public class SystemConfig
    {
        //默认密码
        public static readonly string DEAFULT_PWD = "1111110";
        //登录最大错误计数
        public static readonly int MAX_ERR_COUNT = 5;
        //账户锁定后自动解锁时间(分钟数)
        public static readonly int LOCKED_TIME_MIN = 60;
    }
}
