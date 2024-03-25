# Cache
### Ref
	https://www.c-sharpcorner.com/article/working-with-a-distributed-cache-in-asp-net-core/
	https://gunnarpeipman.com/aspnet-core-sql-server-cache/

### Set absolute and sliding expiration on a cached item
	DistributedCacheEntryOptions options = 
								new DistributedCacheEntryOptions();
								options.AbsoluteExpiration = DateTime.Now.AddMinutes(2);
								options.SlidingExpiration = TimeSpan.FromMinutes(2);
            
								cache.SetString("timestamp", 
								DateTime.Now.ToString(), options);
### Save objec to cache
	Employee emp = new Employee();
	emp.EmployeeID = 1;
	emp.FirstName = "Nancy";
	emp.LastName = "Davolio";
	string value = JsonConvert.SerializeObject(emp);
	cache.SetString("employee", value);

### CacheTable
	CREATE TABLE [dbo].[CacheTable](
		[Id] [nvarchar](449) NOT NULL,
		[Value] [varbinary](max) NOT NULL,
		[ExpiresAtTime] [datetimeoffset](7) NOT NULL,
		[SlidingExpirationInSeconds] [bigint] NULL,
		[AbsoluteExpiration] [datetimeoffset](7) NULL,
	 CONSTRAINT [pk_Id] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
		   IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, 
		   ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
	CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime] ON [dbo].[CacheTable]
	(
		[ExpiresAtTime] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
		   SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, 
		   ONLINE = OFF, ALLOW_ROW_LOCKS = ON, 
		   ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
