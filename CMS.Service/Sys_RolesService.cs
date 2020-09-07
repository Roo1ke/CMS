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
    public class Sys_RolesService: ISys_RolesService
    {
        private readonly ISys_RolesRepository _rep;
        public Sys_RolesService(ISys_RolesRepository rep)
        {
            _rep = rep;
        }

        public async Task<ResultMsg> DeleteSys_Roles(int PKID)
        {
            return await _rep.DeleteSys_Roles(PKID);
        }

        public async Task<PagedList<Sys_Roles>> GetRolePagedList(int pageIndex, int pageSize, string condictions)
        {
            return await _rep.GetRolePagedList(pageIndex,pageSize,condictions);
        }

        public async Task<ResultMsg> SaveSys_Roles(Sys_Roles model)
        {
            return await _rep.SaveSys_Roles(model);
        }

        public async Task<ResultMsg> SaveRolePerrmission(int PKID, string permissionids)
        {
            return await _rep.SaveRolePerrmission(PKID, permissionids);
        }
    }
}
