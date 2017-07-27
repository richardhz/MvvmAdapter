using MvvmAadapters;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTestMvvmAdapters
{
    public class ChangeTrackingCollectionTest
    {
        private List<PocoListItemAdapter> _listItems;


        private void Initialize()
        {
            _listItems = new List<PocoListItemAdapter>
           {
               new PocoListItemAdapter( new PocoListItem { Id = 1, Title = "TestItem01", Description = "Description01" }),
               new PocoListItemAdapter( new PocoListItem { Id = 2, Title = "TestItem02", Description = "Description02" })
           };
        }
        
        


        [Fact]
        public void ShouldTrackAddedItems()
        {
            Initialize();

            var listItemToAdd = new PocoListItemAdapter(new PocoListItem());

            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            Assert.Equal(2, c.Count);
            Assert.False(c.IsChanged);

            c.Add(listItemToAdd);
            Assert.Equal(3, c.Count);
            Assert.Equal(1, c.AddedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.Equal(listItemToAdd, c.AddedItems.First());
            Assert.True(c.IsChanged);

            c.Remove(listItemToAdd);
            Assert.Equal(2, c.Count);
            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.False(c.IsChanged);
        }


        [Fact]
        public void ShouldTrackRemovedItems()
        {
            Initialize();
            var listItemToRemove = _listItems.First();
            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            Assert.Equal(2, c.Count);
            Assert.False(c.IsChanged);

            c.Remove(listItemToRemove);
            Assert.Equal(1, c.Count);
            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(1, c.RemovedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.Equal(listItemToRemove, c.RemovedItems.First());
            Assert.True(c.IsChanged);

            c.Add(listItemToRemove);
            Assert.Equal(2, c.Count);
            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.False(c.IsChanged);
        }

        [Fact]
        public void ShouldTrackModifiedItem()
        {
            Initialize();
            var listItemToModify = _listItems.First();
            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            Assert.Equal(2, c.Count);
            Assert.False(c.IsChanged);

            listItemToModify.Description = "modified entry";
            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(1, c.ModifiedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);
            Assert.True(c.IsChanged);

            listItemToModify.Description = "Description01";
            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);
            Assert.False(c.IsChanged);
        }

        [Fact]
        public void ShouldNotTrackAddedItemAsModified()
        {
            Initialize();
            var listItemToAdd = new PocoListItemAdapter(new PocoListItem());

            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            c.Add(listItemToAdd);
            listItemToAdd.Description = "modified entry";
            Assert.True(listItemToAdd.IsChanged);
            Assert.Equal(3, c.Count);
            Assert.Equal(1, c.AddedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.True(c.IsChanged);
        }

        [Fact]
        public void ShouldNotTrackRemovedItemAsModified()
        {
            Initialize();
            var listItemToModifyAndRemove = _listItems.First();

            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);
            listItemToModifyAndRemove.Description = "modified entry";
            Assert.Equal(2, c.Count);
            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);
            Assert.Equal(1, c.ModifiedItems.Count);
            Assert.Equal(listItemToModifyAndRemove, c.ModifiedItems.First());
            Assert.True(c.IsChanged);

            c.Remove(listItemToModifyAndRemove);
            Assert.Equal(1, c.Count);
            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(1, c.RemovedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.Equal(listItemToModifyAndRemove, c.RemovedItems.First());
            Assert.True(c.IsChanged);
        }

        [Fact]
        public void ShouldAcceptChanges()
        {
            Initialize();
            var listItemToAdd = new PocoListItemAdapter(new PocoListItem { Id = 3, Title = "TestItem03", Description = "Description03" });
            var listItemToModify = _listItems.First();
            var listItemToRemove = _listItems.Skip(1).First();
            

            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);

            c.Add(listItemToAdd);
            c.Remove(listItemToRemove);
            listItemToModify.Description = "modified item";
            Assert.Equal("Description01", listItemToModify.DescriptionOriginal);

            Assert.Equal(2, c.Count);
            Assert.Equal(1, c.AddedItems.Count);
            Assert.Equal(1, c.ModifiedItems.Count);
            Assert.Equal(1, c.RemovedItems.Count);

            c.AcceptChanges();

            Assert.Equal(2, c.Count);
            Assert.True(c.Contains(listItemToModify));
            Assert.True(c.Contains(listItemToAdd));

            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);

            Assert.False(listItemToModify.IsChanged);
            Assert.Equal("modified item", listItemToModify.Description);
            Assert.Equal("modified item", listItemToModify.DescriptionOriginal);

            Assert.False(c.IsChanged);
        }

        [Fact]
        public void ShouldRejectChanges()
        {
            Initialize();
            var listItemToAdd = new PocoListItemAdapter(new PocoListItem { Id = 3, Title = "TestItem03", Description = "Description03" });
            var listItemToModify = _listItems.First();
            var listItemToRemove = _listItems.Skip(1).First();


            var c = new ChangeTrackingCollection<PocoListItemAdapter>(_listItems);

            c.Add(listItemToAdd);
            c.Remove(listItemToRemove);
            listItemToModify.Description = "modified item";
            Assert.Equal("Description01", listItemToModify.DescriptionOriginal);

            Assert.Equal(2, c.Count);
            Assert.Equal(1, c.AddedItems.Count);
            Assert.Equal(1, c.ModifiedItems.Count);
            Assert.Equal(1, c.RemovedItems.Count);

            c.RejectChanges();

            Assert.Equal(2, c.Count);
            Assert.True(c.Contains(listItemToModify));
            Assert.True(c.Contains(listItemToRemove));

            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);

            Assert.False(listItemToModify.IsChanged);
            Assert.Equal("Description01", listItemToModify.Description);
            Assert.Equal("Description01", listItemToModify.DescriptionOriginal);

            Assert.False(c.IsChanged);
        }

        [Fact]
        public void ShouldRejectChangesWithModifiedAndRemovedItem()
        {
            Initialize();

            var listItem = _listItems.First();
           
            var c = new ChangeTrackingCollection<PocoListItemAdapter> (_listItems);

            listItem.Description = "modified item";
            c.Remove(listItem);
            Assert.Equal("Description01", listItem.DescriptionOriginal);

            Assert.Equal(1, c.Count);
            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.Equal(1, c.RemovedItems.Count);

            c.RejectChanges();

            Assert.Equal(2, c.Count);
            Assert.True(c.Contains(listItem));

            Assert.Equal(0, c.AddedItems.Count);
            Assert.Equal(0, c.ModifiedItems.Count);
            Assert.Equal(0, c.RemovedItems.Count);

            Assert.False(listItem.IsChanged);
            Assert.Equal("Description01", listItem.Description);
            Assert.Equal("Description01", listItem.DescriptionOriginal);

            Assert.False(c.IsChanged);
        }


    }
}
