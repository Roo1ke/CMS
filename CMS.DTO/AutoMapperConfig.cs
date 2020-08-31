using AutoMapper;
using CMS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.DTO
{
    public class AutoMapperConfig:Profile
    {
        /// <summary>
        /// 配置可以相互转换的类
        /// </summary>
        public AutoMapperConfig() {
            CreateMap<Sys_Users, Sys_UserDTO>();
            CreateMap<Sys_UserDTO, Sys_Users>();
        }
    }
}
