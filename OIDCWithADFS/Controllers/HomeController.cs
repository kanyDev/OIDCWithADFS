using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OIDCWithADFS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // 로그인
        [Authorize]
        public ActionResult Login()
        {
            return View();
        }

        // 로그아웃
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut(
                "Cookies",
                "OpenIdConnect");
            return Redirect("https://sts.vm.local/adfs/oauth2/logout");
        }

        // 오류 페이지
        public ActionResult Error()
        {
            return View();
        }
    }
}