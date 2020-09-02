using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings =false,ErrorMessage ="用户名不能为空")]
        public string account { set; get; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        public string pwd { set; get; }
    }
}
