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
    public class UserController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: User
        public ActionResult Index()
        {
            var lstItem = db.SANPHAMs.Take(8).Select(p => p).ToList();
            return View(lstItem);
        }
        public ActionResult SanPhamBanChay()
        {
            var lstItem = db.SANPHAMs.OrderBy(p => p.GiaBan).Take(8).Select(p => p).ToList();
            return PartialView("SanPhamBanChay", lstItem);
        }

        public ActionResult Header()
        {
            return PartialView();
        }

        public ActionResult Slider()
        {
            return PartialView("Slider");
        }

        public ActionResult Footer()
        {
            return PartialView();
        }

        public ActionResult Product(int? page) 
        {
            int iSize = 6;
            int iPageNumber = (page ?? 1);
            var lstItem = db.SANPHAMs.ToList();
            return View(lstItem.ToPagedList(iPageNumber, iSize));
        }

        public ActionResult PhuKien()
        {
            return View();
        }
        
        public ActionResult ChinhSachHoanTra()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult ChonSize()
        {

            return View();
        }
    }
}