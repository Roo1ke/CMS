﻿using CMS.Common.DB;
using CMS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.IService
{
    public interface ISys_UserService
    {
        Task<ResultMsg> SaveSys_User(Sys_Users user);
        Task<bool> CheckMobilePhone(int PKID, string MobilePhone);
        Task<ResultMsg> UserLogin(string account, string pwd);
        Task<bool> ClearErrCount(int PKID);
        Task<PagedList<Sys_Users>> GetUserPagedList(int pageIndex, int pageSize, string condictions);
    }
}