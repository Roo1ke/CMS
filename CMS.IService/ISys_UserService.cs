using CMS.Common.DB;
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
        Task<bool> CheckLoginName(int PKID, string loginName);
        Task<ResultMsg> Get_UsersAsyncByPKID(int PKID);
        Task<ResultMsg> UserLogin(string account, string pwd);
        Task<ResultMsg> ModifyPassword(int PKID, string oldPassword, string newPassword);
        Task<bool> ClearErrCount(int PKID);
        Task<PagedList<Sys_Users>> GetUserPagedList(int pageIndex, int pageSize, string condictions);
        Task<List<Sys_Menu>> GetUserPermission(int userid);
        Task<bool> ValidUserPermission(int userid, string path, string operation);
        Task<int> GetUserTotalCount();
        Task<ResultMsg> DeleteUser(int PKID);
    }
}
