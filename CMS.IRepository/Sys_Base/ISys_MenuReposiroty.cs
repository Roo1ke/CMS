using CMS.Common.DB;
using CMS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.IRepository
{
    public interface ISys_MenuRepository
    {
        Task<PagedList<Sys_Menu>> GetMenuPagedList(int pageIndex, int pageSize, string condictions);
        Task<List<Sys_Menu>> GetParentMenus();
        Task<ResultMsg> SaveSys_Menu(Sys_Menu model);
        Task<ResultMsg> DeleteSys_Menu(int PKID);
        Task<List<Sys_Menu_Operation>> GetOperation(int menuid);

    }
}
