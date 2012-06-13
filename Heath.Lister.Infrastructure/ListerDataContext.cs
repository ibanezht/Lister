#region usings

using System.Data.Linq;
using Heath.Lister.Infrastructure.Models;

#endregion

namespace Heath.Lister.Infrastructure
{
    internal class ListerDataContext : DataContext
    {
        private const string ConnectionString = "isostore:/lister.sdf";

        public ListerDataContext()
            : base(ConnectionString) {}

        public Table<Color> Colors
        {
            get { return GetTable<Color>(); }
        }

        public Table<Item> Items
        {
            get { return GetTable<Item>(); }
        }

        public Table<List> Lists
        {
            get { return GetTable<List>(); }
        }
    }
}