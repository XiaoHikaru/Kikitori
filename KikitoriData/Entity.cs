// Kikitori
// (C) 2021, Andreas Gaiser

using SQLite;

namespace Kikitori.Data
{

    public class Entity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }

}
