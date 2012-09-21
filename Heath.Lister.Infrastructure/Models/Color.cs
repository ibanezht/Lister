#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Heath.Lister.Infrastructure.Models
{
    [Table]
    public class Color : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private readonly EntitySet<List> _lists = new EntitySet<List>();

        private byte _b;
        private byte _g;
        private Guid _id;
        private byte _r;
        private string _text;

        [Column(IsVersion = true)] private Binary _version;

        [Column]
        public byte B
        {
            get { return _b; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("B"));
                _b = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("B"));
            }
        }

        [Column]
        public byte G
        {
            get { return _g; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("G"));
                _g = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("G"));
            }
        }

        [Column(IsPrimaryKey = true)]
        public Guid Id
        {
            get { return _id; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Id"));
                _id = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("Id"));
            }
        }

        [Association(Storage = "_lists", ThisKey = "Id", OtherKey = "_colorId")]
        public EntitySet<List> Lists
        {
            get { return _lists; }
            set { _lists.Assign(value); }
        }

        [Column]
        public byte R
        {
            get { return _r; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("R"));
                _r = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("R"));
            }
        }

        [Column]
        public string Text
        {
            get { return _text; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Text"));
                _text = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("Text"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        protected virtual void OnNotifyPropertyChanging(PropertyChangingEventArgs e)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, e);
        }
    }
}