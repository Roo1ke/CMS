using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Model
{
    public class Sys_Roles
    {
        public int PKID { set; get; }
        public string RoleName { set; get; }
        public string Description { set; get; }
        public int Status { set; get; }
        public string PermissionIDS { set; get; }
    }
}
