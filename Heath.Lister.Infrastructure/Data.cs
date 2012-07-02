#region usings

using System.Data.Linq;
using Heath.Lister.Infrastructure.Models;

#endregion

namespace Heath.Lister.Infrastructure
{
    internal class Data : DataContext
    {
        private const string ConnectionString = "isostore:/lister.sdf";

        public Data()
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