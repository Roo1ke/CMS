using CMS.Common;
using CMS.Common.DB;
using CMS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.IRepository
{
    public interface ISys_UserRepository : IRepositoryBase<Sys_Users>
    {
        Task<ResultMsg> SaveSys_User(Sys_Users user);
        Task<PagedList<Sys_Users>> GetUserPagedList(int pageIndex, int pageSize, string condictions);
        Task<bool> CheckMobilePhone(int PKID, string MobilePhone);
        Task<bool> CheckLoginName(int PKID, string loginName);
        Task<ResultMsg> Get_UsersAsyncByPKID(int PKID);
        Task<ResultMsg> UserLogin(string account, string pwd);
        Task<ResultMsg> ModifyPassword(int PKID, string oldPassword, string newPassword);
        Task<bool> ClearErrCount(int PKID);
        Task<List<Sys_Menu>> GetUserPermission(int userid);
        Task<bool> ValidUserPermission(int userid, string path, string operation);
        Task<int> GetUserTotalCount();
        Task<ResultMsg> DeleteUser(int PKID);
    }
}
