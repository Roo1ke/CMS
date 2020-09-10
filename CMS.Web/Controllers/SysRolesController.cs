using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.IService;
using CMS.Model;
using CMS.Web.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize("Permission")]
    public class SysRolesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISys_RolesService _service;
        public SysRolesController(IMapper mapper, ISys_RolesService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpGet]
        [Route("GetPage")]
        [ParentPermission("/role", "查询")]
        public async Task<JsonResult> Get([FromQuery]int pageindex, [FromQuery]int pagesize, [FromQuery]int keywords)
        {
            var rs = await _service.GetRolePagedList(pageindex, pagesize, "status<>-1");
            return Json(rs);
        }


        [HttpGet]
        [Route("GetAllRoles")]
        [AllowAnonymous]
        public async Task<JsonResult> Get()
        {
            var rs = await _service.GetRolePagedList(1, 100, "status<>-1");
            return Json(rs.Items);
        }

        [HttpPost]
        [Route("Add")]
        [ParentPermission("/role", "新增&修改")]
        public async Task<JsonResult> PostAsync([FromBody]Sys_Roles value)
        {
            var rs = await _service.SaveSys_Roles(value);
            return Json(rs);
        }

        [HttpPost]
        [Route("SaveRolePerrmission")]
        [ParentPermission("/role", "权限设置")]
        public async Task<JsonResult> SaveRolePerrmission([FromBody]IDValueModel model)
        {
            var rs = await _service.SaveRolePerrmission(model.PKID,model.Values);
            return Json(rs);
        }
    }
}
