//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApp1
{
    using System;
    using System.Collections.Generic;
    
    public partial class HO_RESERV
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HO_RESERV()
        {
            this.DATA_TRANSFERS = new HashSet<DATA_TRANSFERS>();
        }
    
        public int HO_RESERV_ID { get; set; }
        public Nullable<System.DateTime> FROM_DATE { get; set; }
        public Nullable<System.DateTime> TO_DATE { get; set; }
        public Nullable<int> DURATION { get; set; }
        public Nullable<int> FK_MEDDEP_ID { get; set; }
        public Nullable<int> FK_HO_RESERV_STATUS_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DATA_TRANSFERS> DATA_TRANSFERS { get; set; }
        public virtual MEDDEP MEDDEP { get; set; }
        public virtual HO_RESERV_STATUS HO_RESERV_STATUS { get; set; }
    }
}