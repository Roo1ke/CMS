using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.Common;
using CMS.Common.DB;
using CMS.DTO;
using CMS.IService;
using CMS.Model;
using CMS.Web.JWT;
using CMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize("Permission")]
    public class SystemUserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISys_UserService _service;
        public SystemUserController(IMapper mapper, ISys_UserService service)
        {
            _mapper = mapper;
            _service = service;
        }
        // GET: api/<controller>
        [HttpGet]
        [Route("GetPage")]
        [ParentPermission("/usermanage","查询")]
        public async Task<JsonResult> Get([FromQuery]int pageindex, [FromQuery]int pagesize, [FromQuery]int keywords)
        {
            var rs = await _service.GetUserPagedList(pageindex, pagesize, "status<>-1");
            var pagelist = new PagedList<Sys_UserDTO>()
            {
                TotalCount = rs.TotalCount,
                Items = new List<Sys_UserDTO>()
            };
            rs.Items.ForEach(e =>
            {
                var userDTO = _mapper.Map<Sys_Users, Sys_UserDTO>(e);
                pagelist.Items.Add(userDTO);
            });
            return Json(pagelist);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<JsonResult> Get(int id)
        {
            var rs = await _service.Get_UsersAsyncByPKID(id);
            return Json(rs);
        }

        // POST api/<controller>
        [HttpPost]
        [ParentPermission("/usermanage", "新增&修改")]
        public async Task<JsonResult> PostAsync([FromBody]Sys_UserDTO value)
        {
            var user = _mapper.Map<Sys_UserDTO, Sys_Users>(value);
            user.PassWord = user.PassWord ?? SystemConfig.DEAFULT_PWD;
            var rs = await _service.SaveSys_User(user);
            return Json(rs);
        }

        [HttpPost]
        [Route("CheckMobilePhone")]
        [ParentPermission("/usermanage", "新增&修改")]
        public async Task<bool> CheckMobilePhone([FromBody]IDValueModel model)
        {
            var rs = await _service.CheckMobilePhone(model.PKID, model.Values);
            return rs;
        }

        [HttpPost]
        [Route("CheckLoginName")]
        [ParentPermission("/usermanage", "新增&修改")]
        public async Task<bool> CheckLoginName([FromBody]IDValueModel model)
        {
            var rs = await _service.CheckLoginName(model.PKID, model.Values);
            return rs;
        }

        [HttpPost]
        [Route("ModifyPassword")]
        [AllowAnonymous]
        public async Task<JsonResult> ModifyPassword([FromBody]ModifyPwdModel model)
        {
            var rs = new ResultMsg() { Code = 0 };
            if (!ModelState.IsValid) {
                foreach (var key in ModelState.Keys)
                {
                    var modelstate = ModelState[key];
                    if (modelstate.Errors.Any())
                    {
                        rs.Msg += modelstate.Errors.FirstOrDefault().ErrorMessage + ";";
                    }
                }
                return Json(rs);
            }
            if (model.NewPassword != model.Confirm_NewPassword)
            {
                rs.Msg = "两次输入的密码不一致";
                return Json(rs);
            }
            rs = await _service.ModifyPassword(model.PKID, model.OldPassword,model.NewPassword);
            return Json(rs);
        }


        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [ParentPermission("/usermanage", "删除")]
        public async Task<JsonResult> Delete(int id)
        {
            var rs = await _service.DeleteUser(id);
            return Json(rs);
        }
    }
}
