//      *********    このファイルを編集しないでください     *********
//      このファイルはデザイン ツールにより作成されました。
//      このファイルに変更を加えるとエラーが発生する場合があります。
namespace Expression.Blend.DataStore.DataStore
{
    using System;
    using System.Collections.Generic;

    public class DataStoreGlobalStorage
    {
        public static DataStoreGlobalStorage Singleton;
        public bool Loading {get;set;}
        private List<WeakReference> registrar; 

        public DataStoreGlobalStorage()
        {
            this.registrar = new List<WeakReference>();
        }
        
        static DataStoreGlobalStorage()
        {
            Singleton = new DataStoreGlobalStorage();
        }

        public void Register(DataStore dataStore)
        {
            this.registrar.Add(new WeakReference(dataStore));
        }

        public void OnPropertyChanged(string property)
        {
            foreach (WeakReference entry in this.registrar)
            {
                if (!entry.IsAlive)
                {
                    continue;
                }
                DataStore dataStore = (DataStore)entry.Target;
                dataStore.FirePropertyChanged(property);
            }
        }
        
        public bool AssignementAllowed
        {
            get
            {
                if(this.Loading && this.registrar.Count > 0)
                {
                    return false;
                }
                
                return true;
            }
        }
    }

    public class DataStore : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        public void FirePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        public DataStore()
        {
            try
            {
                System.Uri resourceUri = new System.Uri("/YukkuriUtil;component/DataStore/DataStore/DataStore.xaml", System.UriKind.Relative);
                if (System.Windows.Application.GetResourceStream(resourceUri) != null)
                {
                    DataStoreGlobalStorage.Singleton.Loading = true;
                    System.Windows.Application.LoadComponent(this, resourceUri);
                    DataStoreGlobalStorage.Singleton.Loading = false;
                    DataStoreGlobalStorage.Singleton.Register(this);
                }
            }
            catch (System.Exception)
            {
            }
        }
    }
}
