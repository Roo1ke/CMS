using CMS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.DTO
{
    public class Sys_UserDTO
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
        /// 手机号
        /// </summary>
        public string MobilePhone { set; get; }
        public string HeaderImgUrl { set; get; }
        public string Roles { set; get; }
        public List<MenusTreeModel> menus { set; get; }
        public List<Sys_Menu> permission { set; get; }

    }
}
