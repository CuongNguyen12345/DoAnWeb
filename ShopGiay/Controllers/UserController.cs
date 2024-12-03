using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;


namespace ShopGiay.Controllers
{
    public class UserController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        private static int MaGiayGolbal = 0;
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
        private void GuiEmailXacNhan(string email, KHACHHANG kh, string matKhauNgauNhien)
        {
            string username = "Quynhnhuvh19@gmail.com";
            string password = "cnjj hlij byjd hclu";

            try
            {
                string body = $@"
         <table style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd;'>
             <tr>
                 <td style='background: linear-gradient(45deg, #007bff, #00c6ff); padding: 25px; text-align: center; color: white;'>
                     <h2 style='font-size: 28px; margin: 0;'>Xác nhận đăng ký thành công</h2>
                     <p style='font-size: 16px; margin: 0;'>Cảm ơn bạn đã đăng ký thành viên tại <strong>Shop Giày</strong>!</p>
                 </td>
             </tr>
             <tr>
                 <td style='padding: 30px; background-color: #fafafa;'>
                     <h3 style='font-size: 22px; color: #333; margin-bottom: 15px;'>Thông tin đăng ký của bạn:</h3>
                     <p><strong>Tên đăng nhập:</strong> {kh.TaiKhoanKH}</p>
                     <p><strong>Số điện thoại:</strong> {kh.DienThoaiKH}</p>
                     <p><strong>Email:</strong> {kh.EmailKH}</p>
                     <p style='color: #FF5733; font-size: 18px; margin-top: 20px;'><strong>MẬT KHẨU CỦA BẠN LÀ:</strong> {matKhauNgauNhien}</p>
                     
                     <p style='margin-top: 20px; font-size: 14px; color: #666;'>Nếu có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: 
                         <a href='mailto:Quynhnhuvh19@gmail.com' style='color: #006EC7; text-decoration: none;'>DevNguyen@gmail.com</a>.
                     </p>
                     <p style='margin-top: 15px; font-size: 14px; color: #666;'>Trân trọng,<br/><strong>Giày chất lượng cao</strong></p>
                 </td>
             </tr>
             <tr>
                 <td style='background: linear-gradient(45deg, #007bff, #00c6ff); padding: 15px; text-align: center; color: white;'>
                     <p style='margin: 0; font-size: 14px;'>&copy; 2024 Shop Giày. Dev Nguyen.</p>
                 </td>
             </tr>
         </table>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(username);
                mail.To.Add(email);
                mail.Subject = "Xác Nhận Đơn Hàng";
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(username, password);
                smtp.EnableSsl = true;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gửi email thất bại: " + ex.Message);
            }

        }
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            Random random = new Random();
            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Chuyển byte thành chuỗi hex
                }
                return builder.ToString();
            }
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection f)
        {
            string hoTen = f["HoTen"];
            string userName = f["TaiKhoan"];
            string ngaySinh = f["NgaySinh"];
            string email = f["Email"];
            string phoneNumber = f["DienThoai"];
            string diaChi = f["DiaChi"];
            KHACHHANG acc = db.KHACHHANGs.FirstOrDefault(u => u.EmailKH == email || u.TaiKhoanKH == userName || u.DienThoaiKH == phoneNumber);
            if (acc == null)
            {
                acc = new KHACHHANG();
                var matKhauNgauNhien = GenerateRandomPassword(8);
                var matKhauMaHoa = HashPassword(matKhauNgauNhien);
                acc.EmailKH = email;
                acc.DienThoaiKH = phoneNumber;
                acc.NgaySinh = DateTime.Parse(ngaySinh);
                acc.TaiKhoanKH = userName;
                acc.MatKhau = matKhauMaHoa;
                acc.HoTen = hoTen;
                acc.DiaChiKH = diaChi;
                acc.TrangThai = true;

                db.KHACHHANGs.Add(acc);
                db.SaveChanges();
                GuiEmailXacNhan(acc.EmailKH, acc, matKhauNgauNhien);

                //TempData["dangkythanhcong"] = true;
                return RedirectToAction("DangNhap", "User");
            }
            else
            {
                if (acc.EmailKH == email)
                {
                    ViewBag.thongbao = "Email đã được đăng ký trước đó.";
                }
                else if (acc.TaiKhoanKH == userName)
                {
                    ViewBag.thongbao = "Tên đăng nhập đã được đăng ký trước đó.";
                }
                else if (acc.DienThoaiKH == phoneNumber)
                {
                    ViewBag.thongbao = "Số điện thoại đã được đăng ký trước đó.";
                }
                return View();
            }
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {
                string _hassPassword = HashPassword(sMatKhau);
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoanKH == sTenDN && n.MatKhau == _hassPassword);
                
                QUANLY ql = db.QUANLies.SingleOrDefault(n => n.TaiKhoanQL == sTenDN && n.MatKhau == sMatKhau);
                if (kh != null)
                {
 
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    if (collection["remember"].Contains("true"))
                    {
                        Response.Cookies["TenDN"].Value = sTenDN;
                        Response.Cookies["MatKhau"].Value = sMatKhau;
                        Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(1);
                        Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(1);
                    }
                    else
                    {
                        Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(-1);
                    }
                    Session["HoTen"] = kh.HoTen; // Đảm bảo tên người dùng được lưu vào Session
                    return RedirectToAction("Index", "User");
                }
                else if(ql != null)
                {
                    Session["MaQL"] = ql.MaQL;
                    return RedirectToAction("Admin", "Admin");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }
        
        public ActionResult DangXuat()
        {
            Session["HoTen"] = null;

            if (Request.Cookies["TenDN"] != null)
            {
                var userNameCookie = new HttpCookie("TenDN");
                userNameCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userNameCookie);
            }

            if (Request.Cookies["MatKhau"] != null)
            {
                var passwordCookie = new HttpCookie("MatKhau");
                passwordCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(passwordCookie);
            }

            return RedirectToAction("Index", "User");
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

        public ActionResult ThuongHieu()
        {
            var ThuongHieu = db.THUONGHIEUx.ToList();
            return PartialView(ThuongHieu);
        }
        public ActionResult Product(int? page, int? MaTT) 
        {
            int iSize = 6;
            MaTT = MaTT ?? 0;
            int iPageNumber = (page ?? 1);
            var lstItem = (from sp in db.SANPHAMs
                          where MaTT == 0 || sp.MaThuongHieu == MaTT
                          select sp).ToList();
            
            return View(lstItem.ToPagedList(iPageNumber, iSize));
        }

        [HttpGet]
        public ActionResult ChiTietSanPham(int id)
        {
            var SanPham = db.SANPHAMs.SingleOrDefault(s => s.MaGiay.Equals(id));
            MaGiayGolbal = id;
            return View(SanPham);
        }

        [HttpGet]
        public ActionResult ThemBinhLuan()
        {
            ViewBag.MaGiay = MaGiayGolbal;

            var lstBL = db.BINHLUANs
                                 .Where(b => b.MaSP == MaGiayGolbal)
                                 .OrderByDescending(b => b.NgayBinhLuan)
                                 .ToList();
            return PartialView(lstBL);
        }

        [HttpPost]
        public ActionResult ThemBinhLuan(int maSP, string binhLuan)
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var khachHang = (KHACHHANG)Session["Taikhoan"];

            var bl = new BINHLUAN
            {
                MaKH = khachHang.MaKH,
                TenKH = khachHang.HoTen,
                MaSP = maSP,
                BinhLuan1 = binhLuan,
                NgayBinhLuan = DateTime.Now
            };
                
            db.BINHLUANs.Add(bl);
            db.SaveChanges();

            return RedirectToAction("ChiTietSanPham", new { id = maSP });
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