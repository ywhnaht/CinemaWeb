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
    
    public partial class movy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public movy()
        {
            this.movie_actor = new HashSet<movie_actor>();
            this.movie_display_date = new HashSet<movie_display_date>();
            this.movie_review = new HashSet<movie_review>();
            this.star_rating = new HashSet<star_rating>();
        }
    
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Nullable<int> director_id { get; set; }
        public Nullable<int> type_id { get; set; }
        public Nullable<System.DateTime> release_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public string duration_minutes { get; set; }
        public Nullable<int> country_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<bool> movie_status { get; set; }
        public string url_image { get; set; }
        public Nullable<decimal> rating { get; set; }
        public string url_trailer { get; set; }
        public string url_large_image { get; set; }
    
        public virtual country country { get; set; }
        public virtual director director { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<movie_actor> movie_actor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<movie_display_date> movie_display_date { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<movie_review> movie_review { get; set; }
        public virtual movie_type movie_type { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<star_rating> star_rating { get; set; }
    }
}
