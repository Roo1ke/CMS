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
    public class Sys_RolesRepository : RepositoryBase<Sys_Roles>, ISys_RolesRepository
    {

        public async Task<PagedList<Sys_Roles>> GetRolePagedList(int pageIndex, int pageSize, string condictions)
        {
            var rs = await GetPagedList(pageIndex, pageSize, "Sys_Roles ", "*", condictions, "PKID");
            return rs;
        }

        public async Task<ResultMsg> DeleteSys_Roles(int PKID)
        {
            ResultMsg rs = new ResultMsg() { Msg = "操作失败" };
            string deleteSql = "Update Sys_Roles Set Status=-1 where PKID=@PKID";
            if (await Delete(PKID, deleteSql) > 0)
            {
                rs.Code = 1;
                rs.Msg = "操作成功";
            }
            throw new NotImplementedException();
        }

        public async Task<ResultMsg> SaveSys_Roles(Sys_Roles model)
        {
            ResultMsg rs = new ResultMsg() { Msg = "操作失败" };
            if (model.PKID == 0)
            {
                string insertSql = "INSERT INTO Sys_Roles (RoleName,Description,Status) values (@RoleName,@Description,@Status)";
                if (await Insert(model, insertSql) > 0)
                {
                    rs.Code = 1;
                    rs.Msg = "操作成功";
                }
            }
            else
            {
                string updateSql = "UPDATE Sys_Roles Set RoleName=@RoleName,Description=@Description,Status=@Status WHERE PKID=@PKID";
                if (await Update(model, updateSql) > 0)
                {
                    rs.Code = 1;
                    rs.Msg = "操作成功";
                }
            }
            return rs;
        }

        public async Task<ResultMsg> SaveRolePerrmission(int PKID, string permissionids)
        {
            ResultMsg rs = new ResultMsg() { Msg = "操作失败" };
            string updateSql = "UPDATE Sys_Roles Set PermissionIDS=@PermissionIDS WHERE PKID=@PKID";
            if (await Update(new Sys_Roles { PermissionIDS = permissionids, PKID = PKID }, updateSql) > 0)
            {
                rs.Code = 1;
                rs.Msg = "操作成功";
            }
            return rs;
        }




    }
}
