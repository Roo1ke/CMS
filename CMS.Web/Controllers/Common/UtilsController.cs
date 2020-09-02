using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class UtilsController : Controller
    {
        private IHostingEnvironment hostingEnvironment;
        string[] pictureFormatArray = { "png", "jpg", "jpeg", "PNG", "JPG", "JPEG" };
        public UtilsController(IHostingEnvironment env)
        {
            this.hostingEnvironment = env;
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload()
        {
            try
            {
                var files = Request.Form.Files;
                long size = files.Sum(f => f.Length);
                if (size > 104857600)
                {
                    return Json("图片总大小不能超过100M!");
                }

                List<string> filePathResultList = new List<string>();

                foreach (var file in files)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                    string filePath = hostingEnvironment.WebRootPath + $@"\Imgs\UserHeader\";

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    var arr = fileName.Split('.');
                    string suffix = arr[arr.Length - 1];

                    if (!pictureFormatArray.Contains(suffix))
                    {
                        return Json("仅支持 'png','jpg','jpeg' 类型文件");
                    }

                    fileName = Guid.NewGuid() + "." + suffix;

                    string fileFullName = filePath + fileName;

                    using (FileStream fs = System.IO.File.Create(fileFullName))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                    filePathResultList.Add($"/Imgs/UserHeader/{fileName}");
                }
                return Json(filePathResultList[0]);
            }
            catch (Exception ex)
            {
                return Json("上传失败," + ex.Message);
            }
        }
    }
}
