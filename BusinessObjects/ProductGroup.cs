using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects
{
    public partial class ProductGroup
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
