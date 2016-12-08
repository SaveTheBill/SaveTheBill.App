using System;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace SaveTheBill.Model
{
    public class Bill : Base
    {
        public string ImageSource { get; set; }
        public double Amount { get; set; }
        public DateTime ScanDate { get; set; }
        public bool HasGuarantee { get; set; }
        public DateTime GuaranteeExpireDate { get; set; }
        public string Location { get; set; }
    }
}