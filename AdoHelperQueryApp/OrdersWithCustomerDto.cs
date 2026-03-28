using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoHelperQueryApp
{
    public class OrdersWithCustomerDto
    {
        public string? OrderID { get; set; }
        public string? CompanyName { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? ShipCountry { get; set; }
    }
}
