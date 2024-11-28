using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ShopGiay.Controllers
{
    public class GioHangController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        public ActionResult ThemGioHang(int id, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Find(n => n.iMaGiay == id);
            if (sp == null)
            {
                sp = new GioHang(id);
                lstGioHang.Add(sp);
            }
            else
            {
                sp.iSoLuong++;
            }
            return Redirect(url);
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        private decimal TongTien()
        {
            decimal dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => (decimal)n.dThanhTien);
            }
            return dTongTien;
        }

        [HttpGet]
        public ActionResult GioHang()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("DangNhap", "User");
            }
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "ShopGiay");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        public ActionResult XoaSPKhoiGioHang(int iMaGiay)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaGiay == iMaGiay);
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iMaGiay == iMaGiay);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "ShopGiay");
                }
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "ShopGiay");
        }

        public ActionResult CapNhatGioHang(int iMaGiay, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaGiay == iMaGiay);
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "ShopGiay");
            }

            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            DONHANG ddh = new DONHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List<GioHang> lstGioHang = LayGioHang();

            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrangGiaoHang = true;
            ddh.TongTien = TongTien();
            //ddh.DaThanhToan = false;

            GuiEmailXacNhan(kh.EmailKH, ddh);
            db.DONHANGs.Add(ddh);
            db.SaveChanges();

            foreach (var item in lstGioHang)
            {
                CT_DONHANG ctdh = new CT_DONHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaGiay = item.iMaGiay;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.ThanhTien = (decimal)item.dDonGia;
                db.CT_DONHANG.Add(ctdh);
            }
            db.SaveChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        private void GuiEmailXacNhan(string email, DONHANG order)
        {
            string username = "Quynhnhuvh19@gmail.com";
            string password = "cnjj hlij byjd hclu";

            try
            {
                string body = $@"
<div style='font-family:Arial, sans-serif; max-width:600px; margin:auto; border:1px solid #ddd; border-radius:8px; overflow:hidden; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);'>
    <div style='background: linear-gradient(45deg, #007bff, #00c6ff); padding:25px; text-align:center; color:white;'>
        <h2 style='font-size:28px; margin-bottom:10px;'>Xác nhận đơn hàng</h2>
        <p style='font-size:16px; margin:0;'>Cảm ơn bạn đã đặt hàng tại <b>Shop Giay</b>!</p>
    </div>
    <div style='padding:30px; background-color:#fafafa;'>
        <h3 style='font-size:22px; color:#333; margin-bottom:15px;'>Chi tiết đơn hàng</h3>
        <table style='width:100%; border-collapse:collapse;'>
            <thead>
                <tr style='=color:#555;'>
                    <th style='background: linear-gradient(45deg, #007bff, #00c6ff); padding:12px; border-bottom:2px solid #fff;'>STT</th>
                    <th style='background: linear-gradient(45deg, #007bff, #00c6ff); padding:12px; border-bottom:2px solid #fff;'>Tên Sản Phẩm</th>
                    <th style='background: linear-gradient(45deg, #007bff, #00c6ff); padding:12px; border-bottom:2px solid #fff;'>Số Lượng</th>
                    <th style='background: linear-gradient(45deg, #007bff, #00c6ff); padding:12px; border-bottom:2px solid #fff;'>Đơn Giá</th>
                    <th style='background: linear-gradient(45deg, #007bff, #00c6ff); padding:12px; border-bottom:2px solid #fff;'>Thành Tiền</th>
                </tr>
            </thead>
            <tbody>";
                int stt = 1;
                foreach (var item in LayGioHang())
                {

                    body += $@"
                <tr style='background-color:#fff; border-bottom:1px solid #eee;'>
                    <td style='padding:10px; text-align:center; color:#333;'>{stt++}</td>
                    <td style='padding:10px; color:#333;'>{item.sTenGiay}</td>
                    <td style='padding:10px; text-align:center; color:#333;'>{item.iSoLuong}</td>
                    <td style='padding:10px; text-align:right; color:#333;'>{item.dDonGia:#,##0} VNĐ</td>
                    <td style='padding:10px; text-align:right; color:#333;'>{item.dThanhTien:#,##0} VNĐ</td>
                </tr>";
                }

                body += $@"
            </tbody>
        </table>
        <div style='text-align:right; margin-top:20px; font-size:16px; color:#333;'>
            <p><b>Tổng tiền:</b> {TongTien():#,##0} VNĐ</p>
        </div>
        <p style='margin-top:20px; font-size:14px; color:#666;'> Cảm ơn bạn đã mua sản phẩm, nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: 
            <a href='mailto:Quynhnhuvh19@gmail.com' style='color:#4CAF50; text-decoration:none;'>DevNguyen@gmail.com</a>.
        </p>
        <p style='margin-top:15px; font-size:14px; color:#666;'>Trân trọng,<br/><b>Giày chất lượng cao</b></p>
    </div>
    <div style='background: linear-gradient(45deg, #007bff, #00c6ff); padding:15px; text-align:center; color:white;'>
        <p style='margin:0; font-size:14px;'>&copy; 2024 Shop Giày. Dev Nguyen.</p>
    </div>
</div>";


                // Cấu hình email
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(username);
                mail.To.Add(email);
                mail.Subject = "Xác Nhận Đơn Hàng";
                mail.Body = body;
                mail.IsBodyHtml = true;
                // Cấu hình SMTP
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(username, password);
                smtp.EnableSsl = true;

                // Gửi email
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gửi email thất bại: " + ex.Message);
            }
        }
        [HttpGet]
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }
    }
}