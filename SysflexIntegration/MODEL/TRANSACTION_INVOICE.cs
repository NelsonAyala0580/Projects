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
    
    public partial class TRANSACTION_INVOICE
    {
        public System.Guid ROW_ID { get; set; }
        public string POLICY_NUMBER { get; set; }
        public string TRANS_TYPE { get; set; }
        public string TRANS_DESC { get; set; }
        public Nullable<System.DateTime> TRANS_DATE { get; set; }
        public Nullable<decimal> TRANS_AMOUNT { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public Nullable<decimal> ACTUAL_BALANCE { get; set; }
        public string COMMENT { get; set; }
        public string TRANS_USER { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<System.DateTime> SYNC_CREATE_DATE { get; set; }
        public Nullable<System.DateTime> GP_PROCESS_DATE { get; set; }
        public string GP_PROCESS_USER { get; set; }
        public string GP_MODULE { get; set; }
        public Nullable<int> GP_DOCTYPE { get; set; }
        public string GP_DOCNUMBR { get; set; }
        public string RENEWAL_TYPE { get; set; }
        public Nullable<int> DOCUMENT_TYPE { get; set; }
        public Nullable<bool> DOCUMENT_CANCELLED { get; set; }
        public string NCF { get; set; }
        public string ASSIGNED_DOC_NUMBER { get; set; }
        public Nullable<int> TRANSACTION_SEQUENCE { get; set; }
    }
}
