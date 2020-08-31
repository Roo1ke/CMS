using CMS.Common.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Common
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<int> Insert(T entity, string insertSql);

        Task<int> Update(T entity, string updateSql);

        Task<int> Delete(int PKID, string deleteSql);

        Task<List<T>> Select(string selectSql);

        Task<T> Detail(int PKID, string detailSql);

        Task<PagedList<T>> GetPagedList(int pageIndex, int pageSize, string tableName, string files, string conditions, string orderby, object parameters = null);
    }

}
