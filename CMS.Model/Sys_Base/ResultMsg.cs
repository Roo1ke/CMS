using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Model
{
    public class ResultMsg
    {
        public int Code { set; get; }
        public string Msg { set; get; }
        public dynamic Data { set; get; }
    }
}
