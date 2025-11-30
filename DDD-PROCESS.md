**Backend Typical simplified task Workflow**

[[_TOC_]]

# Introduction

Here we`re going to demonstrate the process of BE development for a new feature.

## **_Reasons to use simplified approach_**

Usually OLTP project have 80-90% of typical queries/commands.
And creating repository methods with proper POCO objects, mappers and combinatoric numbers of select methods by different criteria and by different returning entity types causes a lot of blunt copypasts and mistakes.

Also, developers almost always create more universal select method, which reads all entity's fields insted of creating of many specific ones for each necessary case. This causes cumulative hurt of DB + Network performance and dramatic decrease it for some cases (for example, auto-included props).

Also using generic repo allows to concentrate all the work inside of Handler. This allows developer to concentrate his attention on the task itself, not on the routine copypasts.

Difference of LinQ in cases adding Where clauses to the queries for different providers is minimal. But this allows us to combine them independently of Entity structure and necessary filters.

So, having an option to use generic repository for the routine operations instead of copypasting specific one increase the quality, reliability and resilience of our product.
IMHO

## **_Hypothetic Task description_**

Create a controller method, which should return a list of customers for accounting department, filtered by country code. If country filter is empty, method should return all customers.
Customers request should look like:

```
.../api/customers/list-for-accounting?Country=GB
```

The response should look like:

```
{
  "totalItems": 0,
  "items": [
    {
      "id": 0,
      "code": "string",
      "name": "string",
      "taxNumber": "string",
      "dateRegistered": "2025-11-25T11:21:53.098Z"
    }
  ]
}
```

Since SampleCustomers table does not contain these fields, them should be added:

```
TaxNumber varchar(50),
dateRegistered date
```

# **_Implementation and testing_**

## **Migration creation**

Add new fields to the proper table
(`Osereda.Boilerplate.Domain.Entities.SampleCustomer` in this case) :

```
		public string? TaxNumber { get; set; }
		public DateTime DateRegistered { get; set; }
```

Set up physical storage details to the proper EntityConfiguration
(`Sai.DealAssistant.Infrastructure.EntityConfigurations.SampleCustomerConfiguration` in this case) :

```
            builder.Property(c => c.TaxNumber)
				.HasColumnType("varchar")
				.HasMaxLength(50);
			builder.Property(c => c.DateRegistered)
				.HasColumnType("date");
```

Create migration and apply it to the Local DB for the future tests

## **_Application level classes:_**

### **DTO**

DTO is what we are going to return as a result of command handing.
It must contain only props you need to return from the controller method.
We usually base our Controller`s method response on it.
DTO should be POCO object with default constructor.
Can contain MapperProfile

```
        public class SampleCustomerForAccountingDto
	{
		public int Id { get; set; }
		public string Code { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string? TaxNumber { get; set; }
		public DateTime DateRegistered { get; set; }

		public class MappingProfile : Profile
		{
			public MappingProfile()
			{
				CreateMap<SampleCustomer, SampleCustomerDto>();
			}
		}
	}
```

### **Query(or Command)**

Query and Command represent the input parameters.
Technically it is the same (`: IRequest<TOurDto>`), but logically query is a kind of command, which doesn`t change data in the database.
Query/Command has been processed by MediatR's Send method using appropriate Handler class.
MediatR uses Handler's Generic parameter to map it to Query/Command.
It is a great idea to use Query/Command class as a namespace for Handler/Validator/MapperProfile (In other words, put these classes inside of Query/Command class).

```
	public class GetSampleCustomersForAccountingQuery : IRequest<QueryResult<SampleCustomerForAccountingDto>>
	{
		public string? Country { get; set; }
                // Put Handler class here
                // Put Validator class here if you need a validation
                // Put MapperProfile class here if necessary
        }
```

### **Handler**

Handler class is a heart of our work.
Inject all necessary repositories, loggers, etc... via constructor.
Prefer generic repository, it allows to do almost everything including sophisticated joins of several tables, etc... with the lowest effort.
Implement all the real work in the Handler.Handle method (Except validation).
Use UnitTests to debug instead of running app and inputing data via Swagger - it is easy and saves quite a bit of time.
Using UnitTests to debug helps your cover with tests the majority of cases including the ones, you`re not able to predict.
Also unit tests will be the best documentation for your class.

