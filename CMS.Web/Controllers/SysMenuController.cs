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


namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize("Permission")]
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
        [ParentPermission("/menu", "查询")]
        public async Task<JsonResult> Get([FromQuery]int pageindex, [FromQuery]int pagesize, [FromQuery]int keywords)
        {
            var rs = await _service.GetMenuPagedList(pageindex, pagesize, "a.status<>-1");
            return Json(rs);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> Get()
        {
            var rs = await _service.GetParentMenus();
            return Json(rs);
        }

        [HttpPost]
        [Route("Add")]
        [ParentPermission("/menu", "新增&修改")]
        public async Task<JsonResult> PostAsync([FromBody]Sys_Menu value)
        {
            var rs = await _service.SaveSys_Menu(value);
            return Json(rs);
        }

        [HttpGet]
        [Route("InitTree")]
        [AllowAnonymous]
        public async Task<JsonResult> GetTreeList()
        {
            var res = await _service.GetMenuPagedList(1, 100, "a.status<>-1");
            List<MenusTreeModel> treeList = new List<MenusTreeModel>();
            var menuList = res.Items;
            #region 构造树形结构
            var One_list = menuList.Where(e => e.ParentID == 0).ToList();
            foreach (var item in One_list)
            {
                MenusTreeModel tree = new MenusTreeModel()
                {
                    id = item.PKID.ToString(),
                    label = item.MenuName,
                    children = new List<MenusTreeModel>()
                };
                var two_list = menuList.Where(e => e.ParentID == item.PKID).ToList();
                foreach (var _item in two_list)
                {
                    MenusTreeModel _tree = new MenusTreeModel()
                    {
                        id = _item.PKID.ToString(),
                        label = _item.MenuName,
                        children=new List<MenusTreeModel>()
                    };
                    var operation = await _service.GetOperation(Convert.ToInt32(_tree.id));
                    foreach (var op in operation)
                    {
                        _tree.children.Add(new MenusTreeModel
                        {
                            id= _item.PKID.ToString()+"_"+ op.PKID.ToString(),
                            label=op.OperationName
                        });
                    }
                    tree.children.Add(_tree);
                }
                treeList.Add(tree);
            }
            #endregion
            return Json(treeList);
        }

        [HttpDelete("{id}")]
        [ParentPermission("/menu", "删除")]
        public async Task<JsonResult> Delete(int id)
        {
            var rs = await _service.DeleteSys_Menu(id);
            return Json(rs);
        }

    }
}
