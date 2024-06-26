//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CinemaWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public invoice()
        {
            this.tickets = new HashSet<ticket>();
        }
    
        public int id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> discount_id { get; set; }
        public Nullable<System.DateTime> day_create { get; set; }
        public Nullable<bool> invoice_status { get; set; }
        public Nullable<int> total_ticket { get; set; }
        public Nullable<int> total_money { get; set; }
        public Nullable<int> room_schedule_detail_id { get; set; }
        public string qrcode_image { get; set; }
        public Nullable<bool> is_used { get; set; }
    
        public virtual discount discount { get; set; }
        public virtual room_schedule_detail room_schedule_detail { get; set; }
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ticket> tickets { get; set; }
    }
}
