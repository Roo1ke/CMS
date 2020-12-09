using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CMS.DTO;
using CMS.IService;
using CMS.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ISys_UserService _service;
        private readonly ILogger<AccountController> _log;
        private readonly IHttpContextAccessor _accessor;
        public AccountController(IMapper mapper, ISys_UserService service, ILogger<AccountController> log,IHttpContextAccessor accessor)
        {
            _mapper = mapper;
            _service = service;
            _log = log;
            _accessor = accessor;
        }

        [HttpPost]
        [Route("login")]
        public async Task<JsonResult> Login([FromBody]LoginModel model)
        { 
            // 获取客户端的IP
            string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            _log.LogError(@"用户{0}登录，登录IP：{1}", model.account, ip);
            _log.LogInformation(@"用户{0}登录，登录IP：{1}", model.account, ip);
            ResultMsg rs = new ResultMsg();
            if (ModelState.IsValid)
            {
                rs = await _service.UserLogin(model.account, model.pwd);
                if (rs.Code == 1)
                {
                    var info = _mapper.Map<Sys_Users, Sys_UserDTO>(rs.Data) as Sys_UserDTO;
                    var menus = await _service.GetUserPermission(info.PKID);
                    List<MenusTreeModel> treeList = new List<MenusTreeModel>();
                    #region 构造树形结构
                    CreateTreeModel(menus, treeList);
                    #endregion
                    info.menus = treeList;
                    info.permission = menus;
                    var claims = new[] {
                      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                      new Claim("userinfo", Newtonsoft.Json.JsonConvert.SerializeObject(info)), // 用户信息
                    };
                    var token = JWT.JwtUtil.CreateToken(claims);
                    rs.Data = token;
                   
                    _log.LogInformation(@"用户{0}登录成功，登录IP：{1}",info.UserName,ip);
                }
            }
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    var modelstate = ModelState[key];
                    if (modelstate.Errors.Any())
                    {
                        rs.Msg += modelstate.Errors.FirstOrDefault().ErrorMessage + ";";
                    }
                }
            }
            return Json(rs);
        }

        private static void CreateTreeModel(List<Sys_Menu> menus, List<MenusTreeModel> treeList)
        {
            var One_list = menus.Where(e => e.ParentID == 0).ToList();
            foreach (var item in One_list)
            {
                MenusTreeModel tree = new MenusTreeModel()
                {
                    id = item.PKID.ToString(),
                    label = item.MenuName,
                    path = item.Path,
                    icon = item.Icon,
                    children = new List<MenusTreeModel>()
                };
                var two_list = menus.Where(e => e.ParentID == item.PKID).ToList();
                foreach (var _item in two_list)
                {
                    MenusTreeModel _tree = new MenusTreeModel()
                    {
                        id = _item.PKID.ToString(),
                        path = _item.Path,
                        icon = _item.Icon,
                        label = _item.MenuName,
                    };
                    tree.children.Add(_tree);
                }
                treeList.Add(tree);
            }
        }
    }
}
