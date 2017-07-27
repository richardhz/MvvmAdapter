using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace UnitTestMvvmAdapters
{
    public class ChangeTrackingCollectionPropertyTest
    {
        private PocoTestClass _tester;
        private PocoListItem _listItem;

        public void Initialize()
        {
            _tester = new PocoTestClass
            {
                TestId = 25,
                TestName = "Roger",
                TestBool = false,
                Items = new List<PocoListItem>
                {
                    new PocoListItem { Id = 1, Title = "TestItem01", Description = "Description01" },
                    new PocoListItem { Id = 2, Title = "TestItem02", Description = "Description02" }
                },
                ComplexProp = new PocoListItem { Id = 2, Title = "Complex Property", Description = "Like address" }

            };
        }

        [Fact]
        public void ShouldSetIsChangedOnPocoTestAdapter()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            var listItemToModify = adapter.Items.First();
            listItemToModify.Description = "modified item";

            Assert.True(adapter.IsChanged);

            listItemToModify.Description = "Description01";
            Assert.False(adapter.IsChanged);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForIsChangedPropertyOnPocoTestAdapter()
        {
            Initialize();
            var isRaised = false;
            var adapter = new PocoTestAdapter(_tester);
            adapter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(adapter.IsChanged))
                {
                     isRaised = true;
                }
            };

            adapter.Items.First().Description = "modified item";
            Assert.True(isRaised);
        }

        [Fact]
        public void ShouldAcceptChanges()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);

            var listItemToModify = adapter.Items.First();
            listItemToModify.Description = "modified item";

            Assert.True(adapter.IsChanged);

            adapter.AcceptChanges();

            Assert.False(adapter.IsChanged);
            Assert.Equal("modified item", listItemToModify.Description);
            Assert.Equal("modified item", listItemToModify.DescriptionOriginal);
        }

        [Fact]
        public void ShouldRejectChanges()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);

            var listItemToModify = adapter.Items.First();
            listItemToModify.Description = "modified item";

            Assert.True(adapter.IsChanged);

            adapter.RejectChanges();

            Assert.False(adapter.IsChanged);
            Assert.Equal("Description01", listItemToModify.Description);
            Assert.Equal("Description01", listItemToModify.DescriptionOriginal);
        }
    }

    
}
