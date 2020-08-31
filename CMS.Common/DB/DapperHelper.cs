using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Common.DB
{
    public class DapperHelper
    {
        public static async Task<PagedList<T>> GetPagedListAsync<T>(int pageIndex, int pageSize, string tableName, string files, string conditions, string orderby, object parameters = null) where T : class
        {
           
            PagedList<T> pagedList =new PagedList<T>();
            StringBuilder sb_total = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT COUNT(1) FROM {0} where {1};", tableName, conditions);
            sb.AppendFormat(@"SELECT  {0} FROM  {1} WHERE {2} ORDER BY {3} limit {4},{5}", files, tableName, conditions, orderby, (pageIndex - 1) * pageSize, pageSize);
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                using (var reader = conn.QueryMultiple(sb.ToString()))
                {
                    long x = await reader.ReadFirstAsync<long>();
                    pagedList.TotalCount = Convert.ToInt32(x);
                    var list = await reader.ReadAsync<T>();
                    pagedList.Items = list.ToList();
                    return pagedList;
                }
            }

        }
    }
}
