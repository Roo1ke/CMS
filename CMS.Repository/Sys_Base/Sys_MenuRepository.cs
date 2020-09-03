using CMS.Common;
using CMS.Common.DB;
using CMS.IRepository;
using CMS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository
{
    public class Sys_MenuRepository : RepositoryBase<Sys_Menu>, ISys_MenuRepository
    {
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condictions"></param>
        /// <returns></returns>
        public async Task<PagedList<Sys_Menu>> GetMenuPagedList(int pageIndex, int pageSize, string condictions)
        {
            var rs = await GetPagedList(pageIndex, pageSize, "sys_menu a left join sys_menu b on a.ParentID=b.PKID", "a.*,b.MenuName ParentName", condictions, "PKID");
            return rs;
        }

        public async Task<List<Sys_Menu>> GetParentMenus()
        {
            string selectSql = "Select PKID,MenuName From sys_menu WHERE Status<>-1 AND ParentID=0";
            return await Select(selectSql);
        }

        public async Task<ResultMsg> DeleteSys_Menu(int PKID)
        {
            ResultMsg rs = new ResultMsg() { Msg = "操作失败" };
            string deleteSql = "Update Sys_Menu Set Status=-1 where PKID=@PKID";
            if (await Delete(PKID, deleteSql) > 0)
            {
                rs.Code = 1;
                rs.Msg = "操作成功";
            }
            throw new NotImplementedException();
        }

        public async Task<ResultMsg> SaveSys_Menu(Sys_Menu model)
        {
            ResultMsg rs = new ResultMsg() { Msg = "操作失败" };
            if (model.PKID == 0)
            {
                string insertSql = "INSERT INTO Sys_Menu (ParentID,MenuName,Icon,Path,Status) values (@ParentID,@MenuName,@Icon,@Path,@Status)";
                if (await Insert(model, insertSql) > 0)
                {
                    rs.Code = 1;
                    rs.Msg = "操作成功";
                }
            }
            else {
                string updateSql = "UPDATE Sys_Menu Set ParentID=@ParentID,MenuName=@MenuName,Icon=@Icon,Path=@Path,Status=@Status WHERE PKID=@PKID";
                if (await Update(model, updateSql) > 0)
                {
                    rs.Code = 1;
                    rs.Msg = "操作成功";
                }
            }
            return rs;
        }
    }
}
