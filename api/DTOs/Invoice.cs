using System.Collections.Generic;

namespace api.DTOs
{
    public class Invoice
    {
        public Header Header { get; set; }
        public List<LineItem> LineItems { get; set; }
        public Summary Summary { get; set; }
    }
}
