﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MvvmAadapters
{
    public class ModelAdapter<T> : ViewModelBase 
        where T : class
    {
        private Dictionary<string, object> _changedProperties;
        public ModelAdapter(T model) 
        {
            Model = model ?? throw new ArgumentNullException("model");
            _changedProperties = new Dictionary<string, object>();
        }

        public T Model { get; private set; }

        public bool IsChanged { get { return _changedProperties.Any(); } }

        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            return (TValue)propertyInfo.GetValue(Model);
        }

        protected TValue GetTrackedValue<TValue>( string propertyName)
        {
            return _changedProperties.ContainsKey(propertyName) ?
                (TValue) _changedProperties[propertyName] : GetValue<TValue>(propertyName);
        }

        protected bool IsPropertyChanged(string propertyName)
        {
            return _changedProperties.ContainsKey(propertyName);
        }

        protected void SetValue<TValue>(TValue value,[CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            var currentValue = propertyInfo.GetValue(Model);

            if (!Equals(currentValue,value))
            {
                TrackChangedProperty(currentValue, value, propertyName);
                propertyInfo.SetValue(Model, value);
                OnPropertyChanged(propertyName);
                
            }
        }

        private void TrackChangedProperty(object currentValue, object newValue, string propertyName)
        {
           if(!_changedProperties.ContainsKey(propertyName))
            {
                _changedProperties.Add(propertyName, currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            else
            {
                if(Equals(_changedProperties[propertyName],newValue))
                {
                    _changedProperties.Remove(propertyName);
                    OnPropertyChanged(nameof(IsChanged));
                }
            }
        }

        protected void RegisterCollection<TAdapter,TModel>(ObservableCollection<TAdapter> adapterCollection,List<TModel> modelCollection) 
            where TAdapter : ModelAdapter<TModel> where TModel : class
        {
            adapterCollection.CollectionChanged += (s, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems.Cast<TAdapter>())
                    {
                        modelCollection.Remove(item.Model);
                    }
                }
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems.Cast<TAdapter>())
                    {
                        modelCollection.Add(item.Model);
                    }
                }
            };
        }

      
    }
}
