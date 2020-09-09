using CMS.Common;
using CMS.Common.DB;
using CMS.IRepository;
using CMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using System.Linq;

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
            rs.Items.ForEach(async e =>
            {
                e.operation = await GetOperation(e.PKID);
            });
            return rs;
        }

        public async Task<List<Sys_Menu>> GetParentMenus()
        {
            string selectSql = "Select PKID,MenuName From sys_menu WHERE Status<>-1 AND ParentID=0";
            return await Select(selectSql);
        }

        public async Task<List<Sys_Menu_Operation>> GetOperation(int menuid)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                string selectSql = "Select * From Sys_Menu_Operation WHERE  menuid=@menuid";
                return await Task.Run(() => conn.Query<Sys_Menu_Operation>(selectSql,new { menuid}).ToList()); 
            }
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
            bool _rs = true;
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                using (var tran = conn.BeginTransaction()) {
                    if (model.PKID == 0)
                    {
                        string insertSql = "INSERT INTO Sys_Menu (ParentID,MenuName,Icon,Path,Status) values (@ParentID,@MenuName,@Icon,@Path,@Status);SELECT @@identity";
                        int pkid = await conn.ExecuteScalarAsync<int>(insertSql, model);
                        if (pkid > 0)
                        {
                            model.operation.ForEach(async e =>
                            {
                                e.MenuID = pkid;
                                string sql = "INSERT INTO Sys_Menu_Operation (MenuID,OperationName) values (@MenuID,@OperationName)";
                                if (await conn.ExecuteAsync(sql, e) == 0)
                                {
                                    _rs = false;
                                }
                            });
                        }
                    }
                    else
                    {
                        string updateSql = "UPDATE Sys_Menu Set ParentID=@ParentID,MenuName=@MenuName,Icon=@Icon,Path=@Path,Status=@Status WHERE PKID=@PKID";
                        if (await conn.ExecuteAsync(updateSql,model) > 0)
                        {
                            var operations = await GetOperation(model.PKID);
                            operations.ForEach(async e =>
                            {
                                if (!model.operation.Any(x => x.PKID == e.PKID)) {
                                    var PKID = e.PKID;
                                    string delete_sql = "DELETE FROM  Sys_Menu_Operation WHERE PKID=@PKID";
                                    await conn.ExecuteAsync(delete_sql, new { PKID });
                                }
                            });
                            model.operation.ForEach(async e =>
                            {
                                string sql = "";
                                if (e.PKID == 0){
                                    sql = "INSERT INTO Sys_Menu_Operation (MenuID,OperationName) values (@MenuID,@OperationName)";
                                }
                                else {
                                    sql = "Update  Sys_Menu_Operation Set OperationName=@OperationName where PKID=@PKID";
                                }
                                
                                if (await conn.ExecuteAsync(sql, e) == 0)
                                {
                                    _rs = false;
                                }
                            });
                        }
                    }
                    if (_rs)
                    {
                        rs.Code = 1;
                        rs.Msg = "操作成功";
                        tran.Commit();
                    }
                    else {
                        tran.Rollback();
                    }
                }
                
            }
            return rs;
        }
    }
}
