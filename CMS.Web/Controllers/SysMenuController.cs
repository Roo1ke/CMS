using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.IService;
using CMS.Model;
using Microsoft.AspNetCore.Mvc;


namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class SysMenuController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISys_MenuService _service;
        public SysMenuController(IMapper mapper, ISys_MenuService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpGet]
        [Route("GetPage")]
        public async Task<JsonResult> Get([FromQuery]int pageindex, [FromQuery]int pagesize, [FromQuery]int keywords)
        {
            var rs = await _service.GetMenuPagedList(pageindex, pagesize, "a.status<>-1");
            return Json(rs);
        }

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var rs = await _service.GetParentMenus();
            return Json(rs);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<JsonResult> PostAsync([FromBody]Sys_Menu value)
        {
            var rs = await _service.SaveSys_Menu(value);
            return Json(rs);
        }

    }
}
