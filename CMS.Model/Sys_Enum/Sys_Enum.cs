using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Model
{
    public enum Sys_UserState_Enum
    {
        Delete=-1,
        Forbidden,
        Normal,
    }
    public enum Sys_UserLocked_Enum
    {
        UnLocked,
        Locked,
    }
}
