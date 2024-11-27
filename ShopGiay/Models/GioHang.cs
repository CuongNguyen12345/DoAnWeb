using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopGiay.Models
{
    public class GioHang
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // Thuộc tính giỏ hàng giày
        public int iMaGiay { get; set; } // Mã giày
        public string sTenGiay { get; set; } // Tên giày
        public string sAnhBia { get; set; } // Hình ảnh giày
        public double dDonGia { get; set; } // Đơn giá giày
        public int iSoLuong { get; set; } // Số lượng
        public double dThanhTien { get { return iSoLuong * dDonGia; } } // Thành tiền

        //Thêm size giày, thương hiệu


        // Hàm khởi tạo
        public GioHang(int idGiay)
        {
            iMaGiay = idGiay;
            SANPHAM giay = db.SANPHAMs.Single(n => n.MaGiay == iMaGiay); // Lấy thông tin giày từ database
            sTenGiay = giay.TenGiay; // Lấy tên giày
            sAnhBia = giay.AnhBia; // Lấy đường dẫn hình ảnh
            dDonGia = double.Parse(giay.GiaBan.ToString()); // Lấy giá bán
            iSoLuong = 1; // Khởi tạo số lượng mặc định là 1
        }
    }
}