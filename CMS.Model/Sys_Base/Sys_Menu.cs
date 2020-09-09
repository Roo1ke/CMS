using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Model
{
    public class Sys_Menu
    {
        public int PKID { set; get; }
        public int ParentID { set; get; }
        public string MenuName { set; get; }
        public string Icon { set; get; }
        public string Path { set; get; }
        public int Status { set; get; }
        public string ParentName { set; get; }

        public List<Sys_Menu_Operation> operation { set; get; }
    }

    public class Sys_Menu_Operation {
        public int PKID { set; get; }
        public int MenuID { set; get; }
        public string OperationName { set; get; }
    }
}
