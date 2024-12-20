//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopGiay.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SANPHAM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SANPHAM()
        {
            this.BINHLUANs = new HashSet<BINHLUAN>();
            this.CT_DONHANG = new HashSet<CT_DONHANG>();
        }
    
        public int MaGiay { get; set; }
        public string TenGiay { get; set; }
        public byte Size { get; set; }
        public string AnhBia { get; set; }
        public decimal GiaBan { get; set; }
        public Nullable<int> MaThuongHieu { get; set; }
        public Nullable<bool> TrangThai { get; set; }
        public Nullable<int> MaNCC { get; set; }
        public Nullable<int> MaLoai { get; set; }
        public Nullable<int> ThoiGianBaoHanh { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public int SoLuongTon { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BINHLUAN> BINHLUANs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CT_DONHANG> CT_DONHANG { get; set; }
        public virtual LOAIGIAY LOAIGIAY { get; set; }
        public virtual NHACUNGCAP NHACUNGCAP { get; set; }
        public virtual THUONGHIEU THUONGHIEU { get; set; }
    }
}
