﻿using System;
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
    }
}
