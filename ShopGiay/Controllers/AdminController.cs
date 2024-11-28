﻿using ShopGiay.Models;
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
    }
}