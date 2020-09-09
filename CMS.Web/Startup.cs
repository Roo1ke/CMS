using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using CMS.Common;
using CMS.DTO;
using CMS.Model;
using CMS.Web.AutoFac;
using CMS.Web.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;

namespace CMS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //添加cors 服务 配置跨域处理            
            services.AddCors(options =>
            {
                options.AddPolicy("any", cors =>
                {
                    cors.AllowAnyOrigin() //允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            //序列化设置
            .AddJsonOptions(options =>
             {
                 //不使用驼峰样式的key
                 //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                 //设置时间格式
                 options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
             });
            //添加AutoMapper支持
            services.AddAutoMapper(typeof(AutoMapperConfig));
            #region JWT配置
            var jwtSetting = new JwtSetting();
            //Configuration.Bind("JwtSetting", jwtSetting);
            //services
            //  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //  .AddJwtBearer(options =>
            //  {
            //      options.TokenValidationParameters = new TokenValidationParameters
            //      {
            //          ValidIssuer = jwtSetting.Issuer,
            //          ValidAudience = jwtSetting.Audience,
            //          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Secret)),
            //          // 默认允许 300s  的时间偏移量，设置为0
            //          ClockSkew = TimeSpan.Zero
            //      };
            //  });

            // 令牌验证参数，之前我们都是写在AddJwtBearer里的，这里提出来了
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,//验证发行人的签名密钥
                IssuerSigningKey = key,
                ValidateIssuer = true,//验证发行人
                ValidIssuer = jwtSetting.Issuer,//发行人
                ValidateAudience = true,//验证订阅人
                ValidAudience = jwtSetting.Audience,//订阅人
                ValidateLifetime = true,//验证生命周期
                ClockSkew = TimeSpan.Zero,//这个是定义的过期的缓存时间
                RequireExpirationTime = true,//是否要求过期

            };
            var permission = new List<Sys_Menu>();
            // 角色与接口的权限要求参数
            var permissionRequirement = new PolicyRequirement(
                "/api/denied",// 拒绝授权的跳转地址（目前无用）
                permission,//这里还记得么，就是我们上边说到的角色地址信息凭据实体类 Permission
                ClaimTypes.Role,//基于角色的授权
                jwtSetting.Issuer,//发行人
                jwtSetting.Audience,//订阅人
                //signingCredentials,//签名凭据
                expiration: TimeSpan.FromSeconds(60 * 2)//接口的过期时间，注意这里没有了缓冲时间，你也可以自定义，在上边的TokenValidationParameters的 ClockSkew
                );
            // ① 核心之一，配置授权服务，也就是具体的规则，已经对应的权限策略，比如公司不同权限的门禁卡
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client",
                    policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin",
                    policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("SystemOrAdmin",
                    policy => policy.RequireRole("Admin", "System"));

                // 自定义基于策略的授权权限
                options.AddPolicy("Permission",
                         policy => policy.Requirements.Add(permissionRequirement));
            })
            // ② 核心之二，必需要配置认证服务，这里是jwtBearer默认认证，比如光有卡没用，得能识别他们
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // ③ 核心之三，针对JWT的配置，比如门禁是如何识别的，是放射卡，还是磁卡
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParameters;
            });


            // 依赖注入，将自定义的授权处理器 匹配给官方授权处理器接口，这样当系统处理授权的时候，就会直接访问我们自定义的授权处理器了。
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            // 将授权必要类注入生命周期内
            services.AddSingleton(permissionRequirement);

            #endregion
            #region 依赖注入
            //return RegisterAutofac(services);//注册Autofac
            var builder = new ContainerBuilder();//实例化容器
            //注册所有模块module
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            //获取所有的程序集
            //var assemblys = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            var assemblys = RuntimeHelper.GetAllAssemblies().ToArray();
            //注册所有继承IDependency接口的类
            builder.RegisterAssemblyTypes().Where(type => typeof(IDependency).IsAssignableFrom(type) && !type.IsAbstract);
            //注册仓储，所有IRepository接口到Repository的映射
            builder.RegisterAssemblyTypes(assemblys).Where(t => t.Name.EndsWith("Repository") && !t.Name.StartsWith("I")).AsImplementedInterfaces();
            //注册服务，所有IApplicationService到ApplicationService的映射
            builder.RegisterAssemblyTypes(assemblys).Where(t => t.Name.EndsWith("Service") && !t.Name.StartsWith("I")).AsImplementedInterfaces();
            builder.Populate(services);
            var ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer); //第三方IOC接管 core内置DI容器
                                                                     //return services.BuilderInterceptableServiceProvider(builder => builder.SetDynamicProxyFactory());
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            NLogBuilder.ConfigureNLog("NLog.config");
            //启用静态资源(无参默认wwwroot文件夹)
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            //请求错误提示配置
            app.UseErrorHandling();
            //使用认证授权
            app.UseAuthentication();
            //配置Cors
            app.UseCors("any");
            app.UseMvc();
        }

        private IServiceProvider RegisterAutofac(IServiceCollection services)
        {
            //实例化Autofac容器
            var builder = new ContainerBuilder();
            //将Services中的服务填充到Autofac中
            builder.Populate(services);
            //新模块组件注册    
            builder.RegisterModule<AutofacModuleRegister>();
            //创建容器
            var Container = builder.Build();
            //第三方IOC接管 core内置DI容器 
            return new AutofacServiceProvider(Container);
        }
    }
}
