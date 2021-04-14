using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using Blog.Controllers;

namespace Blog.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : BlogControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
