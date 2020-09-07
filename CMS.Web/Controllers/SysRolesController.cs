using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.IService;
using CMS.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<JsonResult> Get([FromQuery]int pageindex, [FromQuery]int pagesize, [FromQuery]int keywords)
        {
            var rs = await _service.GetRolePagedList(pageindex, pagesize, "status<>-1");
            return Json(rs);
        }


        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<JsonResult> Get()
        {
            var rs = await _service.GetRolePagedList(1, 100, "status<>-1");
            return Json(rs.Items);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<JsonResult> PostAsync([FromBody]Sys_Roles value)
        {
            var rs = await _service.SaveSys_Roles(value);
            return Json(rs);
        }

        [HttpPost]
        [Route("SaveRolePerrmission")]
        public async Task<JsonResult> SaveRolePerrmission([FromBody]IDValueModel model)
        {
            var rs = await _service.SaveRolePerrmission(model.PKID,model.Values);
            return Json(rs);
        }
    }
}
