using ShopGiay.Models;
using ShopGiay.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ShopGiay.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ShopGiayEntities db;
        private readonly KhuyenMaiService khuyenMaiService;

        public GioHangController()
        {
            db = new ShopGiayEntities();
            khuyenMaiService = new KhuyenMaiService(db);
        }

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
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "ShopGiay");
            }
            ViewBag.DanhSachKhuyenMai = khuyenMaiService.GetAllValidKhuyenMais();
            ViewBag.KhuyenMaiDangDung = Session["KhuyenMai"] as KHUYENMAI;

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.TongGiamGia = lstGioHang.Sum(sp => sp.GiamGia);
            ViewBag.TongTienSauGiam = lstGioHang.Sum(sp => sp.ThanhTienSauGiam);

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
            ViewBag.TongGiamGia = lstGioHang.Sum(sp => sp.GiamGia);
            ViewBag.TongTienSauGiam = lstGioHang.Sum(sp => sp.ThanhTienSauGiam);

            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            string paymentMethod = f["thanhtoan"];
            string bankCode = f["bankCode"];
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);

            if (paymentMethod == "vnpay")
            {
                // Redirect to VNPay payment with selected bank
                return RedirectToAction("PaymentVNPay", new { ngayGiao = NgayGiao, bankCode = bankCode });
            }
            else if (paymentMethod == "bidv")
            {
                // Lưu đơn hàng với trạng thái chờ thanh toán
                DONHANG ddh = new DONHANG();
                KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
                List<GioHang> lstGioHang = LayGioHang();

                ddh.MaKH = kh.MaKH;
                ddh.NgayDat = DateTime.Now;
                ddh.NgayGiao = DateTime.Parse(NgayGiao);
                ddh.TinhTrangGiaoHang = false;
                ddh.DaThanhToan = 0;

                db.DONHANGs.Add(ddh);
                db.SaveChanges();

                foreach (var item in lstGioHang)
                {
                    CT_DONHANG ctdh = new CT_DONHANG();
                    ctdh.MaDonHang = ddh.MaDonHang;
                    ctdh.MaGiay = item.iMaGiay;
                    ctdh.SoLuong = item.iSoLuong;
                    ctdh.DonGia = (decimal)item.dDonGia;
                    db.CT_DONHANG.Add(ctdh);
                }
                db.SaveChanges();

                // Chuyển hướng đến trang hướng dẫn thanh toán BIDV
                return RedirectToAction("PaymentBIDV", new { ngayGiao = NgayGiao }); ;
            }
            else
            {
                // Original cash on delivery logic
                DONHANG ddh = new DONHANG();
                KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
                List<GioHang> lstGioHang = LayGioHang();

                ddh.MaKH = kh.MaKH;
                ddh.NgayDat = DateTime.Now;
                ddh.NgayGiao = DateTime.Parse(NgayGiao);
                ddh.TinhTrangGiaoHang = false;
                ddh.DaThanhToan = 0;

                GuiEmailXacNhan(kh.EmailKH, ddh);
                db.DONHANGs.Add(ddh);
                db.SaveChanges();


                foreach (var item in lstGioHang)
                {
                    CT_DONHANG ctdh = new CT_DONHANG();
                    ctdh.MaDonHang = ddh.MaDonHang;
                    ctdh.MaGiay = item.iMaGiay;
                    ctdh.SoLuong = item.iSoLuong;
                    ctdh.DonGia = (decimal)item.dDonGia;
                    db.CT_DONHANG.Add(ctdh);
                }

                db.SaveChanges();
                Session["GioHang"] = null;
                return RedirectToAction("XacNhanDonHang", "GioHang");
            }
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
        public ActionResult PaymentBIDV(string ngayGiao)
        {
            // Save order
            SaveOrder(ngayGiao, false);

            // Redirect to BIDV SmartBanking URL
            string redirectUrl = "https://smartbanking.bidv.com.vn/payment?account=123456789&amount="
                                 + (TongTien() * 100)
                                 + "&content=DH" + DateTime.Now.Ticks;
            return Redirect(redirectUrl);
        }

        public ActionResult PaymentVNPay(string ngayGiao, string bankCode)
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();
            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", (TongTien() * 100).ToString());
            pay.AddRequestData("vnp_BankCode", string.IsNullOrEmpty(bankCode) ? "NCB" : bankCode);
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress());
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang ShopGiay");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);


            SaveOrder(ngayGiao, false);

            return Redirect(paymentUrl);
        }
        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        // Update order status to paid
                        var order = db.DONHANGs.FirstOrDefault(x => x.MaDonHang == orderId);
                        if (order != null)
                        {
                            db.SaveChanges();
                        }
                        ViewBag.Message = "Thanh toán thành công";
                    }
                    else
                    {
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }

        private void SaveOrder(string ngayGiao, bool daThanhToan)
        {
            DONHANG ddh = new DONHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List<GioHang> lstGioHang = LayGioHang();

            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Parse(ngayGiao);
            ddh.TinhTrangGiaoHang = false;
            db.DONHANGs.Add(ddh);
            db.SaveChanges();

            foreach (var item in lstGioHang)
            {
                CT_DONHANG ctdh = new CT_DONHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaGiay = item.iMaGiay;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                db.CT_DONHANG.Add(ctdh);
            }
            db.SaveChanges();
            Session["GioHang"] = null;
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
        [HttpPost]
        public ActionResult ApDungKhuyenMai(string maKM)
        {
            if (string.IsNullOrEmpty(maKM))
            {
                TempData["Error"] = "Vui lòng chọn mã khuyến mãi.";
                return RedirectToAction("GioHang");
            }

            var khuyenMai = khuyenMaiService.GetValidKhuyenMai(maKM);
            if (khuyenMai == null)
            {
                TempData["Error"] = "Mã khuyến mãi không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("GioHang");
            }

            var gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang == null || !gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("GioHang");
            }

            foreach (var item in gioHang)
            {
                double giamGia = KhuyenMaiService.CalculateDiscount(
                    item.dThanhTien,
                    khuyenMai.PhanTramGiam ?? 0
                );
                item.GiamGia = giamGia;
            }

            Session["GioHang"] = gioHang;
            Session["KhuyenMai"] = khuyenMai;

            TempData["Success"] = $"Áp dụng mã khuyến mãi thành công! Giảm {khuyenMai.PhanTramGiam}% trên tổng đơn hàng.";
            return RedirectToAction("GioHang");
        }
    }

    
}