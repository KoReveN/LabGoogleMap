using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; } //- Rate(1-10 integer)
        public string Phone { get; set; }

        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public Point Point { get; set; }
        public int PintId { get; set; }
        public Address Address { get; set; }

    }
}
