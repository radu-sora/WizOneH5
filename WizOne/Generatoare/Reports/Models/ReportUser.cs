//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wizrom.Reports.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReportUser
    {
        public int ReportUserId { get; set; }
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] LayoutData { get; set; }
        public string RegUserId { get; set; }
        public string UpdUserId { get; set; }
        public System.DateTime RegDate { get; private set; }
        public Nullable<System.DateTime> UpdDate { get; set; }
    
        public virtual Report Report { get; set; }
    }
}
