﻿#region usings

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Heath.Lister.Infrastructure.Models;
using Color = Heath.Lister.Infrastructure.Models.Color;

#endregion

namespace Heath.Lister.Infrastructure
{
    public class ListerData : IDisposable
    {
        private readonly ListerDataContext _dataContext;

        private bool _disposed;

        public ListerData()
        {
            _dataContext = new ListerDataContext();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void Initialize()
        {
            if (_dataContext.DatabaseExists())
                return;

            _dataContext.CreateDatabase();

            _dataContext.Colors.InsertAllOnSubmit(
                new List<Color>
                {
                    new Color { Id = Guid.NewGuid(), Text = "blue", R = 27, G = 161, B = 226 },
                    new Color { Id = Guid.NewGuid(), Text = "brown", R = 160, G = 80, B = 0 },
                    new Color { Id = Guid.NewGuid(), Text = "green", R = 51, G = 153, B = 51 },
                    new Color { Id = Guid.NewGuid(), Text = "lime", R = 162, G = 193, B = 57 },
                    new Color { Id = Guid.NewGuid(), Text = "magenta", R = 216, G = 0, B = 115 },
                    new Color { Id = Guid.NewGuid(), Text = "mango", R = 240, G = 150, B = 9 },
                    new Color { Id = Guid.NewGuid(), Text = "pink", R = 230, G = 113, B = 184 },
                    new Color { Id = Guid.NewGuid(), Text = "purple", R = 162, G = 0, B = 255 },
                    new Color { Id = Guid.NewGuid(), Text = "red", R = 229, G = 20, B = 0 },
                    new Color { Id = Guid.NewGuid(), Text = "teal", R = 0, G = 171, B = 169 }
                });

            _dataContext.SubmitChanges();

            if (_dataContext.Lists.Any())
                return;

            var lime = _dataContext.Colors.Single(c => c.Text == "blue");
            var welcome = new List { Id = Guid.NewGuid(), Color = lime, Title = "welcome!", CreatedDate = DateTime.Now };

            _dataContext.Lists.InsertOnSubmit(welcome);

            _dataContext.Items.InsertOnSubmit(
                new Item
                {
                    Id = Guid.NewGuid(), 
                    Title = "tap me!", 
                    Notes = "Thank you for downloading Lister! For questions, comments or suggestions about this app please email listerapp@hotmail.com.", 
                    List = welcome, 
                    DueDate = DateTime.Now.Date, 
                    Priority = Priority.Low, 
                    CreatedDate = DateTime.Now
                });

            _dataContext.SubmitChanges();
        }

        public IEnumerable<Color> GetColors()
        {
            return _dataContext.Colors.ToList();
        }

        public IEnumerable<List> GetLists()
        {
            var dataLoadOptions = new DataLoadOptions();

            dataLoadOptions.LoadWith<List>(l => l.Items);
            dataLoadOptions.AssociateWith<List>(l => l.Items.Where(i => !i.Deleted));

            _dataContext.LoadOptions = dataLoadOptions;

            return _dataContext.Lists.Where(l => !l.Deleted).ToList();
        }

        public IEnumerable<string> GetListTitles()
        {
            return _dataContext.Lists.Select(i => i.Title).Distinct().ToList();
        }

        public List GetList(Guid id, bool complete)
        {
            if (complete)
            {
                var dataLoadOptions = new DataLoadOptions();

                dataLoadOptions.LoadWith<List>(l => l.Items);
                dataLoadOptions.AssociateWith<List>(l => l.Items.Where(i => !i.Deleted));

                _dataContext.LoadOptions = dataLoadOptions;
            }
            return _dataContext.Lists.Single(l => l.Id == id);
        }

        public List UpsertList(Guid id, Guid colorId, string title)
        {
            List retval;

            if (id != Guid.Empty)
                retval = _dataContext.Lists.Single(l => l.Id == id);

            else
            {
                retval = new List();
                retval.CreatedDate = DateTime.Now;
                retval.Id = Guid.NewGuid();
                _dataContext.Lists.InsertOnSubmit(retval);
            }

            retval.Color = _dataContext.Colors.Single(c => c.Id == colorId);
            retval.Title = title;

            _dataContext.SubmitChanges();

            return retval;
        }

        public void DeleteList(Guid id)
        {
            var list = _dataContext.Lists.Single(l => l.Id == id);

            foreach (var item in list.Items)
                item.Deleted = true;

            list.Deleted = true;
            _dataContext.SubmitChanges();
        }

        public IEnumerable<string> GetItemTitles()
        {
            return _dataContext.Items.Select(i => i.Title).Distinct().ToList();
        }

        public Item GetItem(Guid id)
        {
            return _dataContext.Items.Single(i => i.Id == id);
        }

        public Item UpsertItem(Guid id, Guid listId, bool completed, DateTime? dueDate, DateTime? dueTime, string notes, Priority priority, string title)
        {
            Item retval;

            if (id != Guid.Empty)
                retval = _dataContext.Items.Single(i => i.Id == id);

            else
            {
                retval = new Item();
                retval.Id = Guid.NewGuid();
                retval.CreatedDate = DateTime.Now;
                retval.List = _dataContext.Lists.Single(l => l.Id == listId);
                _dataContext.Items.InsertOnSubmit(retval);
            }

            retval.Completed = completed;
            retval.DueDate = dueDate;
            retval.DueTime = dueTime;
            retval.Notes = string.IsNullOrWhiteSpace(notes) ? null : notes;
            retval.Priority = priority;
            retval.Title = title;

            _dataContext.SubmitChanges();

            return retval;
        }

        public void UpdateItem(Guid id, bool completed)
        {
            var item = _dataContext.Items.Single(i => i.Id == id);

            item.Completed = completed;
            _dataContext.SubmitChanges();
        }

        public void DeleteItem(Guid id)
        {
            var item = _dataContext.Items.Single(i => i.Id == id);

            item.Deleted = true;
            _dataContext.SubmitChanges();
        }

        ~ListerData()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dataContext.Dispose();
            }

            _disposed = true;
        }
    }
}