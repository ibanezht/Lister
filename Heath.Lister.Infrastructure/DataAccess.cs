#region usings

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Heath.Lister.Infrastructure.Models;

#endregion

namespace Heath.Lister.Infrastructure
{
    public class DataAccess : IDisposable
    {
        private readonly Data _data;

        private bool _disposed;

        public DataAccess()
        {
            _data = new Data();
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
            if (_data.DatabaseExists())
                return;

            _data.CreateDatabase();

            _data.Colors.InsertAllOnSubmit(
                new List<Color>
                {
                    new Color { Id = Guid.NewGuid(), Text = "lime", R = 164, G = 196, B = 0 },
                    new Color { Id = Guid.NewGuid(), Text = "green", R = 96, G = 169, B = 23 },
                    new Color { Id = Guid.NewGuid(), Text = "emerald", R = 0, G = 138, B = 0 },
                    new Color { Id = Guid.NewGuid(), Text = "teal", R = 0, G = 171, B = 169 },
                    new Color { Id = Guid.NewGuid(), Text = "cyan", R = 27, G = 161, B = 226 },
                    new Color { Id = Guid.NewGuid(), Text = "cobalt", R = 0, G = 80, B = 239 },
                    new Color { Id = Guid.NewGuid(), Text = "indigo", R = 106, G = 0, B = 255 },
                    new Color { Id = Guid.NewGuid(), Text = "violet", R = 170, G = 0, B = 255 },
                    new Color { Id = Guid.NewGuid(), Text = "pink", R = 244, G = 114, B = 208 },
                    new Color { Id = Guid.NewGuid(), Text = "magenta", R = 216, G = 0, B = 115 },
                    new Color { Id = Guid.NewGuid(), Text = "crimson", R = 162, G = 0, B = 37 },
                    new Color { Id = Guid.NewGuid(), Text = "red", R = 229, G = 20, B = 0 },
                    new Color { Id = Guid.NewGuid(), Text = "orange", R = 250, G = 104, B = 0 },
                    new Color { Id = Guid.NewGuid(), Text = "amber", R = 240, G = 163, B = 10 },
                    new Color { Id = Guid.NewGuid(), Text = "yellow", R = 227, G = 200, B = 0 },
                    new Color { Id = Guid.NewGuid(), Text = "brown", R = 130, G = 90, B = 44 },
                    new Color { Id = Guid.NewGuid(), Text = "olive", R = 109, G = 135, B = 100 },
                    new Color { Id = Guid.NewGuid(), Text = "steel", R = 100, G = 118, B = 135 },
                    new Color { Id = Guid.NewGuid(), Text = "mauve", R = 118, G = 96, B = 138 },
                    new Color { Id = Guid.NewGuid(), Text = "taupe", R = 135, G = 121, B = 78 },
                });

            _data.SubmitChanges();

            if (_data.Lists.Any())
                return;

            var steel = _data.Colors.Single(c => c.Text == "steel");
            var welcome = new List { Id = Guid.NewGuid(), Color = steel, Title = "welcome!", CreatedDate = DateTime.Now };

            _data.Lists.InsertOnSubmit(welcome);

            _data.Items.InsertOnSubmit(
                new Item
                {
                    Id = Guid.NewGuid(),
                    Title = "tap me!",
                    Notes = "Thank you for downloading Lister! For questions, comments or suggestions about this app please email listerapp@hotmail.com.",
                    List = welcome,
                    DueDate = DateTime.Now.Date,
                    Priority = Priority.None,
                    CreatedDate = DateTime.Now
                });

            _data.SubmitChanges();
        }

        public IEnumerable<Color> GetColors()
        {
            return _data.Colors.ToList();
        }

        public IEnumerable<List> GetLists()
        {
            var dataLoadOptions = new DataLoadOptions();

            dataLoadOptions.LoadWith<List>(l => l.Items);
            dataLoadOptions.AssociateWith<List>(l => l.Items.Where(i => !i.Deleted));

            _data.LoadOptions = dataLoadOptions;

            return _data.Lists.Where(l => !l.Deleted).ToList();
        }

        public IEnumerable<string> GetListTitles()
        {
            return _data.Lists.Select(i => i.Title).Distinct().ToList();
        }

        public List GetList(Guid id, bool complete)
        {
            if (complete)
            {
                var dataLoadOptions = new DataLoadOptions();

                dataLoadOptions.LoadWith<List>(l => l.Items);
                dataLoadOptions.AssociateWith<List>(l => l.Items.Where(i => !i.Deleted));

                _data.LoadOptions = dataLoadOptions;
            }
            return _data.Lists.Single(l => l.Id == id);
        }

        public List UpsertList(Guid id, Guid colorId, string title)
        {
            List retval;

            if (id != Guid.Empty)
                retval = _data.Lists.Single(l => l.Id == id);

            else
            {
                retval = new List();
                retval.CreatedDate = DateTime.Now;
                retval.Id = Guid.NewGuid();
                _data.Lists.InsertOnSubmit(retval);
            }

            retval.Color = _data.Colors.Single(c => c.Id == colorId);
            retval.Title = title;

            _data.SubmitChanges();

            return retval;
        }

        public void DeleteList(Guid id)
        {
            var list = _data.Lists.Single(l => l.Id == id);

            foreach (var item in list.Items)
                item.Deleted = true;

            list.Deleted = true;
            _data.SubmitChanges();
        }

        public IEnumerable<string> GetItemTitles()
        {
            return _data.Items.Select(i => i.Title).Distinct().ToList();
        }

        public Item GetItem(Guid id)
        {
            return _data.Items.Single(i => i.Id == id);
        }

        public Item UpsertItem(Guid id, Guid listId, bool completed, DateTime? dueDate, DateTime? dueTime, string notes, Priority priority, string title)
        {
            Item retval;

            if (id != Guid.Empty)
                retval = _data.Items.Single(i => i.Id == id);

            else
            {
                retval = new Item();
                retval.Id = Guid.NewGuid();
                retval.CreatedDate = DateTime.Now;
                retval.List = _data.Lists.Single(l => l.Id == listId);
                _data.Items.InsertOnSubmit(retval);
            }

            retval.Completed = completed;
            retval.DueDate = dueDate;
            retval.DueTime = dueTime;
            retval.Notes = string.IsNullOrWhiteSpace(notes) ? null : notes;
            retval.Priority = priority;
            retval.Title = title;

            _data.SubmitChanges();

            return retval;
        }

        public void UpdateItem(Guid id, bool completed)
        {
            var item = _data.Items.Single(i => i.Id == id);

            item.Completed = completed;
            _data.SubmitChanges();
        }

        public void DeleteItem(Guid id)
        {
            var item = _data.Items.Single(i => i.Id == id);

            item.Deleted = true;
            _data.SubmitChanges();
        }

        ~DataAccess()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _data.Dispose();
            }

            _disposed = true;
        }
    }
}