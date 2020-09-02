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
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<JsonResult> PostAsync([FromBody]Sys_UserDTO value)
        {
            var user = _mapper.Map<Sys_UserDTO, Sys_Users>(value);
            user.PassWord = user.PassWord ?? SystemConfig.DEAFULT_PWD;
            var rs = await _service.SaveSys_User(user);
            return Json(rs);
        }

        [HttpPost]
        [Route("CheckMobilePhone")]
        public async Task<bool> CheckMobilePhone([FromQuery]int PKID,string mobilephone)
        {
            var rs = await _service.CheckMobilePhone(PKID, mobilephone);
            return rs;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
