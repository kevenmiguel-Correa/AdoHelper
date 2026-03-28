using AdoHelperLib.Ado;
using AdoHelperQueryApp;
using Microsoft.Data.SqlClient;

var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;";
var context = new AdoHelperContext(connectionString);

Console.WriteLine("=== TEST MINI ORM ===");

// ----------------------------
// 1. QueryAsync
// ----------------------------
Console.WriteLine("\n1. Orders by Customer:");

var orders = await context.QueryAsync<OrdersWithCustomerDto>(
    "dbo.usp_GetOrdersWithCustomer"//,
    //new SqlParameter("@CustomerId", "ALFKI")
);

foreach (var o in orders)
{
    Console.WriteLine($"{o.OrderID} - {o.OrderDate} - {o.ShipCountry} - {o.CompanyName}");
}

// ----------------------------
// 2. ExecuteScalarAsync
// ----------------------------
Console.WriteLine("\n2. Order count:");

var count = await context.ExecuteScalarAsync<int>(
    "dbo.usp_GetOrderCountByCustomer",
    new SqlParameter("@CustomerId", "ALFKI")
);

Console.WriteLine($"Total Orders: {count}");

// ----------------------------
// 3. QuerySingleAsync
// ----------------------------
Console.WriteLine("\n3. Single customer:");

var customer = await context.QuerySingleAsync<CustomerDto>(
    "dbo.usp_GetCustomerFullData",
    new SqlParameter("@CustomerId", "ALFKI")
);

Console.WriteLine($"{customer.CompanyName} - {customer.Country}");

// ----------------------------
// 4. QueryMultipleAsync (LA PRUEBA CLAVE)
// ----------------------------
Console.WriteLine("\n4. Multiple result sets:");

await using (var multi = await context.QueryMultipleAsync(
    "dbo.usp_GetCustomerFullData",
    new SqlParameter("@CustomerId", "ALFKI")))
{
    var cust = await multi.ReadAsync<CustomerDto>();
    var ords = await multi.ReadAsync<OrderDto>();
    var details = await multi.ReadAsync<OrderDetailDto>();

    Console.WriteLine($"Customer: {cust.FirstOrDefault()?.CompanyName}");
    Console.WriteLine($"Orders count: {ords.Count}");
    Console.WriteLine($"Details count: {details.Count}");
}

// ----------------------------
// 5. ExecuteAsync (INSERT)
// ----------------------------
//Console.WriteLine("\n5. Insert test:");

//var rows = await context.ExecuteAsync(
//    "dbo.usp_CreateCustomer",
//    new SqlParameter("@CustomerID", "ZZZZZ"),
//    new SqlParameter("@CompanyName", "Test Company"),
//    new SqlParameter("@Country", "DR")
//);

//Console.WriteLine($"Rows affected: {rows}");

Console.WriteLine("\n=== END TEST ===");