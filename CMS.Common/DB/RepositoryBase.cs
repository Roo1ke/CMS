using CMS.Common.DB;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Common
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        public async Task<int> Delete(int PKID, string deleteSql)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                return await conn.ExecuteAsync(deleteSql, new { PKID });
            }
        }

        public async Task<T> Detail(int PKID, string detailSql)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                //string querySql = @"SELECT Id, UserName, Password, Gender, Birthday, CreateDate, IsDelete FROM dbo.Users WHERE Id=@Id";
                return await conn.QueryFirstOrDefaultAsync<T>(detailSql, new { PKID });
            }
        }

        /// <summary>
        /// 无参存储过程
        /// </summary>
        /// <param name="SPName"></param>
        /// <returns></returns>
        public async Task<List<T>> ExecQuerySP(string SPName)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                return await Task.Run(() => conn.Query<T>(SPName, null, null, true, null, CommandType.StoredProcedure).ToList());
            }
        }

        public async Task<int> Insert(T entity, string insertSql)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
               return await conn.ExecuteAsync(insertSql, entity);
            }
        }

        public async Task<List<T>> Select(string selectSql)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                //string selectSql = @"SELECT Id, UserName, Password, Gender, Birthday, CreateDate, IsDelete FROM dbo.Users";
                return await Task.Run(() => conn.Query<T>(selectSql).ToList());
            }
        }

        public async Task<int> Update(T entity, string updateSql)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                return await conn.ExecuteAsync(updateSql, entity);
            }
        }

        public  async Task<PagedList<T>> GetPagedList(int pageIndex, int pageSize,string tableName,string files, string conditions, string orderby,  object parameters = null)
        {
            var rs = await DapperHelper.GetPagedListAsync<T>(pageIndex, pageSize, tableName, files, conditions, orderby, parameters);
            return rs;
        }
    }
}
