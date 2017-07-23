using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MvvmAadapters
{
    public class ModelAdapter<T> : ViewModelBase , IRevertibleChangeTracking
    {
        private Dictionary<string, object> _changedProperties;
        private List<IRevertibleChangeTracking> _trackingObjects;
        public ModelAdapter(T model) 
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Model = model; 
            _changedProperties = new Dictionary<string, object>();
            _trackingObjects = new List<IRevertibleChangeTracking>();
        }

        public T Model { get; private set; }

        public bool IsChanged { get { return _changedProperties.Any() || _trackingObjects.Any(o => o.IsChanged); } }

        public void AcceptChanges()
        {
            _changedProperties.Clear();
            foreach(var complexObject in _trackingObjects)
            {
                complexObject.AcceptChanges();
            }

            OnPropertyChanged("");
        }

        public void RejectChanges()
        {
            foreach(var originalPropertyValue in _changedProperties)
            {
                typeof(T).GetProperty(originalPropertyValue.Key).SetValue(Model,originalPropertyValue.Value);
            }
            _changedProperties.Clear();
            foreach (var complexObject in _trackingObjects)
            {
                complexObject.RejectChanges();
            }
            OnPropertyChanged("");
        }

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

        protected void RegisterComplex<TModel>(ModelAdapter<TModel> adapter)
        {
            RegisterTrackingObject(adapter);
        }

        private void RegisterTrackingObject<TTrackingObject>(TTrackingObject trackingObject)
           where TTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
        {
            if (!_trackingObjects.Contains(trackingObject))
            {
                _trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
            }
        }

        private void TrackingObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChanged))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
        }

    }
}
