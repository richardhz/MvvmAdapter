using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace UnitTestMvvmAdapters
{
    public class AdapterChangeTrackingComplexProperties
    {
        private PocoTestClass _tester;
        private PocoListItem _listItem;


        private void Initialize()
        {
            _listItem = new PocoListItem { Id = 2, Title = "TestItem02", Description = "Description02" };
            _tester = new PocoTestClass
            {
                TestId = 25,
                TestName = "Roger",
                TestBool = false,
                Items = new List<PocoListItem>
                {
                    new PocoListItem { Id = 1, Title = "TestItem01", Description = "Description01" },
                    _listItem
                },
                ComplexProp = new PocoListItem { Id = 2, Title = "Complex Property", Description = "Like address" }

            };
        }

        [Fact]
        public void WhenComplexPropertyIsChangedTheParentIsChangedShouldBeSet()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            adapter.ComplexProp.Description = "Something Different";
            Assert.True(adapter.IsChanged);

            adapter.ComplexProp.Description = "Like address";
            Assert.False(adapter.IsChanged);
        }

        [Fact]
        public void WhenComplexPropertyIsChangedShouldRaisePropertyChangedEventForParentTheObject()
        {
            var isRaised = false;
            Initialize();

            var adapter = new PocoTestAdapter(_tester);

            adapter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(adapter.IsChanged))
                {
                    isRaised = true;
                }
            };
            adapter.ComplexProp.Description = "Something Different";
            Assert.Equal(true, isRaised);
        }

        [Fact]
        public void ShouldAcceptChangesForComplexProperties()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);

            adapter.ComplexProp.Description = "Something Different";
            Assert.True(adapter.ComplexProp.IsChanged);
            Assert.True(adapter.IsChanged);

            adapter.AcceptChanges();

            Assert.Equal("Something Different", adapter.ComplexProp.Description);
            Assert.False(adapter.ComplexProp.IsChanged);
            Assert.False(adapter.IsChanged);

        }
        [Fact]
        public void ShouldRejectChangesForComplexProperties()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);

            adapter.ComplexProp.Description = "Something Different";
            Assert.Equal("Something Different", adapter.ComplexProp.Description);
            Assert.True(adapter.ComplexProp.IsChanged);
            Assert.True(adapter.IsChanged);

            adapter.RejectChanges();

            Assert.Equal("Like address", adapter.ComplexProp.Description);
            Assert.False(adapter.ComplexProp.IsChanged);
            Assert.False(adapter.IsChanged);

        }


    }
}