```
		public class Handler : IRequestHandler<GetSampleCustomersForAccountingQuery, QueryResult<SampleCustomerForAccountingDto>>
		{
			private readonly IReadRepository<SampleCustomer> _customerRepository;

			public Handler(IReadRepository<SampleCustomer> customerRepository)
			{
				_customerRepository = customerRepository;
			}

			public async Task<QueryResult<SampleCustomerForAccountingDto>> Handle(
				GetSampleCustomersForAccountingQuery request, CancellationToken cancellationToken)
			{
                            //Implement all real work here (Except validation)

            }
        }

```

### **Handler implementation example**

```
public async Task<QueryResult<SampleCustomerForAccountingDto>> Handle(
	GetSampleCustomersForAccountingQuery request, CancellationToken cancellationToken)
{

```

Start writing Query from calling GetAll. It returns IQueriable and do nothing with database

```
	IQueryable<SampleCustomer> qry = _customerRepository.GetAll();
```

Apply your filters using all the power of LinQ.
You can add as many where clauses as you would like to.
Create your query dynamically depending on the input parameters - LinQ to SQL converts it into really good performance SQL.

```
	if (request.Country != null && !string.IsNullOrEmpty(request.Country))
	{
		qry = qry.Where(p => p.Country == request.Country);
	}

```

Use `CountAsync` to get rowcount. Please draw your attention, that you can use the same qry for the several purposes (e.g. CountAsync and select)

```
	int totalItems = await _customerRepository.CountAsync(qry);
```

Use SelectAsync to select table Columns / column expressions into the DTO.
Please do not select unnecessary fields here - it causes early pessimization and potentially hurt DB and network performance.
Do not use AutoMapper here. It causes the early pessimization, because selects all the entity props first. (This hurts performance very seriously for the auto-included navigational props)

```
	IReadOnlyCollection<SampleCustomerForAccountingDto> result = await _customerRepository.SelectAsync(
		qry,
		p => new SampleCustomerForAccountingDto
		{
			Id = p.Id,
			Code = p.Code,
			Name = p.Name,
			TaxPayerScheme = p.TaxPayerScheme,
			TaxNumber = p.TaxNumber,
			RegistrationDate = p.RegistrationDate
		});
```

Create result to return from the Controller method

```
	return new QueryResult<SampleCustomerForAccountingDto>(result, totalItems);
}
```

Please take a look at the IReadRepository and ICrudRepository and those tests for more information about generic repositories features

### **Handler UnitTesting**

Unit testing of Handlers is pretty easy:
Just inherit your test class from `UnitTestBase` create your test data in the test body or in the constructor if you like to reuse this data in all tests in this testClass. This base class creates a mock SQLite in-memory database for you and create a new AppDbContext (DbContext prop) Use it to prepare test data within test method

```
	public class GetSampleCustomersForAccountingQuery_Handler_Test : UnitTestBase
	{
		private ReadRepository<SampleCustomer> _customerRepositary;
```

Set base(seedTestData: false) into True if you like to seed sample data, which are already seeded into the local DB

```
		public GetSampleCustomersForAccountingQuery_Handler_Test()
			: base(seedTestData: false)
		{
			_customerRepositary = new ReadRepository<SampleCustomer>(DbContext);
```

Create your test data which are common for all tests in this file inside of using new Benefits Context in constructor. Use DbContext prop to manipulate with test data inside of the test method. This is necessary to guarantee from tracking the same row attempt problem.

```
			using (var db = CreateNewDbContext())
			{
```

Use Fixtire prop to build test data if you like.
Use Fixture.Customize to set up different aspects of certain entity creation. Like this:

```
                            Fixture.Customize<SampleEmployee>(p =>
                                p.With(c => c.CustomerId, _customer.Id)
                                .Without(c => c.Id)
                                .Without(c => c.Customer));
```

Please be careful, because every call of Customize method cleans up previous calls result for this entity type.

Then generate some data

