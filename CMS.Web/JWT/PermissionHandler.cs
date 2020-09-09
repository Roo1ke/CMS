using CMS.Common;
using CMS.DTO;
using CMS.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Web.JWT
{
    public class PermissionHandler : AuthorizationHandler<PolicyRequirement>
    {
        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }

        /// <summary>
        /// services 层注入
        /// </summary>
        public ISys_UserService _service { get; set; }

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="schemes"></param>
        /// <param name="roleModulePermissionServices"></param>
        public PermissionHandler(IAuthenticationSchemeProvider schemes, ISys_UserService service)
        {
            Schemes = schemes;
            _service = service;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            bool haspermission = false;
            //从AuthorizationHandlerContext转成HttpContext，以便取出表头信息
            var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext).HttpContext;
            var pAttrs = (context.Resource as AuthorizationFilterContext).Filters.Where(e => (e as ParentPermissionAttribute) != null).ToList();
            //请求Url
            var questUrl = httpContext.Request.Path.Value.ToLower();
            //判断请求是否停止
            var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                {
                    context.Fail();
                    return;
                }
            }
            //判断请求是否拥有凭据，即有没有登录
            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                //result?.Principal不为空即登录成功
                if (result?.Principal != null)
                {
                    httpContext.User = result.Principal;
                    //权限中是否存在请求的url
                   // if (requirement.Permissions.GroupBy(g => g.Path).Where(w => w.Key?.ToLower() == questUrl).Count() > 0)
                   // {
                        // 获取当前用户的角色信息
                        var currentUserRoles = (from item in httpContext.User.Claims
                                                where item.Type == requirement.ClaimType
                                                select item.Value).ToList();
                    // 获取当前用户的信息
                    var userinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Sys_UserDTO>((from item in httpContext.User.Claims
                                    where item.Type == "userinfo"
                                    select item.Value).FirstOrDefault());
                    //验证权限
                    foreach (ParentPermissionAttribute pattr  in pAttrs)
                    {
                        if (await _service.ValidUserPermission(userinfo.PKID,pattr.Route,pattr.OperationType)) {
                            haspermission = true;
                            break ;
                        }
                    }
                    if (!haspermission) {
                        context.Fail();
                        return;
                    }
                    //判断过期时间
                    if ((httpContext.User.Claims.SingleOrDefault(s => s.Type == "exp")?.Value) != null && DateTimeHelper.ConvertToDateTime(httpContext.User.Claims.SingleOrDefault(s => s.Type == "exp")?.Value) >= DateTime.Now)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                        return;
                    }
                    return;
                }
            }
        }
    }

    public class ParentPermissionAttribute : Attribute, IFilterMetadata
    {
    
        public string  Route { get; set; }
        public string OperationType { set; get; }

        public ParentPermissionAttribute(string route, string operationType)
        {
            Route = route;
            OperationType = operationType;
        }
    }
}
