using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoHelperQueryApp
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public string ShipCity { get; set; }
    }
}
