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
    
    public partial class QUANLY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QUANLY()
        {
            this.PHANQUYENs = new HashSet<PHANQUYEN>();
        }
    
        public int MaQL { get; set; }
        public string TaiKhoanQL { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public string EmailQL { get; set; }
        public string DienThoaiQL { get; set; }
        public Nullable<bool> TrangThai { get; set; }
        public string Avatar { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PHANQUYEN> PHANQUYENs { get; set; }
    }
}
