﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MvvmAadapters
{
    public class ChangeTrackingCollection<T> : ObservableCollection<T>, IRevertibleChangeTracking
        where T : class, IRevertibleChangeTracking, INotifyPropertyChanged
    {
        private IList<T> _originalCollection;

        private ObservableCollection<T> _addedItems = new ObservableCollection<T>();
        private ObservableCollection<T> _removedItems = new ObservableCollection<T>();
        private ObservableCollection<T> _modifiedItems = new ObservableCollection<T>();

        public ChangeTrackingCollection(IEnumerable<T> collection) : base(collection)
        {
            _originalCollection = this.ToList();

            AttachItemPropertyChangeHandler(_originalCollection);

            AddedItems = new ReadOnlyObservableCollection<T>(_addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(_removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(_modifiedItems);

        }

        public ReadOnlyObservableCollection<T> AddedItems { get; }
        public ReadOnlyObservableCollection<T> RemovedItems { get; }
        public ReadOnlyObservableCollection<T> ModifiedItems { get; }


        

        public bool IsChanged => (AddedItems.Any() || RemovedItems.Any() || ModifiedItems.Any());

        public void AcceptChanges()
        {
            _addedItems.Clear();
            _removedItems.Clear();
            _modifiedItems.Clear();
            foreach(var item in this)
            {
                item.AcceptChanges();
            }
            _originalCollection = this.ToList();
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        public void RejectChanges()
        {
            foreach(var addedItem in _addedItems.ToList())
            {
                Remove(addedItem);
            }
            foreach(var removedItem in _removedItems.ToList())
            {
                removedItem.RejectChanges();
                Add(removedItem);
            }
            foreach(var modifiedItem in _modifiedItems.ToList())
            {
                modifiedItem.RejectChanges();
            }
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        private void AttachItemPropertyChangeHandler(IEnumerable<T> items)
        {
            foreach( var item in items)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
        }

        private void DetachItemPropertyChangedHandler(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged -= ItemPropertyChanged;
            }
        }

        private void UpdateObservableCollection(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear(); 
            foreach( var item in items)
            {
                collection.Add(item);
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (T)sender;
            if (_addedItems.Contains(item))
            {
                return;
            }

            if (item.IsChanged)
            {
                if(!_modifiedItems.Contains(item))
                {
                    _modifiedItems.Add(item);
                }
            }
            else
            {
                if (_modifiedItems.Contains(item))
                {
                    _modifiedItems.Remove(item);
                }
            }
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var added = this.Where(current => _originalCollection.All(orig => orig != current));
            var removed = _originalCollection.Where(orig => this.All(current => current != orig));

            var modified = this.Except(added).Except(removed).Where(item => item.IsChanged);

            AttachItemPropertyChangeHandler(added);
            DetachItemPropertyChangedHandler(removed);

            UpdateObservableCollection(_addedItems, added);
            UpdateObservableCollection(_removedItems, removed);
            UpdateObservableCollection(_modifiedItems, modified);

            base.OnCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }
    }
}
