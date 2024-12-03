using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopGiay.Controllers
{
    public class CHATController : Controller
    {
        private ShopGiayEntities db = new ShopGiayEntities();

        public ActionResult ChatBox()
        {
            List<TINNHAN> list = new List<TINNHAN>();
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("DangNhap", "User");
            }
            else
            {
                var khachHang = (KHACHHANG)Session["Taikhoan"];
                list = db.TINNHANs
                    .Where(t => t.MaKH == khachHang.MaKH)
                    .OrderBy(t => t.ThoiGianGui)
                    .ToList();
            }


            return PartialView("_ChatBox", list);
        }

        [HttpPost]
        public ActionResult GuiTinNhan(string noiDung)
        {
            if (Session["Taikhoan"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để gửi tin nhắn" });
            }

            try
            {
                var khachHang = (KHACHHANG)Session["Taikhoan"];
                var tinNhan = new TINNHAN
                {
                    MaKH = khachHang.MaKH,
                    NoiDung = noiDung,
                    ThoiGianGui = DateTime.Now,
                    DaDoc = false
                };

                db.TINNHANs.Add(tinNhan);
                db.SaveChanges();

                return Json(new { success = true, message = "Gửi tin nhắn thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi gửi tin nhắn: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult LayTinNhanMoi(DateTime? lastMessageTime)
        {
            if (Session["Taikhoan"] == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);
            }

            var khachHang = (KHACHHANG)Session["Taikhoan"];
            var tinNhanMoi = db.TINNHANs
                .Where(t => t.MaKH == khachHang.MaKH && t.ThoiGianGui > lastMessageTime)
                .OrderBy(t => t.ThoiGianGui)
                .ToList();

            return Json(new { success = true, messages = tinNhanMoi }, JsonRequestBehavior.AllowGet);
        }
        // GET: CHAT
        public ActionResult Index()
        {
            return View();
        }


    }
}