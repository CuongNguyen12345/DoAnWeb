using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Drawing.Printing;
using System.Data.Entity;
using System.IO;

namespace ShopGiay.Controllers
{
    public class AdminController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        private static int MaSPGolbal = 0;
        // GET: Admin
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult DoanhThuSanPham()
        {
            return View();
        }

        [HttpGet]
        public JsonResult DoanhThu()
        {
            var doanhThuTheoThang = (from donHang in db.DONHANGs
                                     group donHang by new { Month = donHang.NgayDat.Month } into g
                                     select new
                                     {
                                         Thang = g.Key.Month,
                                         DoanhThu = g.Sum(d => d.TongTien)
                                     })
                                     .OrderBy(o => o.Thang)
                                     .ToList();
            return Json(doanhThuTheoThang, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult QuanLySanPham(int? Page)
        {
            Page = Page ?? 1;
            string tenSP = Request.QueryString["TenSP"];
            int MaTT = string.IsNullOrEmpty(Request.QueryString["MaTT"]) ? 0 : int.Parse(Request.QueryString["MaTT"]);
            int giaTu = string.IsNullOrEmpty(Request.QueryString["GiaTu"]) ? 0 : int.Parse(Request.QueryString["GiaTu"]);
            int giaDen = string.IsNullOrEmpty(Request.QueryString["GiaDen"]) ? 0 : int.Parse(Request.QueryString["GiaDen"]);
            int soLuongTu = string.IsNullOrEmpty(Request.QueryString["SoLuongTu"]) ? 0 : int.Parse(Request.QueryString["SoLuongTu"]);
            int soLuongDen = string.IsNullOrEmpty(Request.QueryString["SoLuongDen"]) ? 0 : int.Parse(Request.QueryString["SoLuongDen"]);
            string trangThai = string.IsNullOrEmpty(Request.QueryString["TrangThai"]) ? "" : Request.QueryString["TrangThai"];

            int size = 4;
            var query = db.SANPHAMs.AsQueryable();
            if(!string.IsNullOrEmpty(tenSP))
            {
                query = query.Where(w => w.TenGiay.Contains(tenSP));
            }
            if(MaTT != 0)
            {
                query = query.Where(w => w.MaThuongHieu == MaTT);
            }
            if(giaTu != 0)
            {
                query = query.Where(w => w.GiaBan >= giaTu);
            }
            if (giaDen != 0)
            {
                query = query.Where(w => w.GiaBan <= giaDen);
            }
            if (soLuongTu != 0)
            {
                query = query.Where(w => w.SoLuongTon >= soLuongTu);
            }
            if (soLuongDen != 0)
            {
                query = query.Where(w => w.SoLuongTon <= soLuongDen);
            }
            if(!string.IsNullOrEmpty(trangThai))
            {
                bool tt = bool.Parse(trangThai);
                query = query.Where(w => w.TrangThai == tt);
            }

            var listSP = query.OrderBy(sp => sp.MaGiay)
                              .ToList();
            ViewBag.ThuongHieu = db.THUONGHIEUx.ToList();

            return View(listSP.ToPagedList((int)Page, size));
        }

        [HttpGet]
        public ActionResult ThemSanPham() {
            ViewBag.ThuongHieu = db.THUONGHIEUx.ToList();
            ViewBag.NhaCungCap = db.NHACUNGCAPs.Where(w => w.TrangThai == true).ToList();
            return View(); 
        }
        [HttpPost]
        public ActionResult ThemSanPham(FormCollection f, HttpPostedFileBase HinhAnhFiles)
        {
            string tenSP = f["TenSP"];
            int MaTT = int.Parse(f["MaTT"]);
            int gia = int.Parse(f["Gia"]);
            byte size = byte.Parse(f["Size"]);
            int MaNCC = int.Parse(f["MaNCC"]);
            int soLuong = int.Parse(f["soLuong"]);
            var file = HinhAnhFiles;

            // Tạo tên file mới để tránh trùng lặp
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string newFileName = fileName + "_" + Guid.NewGuid() + extension;

            // Đường dẫn lưu file
            string path = Path.Combine(Server.MapPath("~/Images/"), newFileName);

            // Lưu file lên server
            file.SaveAs(path);

            // Lưu tên file mới vào thuộc tính của đối tượng SANPHAM
            //sanpham.HINH_ANH = newFileName;


            SANPHAM sp = new SANPHAM();
            sp.TenGiay = tenSP;
            sp.Size = size;
            sp.AnhBia = newFileName;
            sp.GiaBan = gia;
            sp.MaThuongHieu = MaTT;
            sp.TrangThai = true;
            sp.MaNCC = MaNCC;
            sp.ThoiGianBaoHanh = 12;
            sp.NgayCapNhat = DateTime.Now;
            sp.SoLuongTon = soLuong;

            db.SANPHAMs.Add(sp);
            db.SaveChanges();
            return RedirectToAction("ThemSanPham", "Admin");
        }

        [HttpGet]
        public ActionResult SuaSanPham(int MaSP)
        {
            var item = db.SANPHAMs.FirstOrDefault(f => f.MaGiay == MaSP);
            MaSPGolbal = MaSP;

            ViewBag.ThuongHieu = db.THUONGHIEUx.ToList();
            ViewBag.NhaCungCap = db.NHACUNGCAPs.Where(w => w.TrangThai == true).ToList();
            return View(item);
        }

        [HttpPost]
        public ActionResult SuaSanPham(FormCollection f, HttpPostedFileBase HinhAnhFiles)
        {
            string tenSP = f["TenSP"];
            int MaTT = int.Parse(f["MaTT"]);
            int gia = int.Parse(f["Gia"]);
            byte size = byte.Parse(f["Size"]);
            int MaNCC = int.Parse(f["MaNCC"]);
            int soLuong = int.Parse(f["soLuong"]);
            var file = HinhAnhFiles;

            // Tạo tên file mới để tránh trùng lặp
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string newFileName = fileName + "_" + Guid.NewGuid() + extension;

            // Đường dẫn lưu file
            string path = Path.Combine(Server.MapPath("~/Images/"), newFileName);

            // Lưu file lên server
            file.SaveAs(path);

            // Lưu tên file mới vào thuộc tính của đối tượng SANPHAM
            //sanpham.HINH_ANH = newFileName;

            SANPHAM sp = db.SANPHAMs.SingleOrDefault(s => s.MaGiay == MaSPGolbal);
            sp.TenGiay = tenSP;
            sp.Size = size;
            sp.AnhBia = newFileName;
            sp.GiaBan = gia;
            sp.MaThuongHieu = MaTT;
            sp.TrangThai = true;
            sp.MaNCC = MaNCC;
            sp.ThoiGianBaoHanh = 12;
            sp.NgayCapNhat = DateTime.Now;
            sp.SoLuongTon = soLuong;

            db.SaveChanges();
            return RedirectToAction("SuaSanPham", "Admin", new {MaSP = MaSPGolbal});
        }
        public ActionResult XoaSanPham(int MaSP)
        {
            var item = db.SANPHAMs.FirstOrDefault(f => f.MaGiay == MaSP);
            item.TrangThai = false;
            db.SaveChanges();

            return RedirectToAction("QuanLySanPham", "Admin");
        }

        public ActionResult HienSanPham(int MaSP)
        {
            var item = db.SANPHAMs.FirstOrDefault(f => f.MaGiay == MaSP);
            item.TrangThai = true;
            db.SaveChanges();

            return RedirectToAction("QuanLySanPham", "Admin");
        }
        public ActionResult Sidebar_Menu()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult QuanLyVoucher()
        {
            var list = db.KHUYENMAIs.ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult ThemVoucher()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemVoucher(FormCollection f)
        {
            string tenKM = f["TenKM"];
            string phanTramGiam = f["PhanTramGiam"];
            DateTime ngayBD = DateTime.Parse(f["NgayBatDau"]);
            DateTime ngayKT = DateTime.Parse(f["NgayKetThuc"]);

            KHUYENMAI km = new KHUYENMAI();
            km.TenKM = tenKM;
            //km.PhanTramGiam = phanTramGiam;
            km.NgayBatDau = ngayBD;
            km.NgayKetThuc = ngayKT;
            db.KHUYENMAIs.Add(km);

            db.SaveChanges();
            return RedirectToAction("ThemVoucher", "Admin");
        }

        public ActionResult QuanLyDonHang()
        {

            ViewBag.lstDH = (from kh in db.KHACHHANGs
                        join dh in db.DONHANGs on kh.MaKH equals dh.MaKH
                        join ctdh in db.CT_DONHANG on dh.MaDonHang equals ctdh.MaDonHang
                        group ctdh by new { kh.MaKH, kh.HoTen, dh.MaDonHang, dh.TongTien } into g
                        select new ViewBagModel
                        {
                            MaKH = g.Key.MaKH,
                            TenKH = g.Key.HoTen,
                            MaDH = g.Key.MaDonHang,
                            SoLuongSP = g.Count(),
                            TongTien = g.Key.TongTien ?? 0
                        }).ToList();

            return View();
        }

        public ActionResult ChiTietDonHang() //Làm tiếp
        {
            return View();
        }

        public ActionResult CSKH()
        {
            if (Session["MaQL"] == null)
            {
                return RedirectToAction("DangNhap", "User");
            }

            var conversations = db.TINNHANs
                                  .OrderByDescending(o => o.MaTinNhan)
                                  .ToList();

            return View(conversations);
        }

        public ActionResult ChatDetail(int maKH)
        {
            var khachHang = db.KHACHHANGs.Find(maKH);
            var messages = db.TINNHANs
                .Where(t => t.MaKH == maKH)
                .OrderByDescending(t => t.ThoiGianGui)
                .ToList();

            // Đánh dấu tin nhắn đã đọc
            var unreadMessages = messages.Where(m => !m.DaDoc && m.MaQL == null);
            foreach (var message in unreadMessages)
            {
                message.DaDoc = true;
            }
            db.SaveChanges();

            ViewBag.KhachHang = khachHang;
            return View(messages);
        }

        [HttpPost]
        public ActionResult TraLoiTinNhan(int maKH, string noiDung)
        {
            int MaQL = (int)Session["MaQL"];
            try
            {
                var quanLy = db.QUANLies.FirstOrDefault(q => q.MaQL == MaQL);

                var tinNhan = new TINNHAN
                {
                    MaKH = maKH,
                    MaQL = quanLy.MaQL,
                    NoiDung = noiDung,
                    ThoiGianGui = DateTime.Now,
                    DaDoc = true
                };

                db.TINNHANs.Add(tinNhan);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}