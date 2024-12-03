using ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopGiay.Services
{
    public class KhuyenMaiService
    {
        private readonly ShopGiayEntities _db;

        public KhuyenMaiService(ShopGiayEntities db)
        {
            _db = db;
        }

        public KHUYENMAI GetValidKhuyenMai(string maKM)
        {
            return _db.KHUYENMAIs.SingleOrDefault(km =>
                km.TenKM == maKM &&
                km.TrangThai == 0 &&
                km.NgayBatDau <= DateTime.Now &&
                km.NgayKetThuc >= DateTime.Now);
        }
        public List<KHUYENMAI> GetAllValidKhuyenMais()
        {
            return _db.KHUYENMAIs
                .Where(km =>
                    km.NgayBatDau <= DateTime.Now &&
                    km.NgayKetThuc >= DateTime.Now)
                .ToList();
        }

        public static double CalculateDiscount(double originalPrice, double discountPercentage)
        {
            return Math.Round(originalPrice * (discountPercentage / 100), 2);
        }
    }
}