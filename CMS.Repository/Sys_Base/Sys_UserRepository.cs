using CMS.Common;
using CMS.Common.DB;
using CMS.IRepository;
using CMS.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository
{
    class Sys_UserRepository : RepositoryBase<Sys_Users>, ISys_UserRepository
    {
        /// <summary>
        /// 保存账户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ResultMsg> SaveSys_User(Sys_Users user)
        {
            ResultMsg rs = new ResultMsg { Code = 0, Msg = "操作失败" };
            if (await CheckMobilePhone(user.PKID, user.MobilePhone))
            {
                rs.Msg = "该手机号码已存在";
                return rs;
            }
            user.PassWord = EncryptHelper.Encrypt(user.PassWord);
            user.Createtime = DateTime.Now;
            if (user.PKID == 0)
            {
                user.Status = (int)Sys_UserState_Enum.Normal;
                string insertSql = @"INSERT INTO Sys_Users (UserName, LoginName,Roles, PassWord, MobilePhone, DepID, ErrCount,Status,Createtime) VALUES(@UserName, @LoginName,@Roles, @PassWord, @MobilePhone, @DepID, @ErrCount,@Status,@Createtime)";
                if (await Insert(user, insertSql) > 0)
                {
                    rs.Code = 1;
                    rs.Msg = "操作成功";
                }
            }
            else {
                string updateSql = @"Update Sys_Users Set Roles=@Roles,UserName=@UserName,MobilePhone=@MobilePhone,HeaderImgUrl=@HeaderImgUrl Where PKID=@PKID";
                if (await Update(user, updateSql) > 0)
                {
                    rs.Code = 1;
                    rs.Msg = "操作成功";
                }  
            }
           
            return rs;
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condictions"></param>
        /// <returns></returns>
        public async Task<PagedList<Sys_Users>> GetUserPagedList(int pageIndex, int pageSize, string condictions)
        {
            var rs = await GetPagedList(pageIndex, pageSize, "Sys_Users", "*", condictions, "PKID");
            return rs;
        }
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="MobilePhone"></param>
        /// <returns></returns>
        public async Task<bool> CheckMobilePhone(int PKID, string MobilePhone)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                string querySql = @"SELECT count(*) FROM Sys_Users WHERE PKID<>@PKID And MobilePhone=@MobilePhone";
                return await conn.QueryFirstOrDefaultAsync<int>(querySql, new { PKID, MobilePhone }) > 0;
            }
        }

        /// <summary>
        /// 验证登录名
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="MobilePhone"></param>
        /// <returns></returns>
        public async Task<bool> CheckLoginName(int PKID, string loginName)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                string querySql = @"SELECT count(*) FROM Sys_Users WHERE PKID<>@PKID And loginName=@loginName";
                return await conn.QueryFirstOrDefaultAsync<int>(querySql, new { PKID, loginName }) > 0;
            }
        }
        /// <summary>
        /// 根据ID获取用户实体
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public async Task<ResultMsg> Get_UsersAsyncByPKID(int PKID)
        {
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                ResultMsg rs = new ResultMsg { Code = 0, Msg = "操作失败" };
                string querySql = @"SELECT PKID,DepID,Roles,loginName,UserName,MobilePhone,ErrCount,UnlockedTime,Status,Is_Locked,HeaderImgUrl FROM Sys_Users WHERE PKID=@PKID";
                var user = await conn.QueryFirstOrDefaultAsync<Sys_Users>(querySql, new { PKID });
                rs.Data = user;
                rs.Code = 1;
                rs.Msg = "操作成功";
                return rs;
            }
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<ResultMsg> UserLogin(string account, string pwd)
        {
            ResultMsg rs = new ResultMsg { Code = 0, Msg = "操作失败" };
            pwd = EncryptHelper.Encrypt(pwd);
            string querySql = @"SELECT PKID,Roles,DepID,loginName,UserName,MobilePhone,ErrCount,UnlockedTime,Status,Is_Locked,HeaderImgUrl From  Sys_Users where LoginName=@account AND PassWord=@pwd AND Status<>-1";
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                var userinfo = await conn.QueryFirstOrDefaultAsync<Sys_Users>(querySql, new { account, pwd });
                if (userinfo!=null)
                {
                    if (userinfo.ErrCount >= SystemConfig.MAX_ERR_COUNT && userinfo.Is_Locked == (int)Sys_UserLocked_Enum.Locked)
                    {
                        rs.Msg = $"该账户已被锁定，将于{userinfo.UnlockedTime.ToString("yyyy-MM-dd HH:mm")}解锁";
                        return rs;
                    }
                    if (userinfo.Status == (int)Sys_UserState_Enum.Forbidden)
                    {
                        rs.Msg = $"该账户已被禁用，请联系管理员";
                        return rs;
                    }
                    rs.Data = userinfo;
                    rs.Code = 1;
                    rs.Msg = "操作成功";
                    await ClearErrCount(userinfo.PKID);
                }
                else
                {
                    rs.Msg = "用户名或密码错误";
                    await AddErrCount(account);
                }
            }
            return rs;
        }


        public async Task<ResultMsg> ModifyPassword(int PKID, string oldPassword, string newPassword)
        {
            ResultMsg rs = new ResultMsg { Code = 0, Msg = "操作失败" };
            oldPassword = EncryptHelper.Encrypt(oldPassword);
            newPassword = EncryptHelper.Encrypt(newPassword);
            string updateSql = @"Update Sys_Users SET PassWord=@newPassword where PKID=@PKID and PassWord=@oldPassword";
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                if (await conn.ExecuteAsync(updateSql, new { oldPassword, newPassword,PKID }) > 0) {
                    rs.Code = 1;
                    rs.Msg = "操作成功";
                }
            }
            return rs;
        }

        /// <summary>
        /// 登录错误计数
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<bool> AddErrCount(string account)
        {
            bool rs = false;
            string updateSql = @"Update Sys_Users SET ErrCount=ErrCount+1 where loginName=@account";
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                rs = await conn.ExecuteAsync(updateSql, new { account }) > 0;
                string querySql = @"SELECT Errcount From Sys_Users WHERE loginName=@account";
                int errcount = await conn.ExecuteScalarAsync<int>(querySql, new { account });
                if (errcount == SystemConfig.MAX_ERR_COUNT)
                {
                    DateTime dt = DateTime.Now.AddMinutes(SystemConfig.LOCKED_TIME_MIN);
                    int status = (int)Sys_UserLocked_Enum.Locked;
                    updateSql = @"Update Sys_Users SET UnLockedTime=@dt,is_Locked=@status where loginName=@account";
                    await conn.ExecuteAsync(updateSql, new { account, dt, status });
                }
            }
            return rs;
        }

        /// <summary>
        /// 清空登录错误计数
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public async Task<bool> ClearErrCount(int PKID)
        {
            bool rs = false;
            int status = (int)Sys_UserLocked_Enum.UnLocked;
            string updateSql = @"Update Sys_Users SET ErrCount=0,is_Locked=status where PKID=@PKID";
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                rs = await conn.ExecuteAsync(updateSql, new { PKID, status }) > 0;
            }
            return rs;
        }

        public async Task<List<Sys_Menu>> GetUserPermission(int userid) {
            List<Sys_Menu> list = new List<Sys_Menu>();
            var rs = await Get_UsersAsyncByPKID(userid);
            var userinfo = (Sys_Users)rs.Data;
            var roleidsList = userinfo.Roles.Split(',');
            using (IDbConnection conn = DataBaseConfig.GetMySqlConnection())
            {
                var rolepermission = new List<int>();
                foreach (var roleid in roleidsList)
                {
                    string querySql = @"SELECT PermissionIDS FROM Sys_Roles WHERE PKID=@roleid And Status <>-1";
                    var permissionIDS = await conn.QueryFirstOrDefaultAsync<string>(querySql, new { roleid });
                    if (permissionIDS.Length > 0)
                    {
                        rolepermission = rolepermission.Concat(permissionIDS.Split(',').Select(x => Convert.ToInt32(x)).ToList()).ToList();
                    }
                }
                rolepermission = rolepermission.Where((x, i) => rolepermission.FindIndex(z => z == x ) == i).ToList();
                foreach (var menuid in rolepermission)
                {
                    string querySql = @"SELECT * FROM Sys_Menu WHERE PKID=@menuid And Status <>-1";
                    var info = await conn.QueryFirstOrDefaultAsync<Sys_Menu>(querySql, new { menuid });
                    list.Add(info);
                }
                return list;
            }

        }
    }
}
