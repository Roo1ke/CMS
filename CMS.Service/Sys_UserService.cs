using CMS.Common.DB;
using CMS.IRepository;
using CMS.IService;
using CMS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Service
{
    public class Sys_UserService : ISys_UserService
    {
        private readonly ISys_UserRepository _userRep;
        public Sys_UserService(ISys_UserRepository userRep)
        {
            _userRep = userRep;
        }

        public async Task<ResultMsg> SaveSys_User(Sys_Users user)
        {
            return await _userRep.SaveSys_User(user);
        }

        public async Task<PagedList<Sys_Users>> GetUserPagedList(int pageIndex, int pageSize, string condictions)
        {
            return await _userRep.GetUserPagedList(pageIndex, pageSize, condictions);
        }

        public async Task<bool> CheckMobilePhone(int PKID, string mobilePhone)
        {
            return await _userRep.CheckMobilePhone(PKID, mobilePhone);
        }

        public async Task<bool> CheckLoginName(int PKID, string loginName)
        {
            return await _userRep.CheckLoginName(PKID, loginName);
        }

        public async Task<bool> ClearErrCount(int PKID)
        {
            return await _userRep.ClearErrCount(PKID);
        }
        public async Task<ResultMsg> Get_UsersAsyncByPKID(int PKID)
        {
            return await _userRep.Get_UsersAsyncByPKID(PKID);
        }

        public async Task<ResultMsg> UserLogin(string account, string pwd)
        {
            return await _userRep.UserLogin(account, pwd);
        }
        public async Task<ResultMsg> ModifyPassword(int PKID, string oldPassword, string newPassword)
        {
            return await _userRep.ModifyPassword(PKID, oldPassword, newPassword);

        }
        public async Task<List<Sys_Menu>> GetUserPermission(int userid)
        {
            return await _userRep.GetUserPermission(userid);
        }
        public async Task<bool> ValidUserPermission(int userid, string path, string operation)
        {
            return await _userRep.ValidUserPermission(userid, path,operation);
        }

    }
}
