using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.ViewModels
{
    public class ModifyPwdModel
    {
        [Required(AllowEmptyStrings =false,ErrorMessage ="信息不完整")]
        public int PKID { set; get; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "旧密码不能为空")]
        public string OldPassword { set; get; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "新密码不能为空")]
        public string NewPassword { set; get; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "请确认新密码")]
        public string Confirm_NewPassword { set; get; }
    }
}
