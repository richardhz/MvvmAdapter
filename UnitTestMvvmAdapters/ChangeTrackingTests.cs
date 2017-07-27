using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTestMvvmAdapters
{
    public class ChangeTrackingTests
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
        public void OriginalValueIsStored()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            Assert.Equal("Roger", adapter.TestNameOriginal);
            adapter.TestName = "Boris";
            Assert.Equal("Roger", adapter.TestNameOriginal);

        }

        [Fact]
        public void IsChangedIsSetWhenPropertyChanges()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            Assert.False(adapter.TestIdHasChanged);
            
            adapter.TestId = 100;
            Assert.True(adapter.TestIdHasChanged);

            
        }

        [Fact]
        public void ObjectIsChangedShouldBeSetForAnyPropertyChange()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            Assert.False(adapter.TestIdHasChanged);
            Assert.False(adapter.IsChanged);

            adapter.TestId = 100;
            Assert.True(adapter.TestIdHasChanged);
            Assert.True(adapter.IsChanged);

            adapter.TestId = 25;
            Assert.False(adapter.IsChanged);

            adapter.TestName = "Boris";
            Assert.True(adapter.IsChanged);
        }

        [Fact]
        public void IsChangedIsFalseWhenPropertyChangesToOriginalValue()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            Assert.False(adapter.TestIdHasChanged);
            adapter.TestId = 100;
            Assert.True(adapter.TestIdHasChanged);
            adapter.TestId = 25;
            Assert.False(adapter.TestIdHasChanged);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForTheObject()
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
            adapter.TestId = 50;
            Assert.Equal(true, isRaised);
        }

        [Fact]

        public void ShouldAcceptChanges()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            adapter.TestName = "Sandra";
            adapter.TestId = 100;
            Assert.Equal("Sandra", adapter.TestName);
            Assert.Equal("Roger", adapter.TestNameOriginal);
            Assert.True(adapter.TestIdHasChanged);
            Assert.True(adapter.IsChanged);

            adapter.AcceptChanges();

            Assert.Equal("Sandra", adapter.TestName);
            Assert.Equal("Sandra", adapter.TestNameOriginal);
            Assert.False(adapter.TestIdHasChanged);
            Assert.False(adapter.IsChanged);

        }

        [Fact]

        public void ShouldRejectChanges()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            adapter.TestName = "Sandra";
            adapter.TestId = 100;
            Assert.Equal("Sandra", adapter.TestName);
            Assert.Equal("Roger", adapter.TestNameOriginal);
            Assert.True(adapter.TestIdHasChanged);
            Assert.True(adapter.IsChanged);

            adapter.RejectChanges();

            Assert.Equal("Roger", adapter.TestName);
            Assert.Equal("Roger", adapter.TestNameOriginal);
            Assert.Equal(25, adapter.TestId);
            Assert.False(adapter.TestIdHasChanged);
            Assert.False(adapter.IsChanged);

        }

    }
}
