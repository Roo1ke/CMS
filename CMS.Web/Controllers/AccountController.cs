using System;
using System.Collections.Generic;
using System.Linq;
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
                rs.Data = _mapper.Map<Sys_Users, Sys_UserDTO>(rs.Data);
            }
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    var modelstate = ModelState[key];
                    if (modelstate.Errors.Any())
                    {
                        rs.Msg += modelstate.Errors.FirstOrDefault().ErrorMessage+ ";";
                    }
                }
            }
            return Json(rs);
        }


    }
}
