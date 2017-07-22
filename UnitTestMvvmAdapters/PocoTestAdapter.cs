using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmAadapters;

namespace UnitTestMvvmAdapters
{
    class PocoTestAdapter : ModelAdapter<PocoTestClass>
    {
        public PocoTestAdapter(PocoTestClass model ) : base(model)
        { }

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
    }
}
