using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTestMvvmAdapters
{
    public class ChangeNotificationTest
    {
        private PocoTestClass _tester;
        private PocoListItem _listItem;


        public void Initialize()
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

        [Fact]
        public void PocoAndAdapterCollectionsShouldBeInSyncAfterRemovingItem()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            var itemToRemove = adapter.Items.Single(ri => ri.Model == _listItem);
            adapter.Items.Remove(itemToRemove);
            
            CheckCollectionsInSync(adapter);
        }

        [Fact]
        public void PocoAndAdapterCollectionsShouldBeInSyncAfterAddingItem()
        {
            Initialize();
            _tester.Items.Remove(_listItem);

            var adapter = new PocoTestAdapter(_tester);
            adapter.Items.Add(new PocoListItemAdapter(_listItem));

            CheckCollectionsInSync(adapter);
        }

        [Fact]
        public void PocoAndAdapterCollectionsShouldBeInSyncAfterRemovingAllListItems()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);

            adapter.Items.Clear();

            CheckCollectionsInSync(adapter);
        }

        private void CheckCollectionsInSync(PocoTestAdapter adapter)
        {
            Assert.Equal(_tester.Items.Count, adapter.Items.Count);
            Assert.True(_tester.Items.All(ti => adapter.Items.Any(ai => ai.Model == ti)));
        }
    }
}
