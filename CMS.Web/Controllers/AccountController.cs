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
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ISys_UserService _service;
        public AccountController(IMapper mapper, ISys_UserService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpPost]
        [Route("login")]
        public async Task<JsonResult> Login([FromBody]LoginModel model)
        {
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
                    var One_list = menus.Where(e => e.ParentID == 0).ToList();
                    foreach (var item in One_list)
                    {
                        MenusTreeModel tree = new MenusTreeModel()
                        {
                            id = item.PKID,
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
                                id = _item.PKID,
                                path = _item.Path,
                                icon = _item.Icon,
                                label = _item.MenuName,
                            };
                            tree.children.Add(_tree);
                        }
                        treeList.Add(tree);
                    }
                    #endregion
                    info.menus = treeList;
                    info.permission = menus;
                    var claims = new[] {
                      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                      new Claim("userinfo", Newtonsoft.Json.JsonConvert.SerializeObject(info)), // 用户信息
                    };
                    var token = JWT.JwtUtil.CreateToken(claims);
                    rs.Data = token;
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


    }
}
