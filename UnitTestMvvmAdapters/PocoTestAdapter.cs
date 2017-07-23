using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmAadapters;
using System.Collections.ObjectModel;

namespace UnitTestMvvmAdapters
{
    class PocoTestAdapter : ModelAdapter<PocoTestClass>
    {
        public PocoTestAdapter(PocoTestClass model ) : base(model)
        {
            InitializeCollections(model);
        }

        private void InitializeCollections(PocoTestClass model)
        {
            //The creation of adapter list items is not tested here, this would be a user test and not a library test.
            Items = new ObservableCollection<PocoListItemAdapter>(model.Items.Select(i => new PocoListItemAdapter(i))); 
            //However, if used the RegisterCollection method should do what it says on the tin. 
            RegisterCollection(Items, model.Items);
        }

        public int TestId
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public string TestName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public bool TestBool
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<PocoListItemAdapter> Items { get; private set; }
    }
}
