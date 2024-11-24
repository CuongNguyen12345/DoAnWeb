using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopGiay.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult DoanhThuSanPham()
        {
            return View();
        }

        public ActionResult QuanLySanPham()
        {
            return View();
        }

        public ActionResult Sidebar_Menu()
        {
            return PartialView();
        }
    }
}