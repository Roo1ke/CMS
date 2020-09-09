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
    public class Sys_MenuService: ISys_MenuService
    {
        private readonly ISys_MenuRepository _rep;
        public Sys_MenuService(ISys_MenuRepository rep)
        {
            _rep = rep;
        }

        public async Task<ResultMsg> DeleteSys_Menu(int PKID)
        {
            return await _rep.DeleteSys_Menu(PKID);
        }

        public async Task<PagedList<Sys_Menu>> GetMenuPagedList(int pageIndex, int pageSize, string condictions)
        {
            return await _rep.GetMenuPagedList(pageIndex,pageSize,condictions);
        }

        public async Task<ResultMsg> SaveSys_Menu(Sys_Menu model)
        {
            return await _rep.SaveSys_Menu(model);
        }
        public async Task<List<Sys_Menu>> GetParentMenus()
        {
            return await _rep.GetParentMenus();
        }
        public async Task<List<Sys_Menu_Operation>> GetOperation(int menuid)
        {
            return await _rep.GetOperation(menuid);
        }
    }
}
