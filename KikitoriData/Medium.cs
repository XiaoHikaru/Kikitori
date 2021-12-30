// Kikitori
// (C) 2021, Andreas Gaiser

namespace Kikitori.Data
{
    public class Medium : Entity
    {
        // Title is assumed to be unique
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
