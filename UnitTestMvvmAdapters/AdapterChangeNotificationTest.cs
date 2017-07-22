using System;
using Xunit;

namespace UnitTestMvvmAdapters
{
    public class AdapterChangeNotificationTest
    {
        private PocoTestClass _tester;


        public void Initialize()
        {
            _tester = new PocoTestClass
            {
                TestId = 25,
                TestName = "Roger",
                TestBool = false
            };
        }

        [Fact]
        public void RaisePropertyChangedEventWhenPropertyIsChanged()
        {
            var isRaised = false;
            Initialize();

            var adapter = new PocoTestAdapter(_tester);

            adapter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "TestName")
                {
                    isRaised = true;
                }
            };
            adapter.TestName = "Boris";
            Assert.Equal(true, isRaised);
        }


        [Fact]
        public void ShouldNotRaisePropertyChangedEventWhenPropertyValueIsSame()
        {
            var isRaised = false;
            Initialize();

            var adapter = new PocoTestAdapter(_tester);

            adapter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "TestId")
                {
                    isRaised = true;
                }
            };
            adapter.TestId = 25;
            Assert.Equal(false, isRaised);
        }
    }
}
