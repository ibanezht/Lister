#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Heath.Lister.Infrastructure.Models
{
    [Table]
    public class Item : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private bool _completed;
        private DateTime _createdDate;
        private bool _deleted;
        private DateTime? _dueDate;
        private DateTime? _dueTime;
        private Guid _id;
        private EntityRef<List> _list;

        [Column]
        private Guid _listId;

        private string _notes;
        private Priority _priority;
        private string _title;

        [Column(IsVersion = true)]
        private Binary _version;

        [Column]
        public bool Completed
        {
            get { return _completed; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Completed"));
                _completed = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("Completed"));
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

        [Column]
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("DueDate"));
                _dueDate = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("DueDate"));
            }
        }

        [Column]
        public DateTime? DueTime
        {
            get { return _dueTime; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("DueTime"));
                _dueTime = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("DueTime"));
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

        [Association(Storage = "_list", ThisKey = "_listId", OtherKey = "Id", IsForeignKey = true)]
        public List List
        {
            get { return _list.Entity; }
            set
            {
                if (value == null)
                    return;

                OnNotifyPropertyChanging(new PropertyChangingEventArgs("List"));
                _list.Entity = value;
                _list.Entity.Items.Add(this);
                _listId = value.Id;
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("List"));
            }
        }

        [Column]
        public string Notes
        {
            get { return _notes; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Notes"));
                _notes = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("Notes"));
            }
        }

        [Column]
        public Priority Priority
        {
            get { return _priority; }
            set
            {
                OnNotifyPropertyChanging(new PropertyChangingEventArgs("Priority"));
                _priority = value;
                OnNotifyPropertyChanged(new PropertyChangedEventArgs("Priority"));
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