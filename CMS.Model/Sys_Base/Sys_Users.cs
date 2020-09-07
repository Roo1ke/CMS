using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Model
{
    public class Sys_Users
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { set; get; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public string Roles { set; get; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string MobilePhone { set; get; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public int DepID { set; get; }
        /// <summary>
        /// 登录错误计数，登录成功后清零
        /// </summary>
        public int ErrCount { set; get; }
        /// <summary>
        /// 账户解锁时间
        /// </summary>
        public DateTime UnlockedTime { set; get; }
        /// <summary>
        /// 账户状态
        /// </summary>
        public int Status { set; get; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public int Is_Locked { set; get; }

        public DateTime Createtime { set; get; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeaderImgUrl { set; get; }
    }
}
