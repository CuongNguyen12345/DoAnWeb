using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace ShopGiay.Controllers
{
    public class AdminController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: Admin
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult DoanhThuSanPham()
        {
            return View();
        }

        public ActionResult QuanLySanPham(int? Page)
        {
            Page = Page ?? 1;

            int size = 4;
            var listSP = (from sp in db.SANPHAMs
                         select sp).ToList();

            ViewBag.ThuongHieu = db.THUONGHIEUx.ToList();
            return View(listSP.ToPagedList((int)Page, size));
        }

        public ActionResult Sidebar_Menu()
        {
            return PartialView();
        }
    }
}