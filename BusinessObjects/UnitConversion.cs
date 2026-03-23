using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects
{
    public partial class UnitConversion
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string UnitName { get; set; } = null!;

        public int ConversionRate { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
