using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SaveTheBill.Properties;

namespace SaveTheBill.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Group : ObservableCollection<Group.BillItem>
    {
        private string _name;

        public Group()
        {

        }

        public Group(string name) : base(new[] { new BillItem() })
        {
            Name = name;
        }

        [JsonProperty("ProxyItems")]
        public IList<BillItem> ProxyItems
        {
            get { return Items; }

            set
            {
                Items.Clear();
                foreach (var item in value)
                    Items.Add(item);
            }
        }

        [JsonProperty("Name")]
        public string Name
        {
            get { return _name; }

            set
            {
                if (Equals(_name, value))
                    return;
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public class BillItem : INotifyPropertyChanged
        {
            private string _title = "+ Hinzufügen";

            public string Title
            {
                get { return _title; }
                set
                {
                    if (value == _title) return;
                    _title = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}