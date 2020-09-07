using CMS.Common.DB;
using CMS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.IRepository
{
    public interface ISys_RolesRepository
    {
        Task<PagedList<Sys_Roles>> GetRolePagedList(int pageIndex, int pageSize, string condictions);
        Task<ResultMsg> SaveSys_Roles(Sys_Roles model);
        Task<ResultMsg> DeleteSys_Roles(int PKID);
        Task<ResultMsg> SaveRolePerrmission(int PKID, string permissionids);

    }
}
