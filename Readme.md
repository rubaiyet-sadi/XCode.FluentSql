# FluentSql

FluentSql in a C# project that converts c# code to sql string.


### Example
See test project for more examples.
* Where
```c#
    var sqlFromOneEntityWithWhere = 
		QueryFactory
			.AsMySqlQuery<Customer>()
			.Select(c => new
			{
				c.Id,
				c.FirstName,
				c.MiddleInitial,
				c.LastName,
				c.DateofBirth,
				c.SSN
			})
			.Where(c => c.FirstName == "AA" && c.DateofBirth < DateTime.Now.AddDays(60))
			.ToString();
```
* Join
```c#
    var twoEntitiesWithJoin = 
        QueryFactory
        .AsMySqlQuery<Customer, CustomerOrders>()
        .Select((c, o) => new
        {
            c.Id,
            c.FirstName,
            c.LastName,
            c.DateofBirth,
            o.CustomerId,
            orderId = o.Id,
            o.OrderDate
        })
        .InnerJoin((c,o) => c.Id == o.CustomerId)
        .Where((c, o) => o.OrderDate >= DateTime.Now.AddDays(-14).Date && c.FirstName == "AAA")
        .ToString();
```

License
----

MIT
