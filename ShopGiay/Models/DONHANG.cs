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
    
    public partial class DONHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DONHANG()
        {
            this.CT_DONHANG = new HashSet<CT_DONHANG>();
        }
    
        public int MaDonHang { get; set; }
        public Nullable<bool> TinhTrangGiaoHang { get; set; }
        public System.DateTime NgayDat { get; set; }
        public Nullable<System.DateTime> NgayGiao { get; set; }
        public Nullable<decimal> TongTien { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<int> DaThanhToan { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CT_DONHANG> CT_DONHANG { get; set; }
        public virtual KHACHHANG KHACHHANG { get; set; }
    }
}
