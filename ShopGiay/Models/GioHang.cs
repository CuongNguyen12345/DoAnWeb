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
        public int iMaGiay { get; set; }
        public string sTenGiay { get; set; }
        public string sAnhBia { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        public double GiamGia { get; set; }
        public double ThanhTienSauGiam
        {
            get { return Math.Max(0, dThanhTien - GiamGia); }
        }

        public GioHang(int idGiay)
        {
            iMaGiay = idGiay;
            SANPHAM giay = db.SANPHAMs.Single(n => n.MaGiay == iMaGiay);
            sTenGiay = giay.TenGiay;
            sAnhBia = giay.AnhBia;
            dDonGia = double.Parse(giay.GiaBan.ToString());
            iSoLuong = 1;
            GiamGia = 0;
        }
    }
}