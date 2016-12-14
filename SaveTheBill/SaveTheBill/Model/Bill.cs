using System;

namespace SaveTheBill.Model
{
    public class Bill : Base
    {
        public string ImageSource { get; set; }
        public string Amount { get; set; }
        public Currency Currency { get; set; }
        public DateTime ScanDate { get; set; }
        public bool HasGuarantee { get; set; }
        public DateTime GuaranteeExpireDate { get; set; }
        public string Location { get; set; }
    }
}