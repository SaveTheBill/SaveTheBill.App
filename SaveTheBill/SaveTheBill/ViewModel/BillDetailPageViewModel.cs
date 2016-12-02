using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaveTheBill.Infrastructure;
using SaveTheBill.Model;
using Xamarin.Forms;

namespace SaveTheBill.ViewModel
{
    public class BillDetailPageViewModel
    {
        private readonly IList<Bill> _billList;
        private readonly FileSaver _fileSaver;

        public BillDetailPageViewModel()
        {
            _billList = new List<Bill>();
            _fileSaver = new FileSaver();
        }
        public async void Save_OnClicked(string title, string ammount, string detail, bool hasGuarantee, DateTime guaranteeDatePicker, DateTime buyDate, string location)
        {
            var list = JsonConvert.DeserializeObject<IEnumerable<Bill>>(await _fileSaver.ReadContentFromLocalFileAsync());

            if (list != null && list.Any())
            {
                foreach (var item in list)
                    _billList.Add(item);
            }

            double cost;
            double.TryParse(ammount, out cost);

            var bill = new Bill
            {
                Title = title,
                Amount = cost,
                Detail = detail,
                HasGuarantee = hasGuarantee,
                GuaranteeExpireDate = guaranteeDatePicker,
                ScanDate = buyDate,
                Location = location
            };
            _billList.Add(bill);

            await _fileSaver.SaveContentToLocalFileAsync(_billList);
        }
    }
}
