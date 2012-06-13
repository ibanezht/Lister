#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Heath.Lister.Infrastructure.Models
{
    [Table]
    public class List : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private readonly EntitySet<Item> _items = new EntitySet<Item>();

        private EntityRef<Color> _color;

        [Column]
        private Guid _colorId;

        private DateTime _createdDate;
        private bool _deleted;
        private Guid _id;
        private string _title;

        [Column(IsVersion = true)]
        private Binary _version;

        [Association(Storage = "_color", ThisKey = "_colorId", OtherKey = "Id", IsForeignKey = true)]
        public Color Color
        {
            get { return _color.Entity; }
            set
            {
                if (value == null)
                    return;

                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Color"));
                _color.Entity = value;
                _color.Entity.Lists.Add(this);
                _colorId = value.Id;
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Color"));
            }
        }

        [Column]
        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("CreatedDate"));
                _createdDate = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("CreatedDate"));
            }
        }

        [Column]
        public bool Deleted
        {
            get { return _deleted; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Deleted"));
                _deleted = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("Deleted"));
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

        [Column]
        public string Title
        {
            get { return _title; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Title"));
                _title = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("Title"));
            }
        }

        [Association(Storage = "_items", OtherKey = "_listId", ThisKey = "Id")]
        public EntitySet<Item> Items
        {
            get { return _items; }
            set { _items.Assign(value); }
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