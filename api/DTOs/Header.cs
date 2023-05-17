using System;

namespace api.DTOs
{
    public class Header
    {
        public string InvoiceId { get; set; }
        public DateTimeOffset InvoiceDate { get; set; }
        public string PurchaseOrder { get; set; }
        public DateTimeOffset DueDate { get; set; }
    }
}