```
                            // Create sample data using fixture
                            var customers = Fixture.CreateMany<SampleCustomer>(100);
                            //Modify generated data if necessary
                            //...
                            //Add data to the proper DbSet
                            db.SampleCustomers.AddRange(customers);

```

Do not forget SaveChanges to database

```
				db.SaveChanges();
			}

		[Fact]
		public async void GetSampleCustomersForAccountingQuery_Handler_SelectByCountrReturnsUs()
		{
			// Arrange
```

Get benchmark data from the database.
It can be datasets, rowcounts, etc.
You will compare the result of examining method with the benchmark data in the assert section.
Please do not use something like `Assert.Equals(5, rowcount)`. You can waste time fixing tests when you need to update the test source data.
Instead, select rowcount into benchmark variable in the arrange section. And compare actual value with it : `Assert.Equals(benchmarkRowcount, rowcount)`.
The same is about datasets.

```
			string firstCountry = DbContext.SampleCustomers.First().Country;
			List<SampleCustomerForAccountingDto> benchmarkCustomersList = DbContext.SampleCustomers
				.Where(p => p.Country == firstCountry)
				.Select(p => new SampleCustomerForAccountingDto
				{
					Id = p.Id,
					Code = p.Code,
					Name = p.Name,
					TaxNumber = p.TaxNumber,
					DateRegistered = p.DateRegistered
				}).ToList();
			// Act
			var handler = new Handler(_customerRepositary);
			var command = new GetSampleCustomersForAccountingQuery { Country = firstCountry };
			var result = await handler.Handle(command, CancellationToken.None);
			// Assert
			Assert.True(benchmarkCustomersList.Count > 0, $"No {firstCountry} customers found");
			Assert.Equal(benchmarkCustomersList.Count, result.Items.Count);

                        // Use this powerful method, it deeply compares objects and really works fine
			Assert.Equivalent(benchmarkCustomersList, result.Items);
		}
	}
```

When you bumped into the examining class behavior, you didn`t expect, please cover it with the UnitTest right after you agree this behavior with FE subteam. It usually takes just a little while, just copypaste existing test and slightly change.

### **Validators**

Validators usually does not make sense for Queries (when we select data from DB).
However, they are necessary do validate data for Inserts and Updates and even Deletes.
Validators allow to notify FE in a stadard way and make it possible to process validation results on the FE.
So, below is an example of validator for `CreateSampleCustomerCommand`. Just inject repos and other stuff you need and write rules for fields needed to be validated:

```
         public class Validator : AbstractValidator<CreateSampleCustomerCommand>
		{
			private readonly IReadRepository<SampleCustomer> _customerRepository;

			public Validator(IReadRepository<SampleCustomer> customerRepository)
			{
				_customerRepository = customerRepository;

				RuleFor(c => c.Code)
					.NotEmpty()
					.MustAsync(async (cmd, code, cToken) => !await _customerRepository.ExistsAsync(p => p.Code == code))
					.WithMessage("Code already exist.");
				RuleFor(c => c.Name)
					.NotEmpty();
			}
		}
```

Validators should be tested using SQLite In-memory DB in the same way as the Handler class above.

## **_Infrastructure level classes:_**

### **Generic repository**

Generic repository is already implemented and well-tested.
It should be treated as already done library.

### **Specific repositories**

Each specific repository should be tested using SQLite In-memory DB in the same way as the Handler class above.

### **Handlers with Specific repositories**

After specific repositories , used in the handler developed and tested, handler can be tested in 2 ways:

- Classic unit testing, when we mock repository
- Pseudo-Integration testing: We don't mock repository, just using real one instead and mock data in database like we do it for the Handler in an example above.

Both of them are good, so, it is up to developer and the cases to test, which one should be used.

### **External service clients**

They should be tested with HttpRequest mocker:
https://github.com/richardszalay/mockhttp is used in other projects

## **_WebApi level classes:_**

### **Controllers**

Controllers should be integration tested.
Integration tests should use PostmanCollections, developer builds as a psrt of ticket.
PostBot helps to automate them
Usually PostmanCollection contains allthe endpoints calls with some valid or invalid data
