//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MODEL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ST_SYSFLEX_POLICY_RENEW_NC
    {
        public long ROWID { get; set; }
        public System.DateTime INITIAL_DATE { get; set; }
        public string CUSTNMBR { get; set; }
        public int TRANSACTION_SEQ { get; set; }
        public decimal DOCUMENT_NUMBER { get; set; }
        public string NCNUMBER { get; set; }
        public short DISTTYPE { get; set; }
        public string DistRef { get; set; }
        public string ACTNUMST { get; set; }
        public decimal DEBITAMT { get; set; }
        public decimal CRDTAMNT { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public bool PROCESS { get; set; }
        public Nullable<System.DateTime> PROCESS_DATE { get; set; }
    }
}
