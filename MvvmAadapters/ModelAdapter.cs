using System;
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
        public ModelAdapter(T model) 
        {
            Model = model ?? throw new ArgumentNullException("model");
        }

        public T Model { get; private set; }


        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            return (TValue)propertyInfo.GetValue(Model);

            
        }

        protected void SetValue<TValue>(TValue value,[CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            var CurrentValue = propertyInfo.GetValue(Model);

            if (!Equals(CurrentValue,value))
            {
                propertyInfo.SetValue(Model, value);
                OnPropertyChanged(propertyName);
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
