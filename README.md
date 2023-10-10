# ASP.NETCore.MicroServices.Common.Template

## dotnet new install ASP.NETCore.Dynamic.MicroService 

then anywhere you want to create the template use:

## dotnet new install micro
OR
## dotnet new install microservice

Using WebAPI template for creating microservices is a faster and more efficient way.
Creating a microservice by choosing from .NET templates is a standard way to get started. 

## Index
[WHY](#WHY)
[WHAT](#WHAT)
[HOW](#HOW)

## WHY?
We already know that a controller can have standard HTTP calls such as HttpGet, HttpPost, etc.
So we know the possible methods of the controller class. 

The problem is: definition of model (entity) which can differ from project to project as different business domains have different entities.  
However, it pained me to start a new project from 'WeatherForecast' template and then write almost everything from the scratch.

## WHAT?
On one fine day, it dwaned upon me that I need to explore a possibility of creating something which addresses the moving part of the microservice device: Model.
The goal I set was: to make a model centric project where everything which I will need: 
a controller, a db-context, a repository, an exception middleware can be defined and controlled at the model level only.

The hard part was to define a generic controller and generic db-context to work for any given model.
A generic service repository should not be a problem though.

### I wanted that a user need to do the minimum to get every thing bound together and work without any issue.

### I also wanted that the user should have a choice to define a contract of operations; 
for example:
To have a read-only (query) controller generated dynamically.
Or
To have a write-only (command) controller generated dynamically
Or to have both.
At last, the user should be able choose to use dynamically generated controller or their own controller (going back to 'WeatherForecast')

### TDD is also an integeral part of any project. Creating a common template that handles the three most prominent testing frameworks (xUNIT, NUNIT and MSTest) should not be easy.

The goal was also to include a common test project to handle all three without the user need to change much except any custom test they want to write.
It would be an apt thing to do to define custom attributes to map importnat attributes from all three testing frameworks.

### Support for keyless (query) models was also to be provided (perhapes not at the begining of the project).
Keyed models should be flexible enough to use various common keys such as int, long, Guid, enum etc.

## To handle under-fetching and over-fetching problems,
Option was to be provided to use interfaces and DTOs as input argument in POST/PUT and as an output argument in GET calls.

## The project was to end with CQRS (Command and Query Segregation) adoptation.

## HOW

To provide supports for the above mentioned, the following CCC (Conditional Compilation Constants) were came to my mind:

To control a contract of operations on a project-to-project basis:
1. MODEL_APPENDABLE - this is to have HttpPost capability.
2. MODEL_DELETABLE - this is to have HttpDelete capability.
3. MODEL_UPDATABLE - this is to have HttpPut capability.
4. MODEL_USEDTO - this is to allow the usage of DTOs.
5. MODEL_NONREADABLE - this is to skip HTTPGet methods from getiing defined.
6. MODEL_NONQUERYABLE - this is to skip HTTPGet methods from getiing defined in regualr controller as well as query controller as well.
7. MODEL_USEMYOWNCONTROLLER - this is to prevent the template from generating dynamic controllers under the fair presumption that
   you will provide your own controller class implementation.

For testing:
1. TDD - this is to stay in TDD environment (before writing any ASP WEB-API code).
2. MODEL_ADDTEST - this is to enable support for service and standard test templates
3. TEST_USERMODELS - this is to test actual models defined by the user as opposed to test model provided.
4. MODEL_USEXUNIT - this is to choose xUnit as testing framework.
5. MODEL_USENUNIT - this to choose NUnit as testing framework

In absense of any of the last two CCCs, MSTest should be used as default test framework.


To handle under-fetching and over fetching
1. MODEL_USEDTO

    
The follwing Generic Type definitions are used throughout the project:
TID where TID : struct. (To represent a primary key type of the model - no string key support sorry!).

TOutDTO  where TOutDTO : IModel (Root interface of all models).
TInDTO  where TInDTO : IModel (Root interface of all models).

TModel where TModel : class, ISelfModel\<TID, TModel\>, new()

#if (!MODEL_USEDTO)

, TOutDTO,

#endif

For keyless support:
TModel where TModel : class, ISelfModel\<TModel\>, new()

Concrete (non abstract) keyed model defined by you must derive from an abstract Model\<TID, TModel\> class provided.
Concrete (non abstract) keyless model defined by you must derive from an abstract Model\<TModel\> class provided.

you can choose to provide your own Command and Query implementation by:
Inheriting from and then overriding methods of Query\<TOutDTO, TModel\> and Command\<TOutDTO, TModel\> classes respectively.


If you must choose your own controller for your model then.. 
1. Create your controller - inherited from Controller\<TModelDTO, TModel, TID, TInDTO\> class.
2. Adorn your model with attribute: \[Model(AutoController = false)\]. 
OR use MODEL_USEMYOWNCONTROLLER ccc.

If you want your model to provide seed data when DBContext is empty then.. 
1. You must override GetInitialData() method of the Model\<TModel\> class to provide a list of created models.
2. Adorn your model with attribute: \[Model(ProvideSeedData = true)].

If you want your model to specify a scope of attached service then.. 
1.  Adorn your model with attribute: \[Model(Scope = ServiceScope.Your Choice)].

By default, DBContext uses InMemory SqlLite by using "InMemory" connection string stored in configuration.

That's it. 

# UPDATE: A single test project is created.

### This project is with bare minimum code and can be used commonly for all frameworks i.e. xUnit, NUnit or MSTest.

Which framework will be used can be decided by a user simply by defining compiler constants MODEL_USEXUNIT or MODEL_USENUNIT. 
If neither of those constants defined then MSTest will be used.

# UPDATE: Criteria based search feature for models added.

Try FindAll (ISearchParameter searchParameter) method.
  
# UPDATE: Support for ClassData and MemberData attributes added.

ClassData attribute is mapped to: ArgSourceAttribute\<T\> where T: ArgSource
ArgSource is an abstract class with an abstract property IEnumerable<object[]> Data {get; }
You will have to inherit from this class and provide your own data and then you can use

This is an example on how to use source member data.
To use member data, you must define a static method or property returning IEnumerable<object[]>.


    [WithArgs]
    [ArgSource(typeof(MemberDataExample), "GetData")]
    public Task GetAll_ReturnAllUseMemberData(int limitOfResult = 0)
    {
        //
    }

This is an example on how to use source class data.
To use class data, ArgSource\<source\> will suffice.


    [WithArgs]
    [ArgSource\<ClassDataExample\>]
    public Task GetAll_ReturnAllUseClassData(int limitOfResult = 0)
    {
        //
    
    }
    
 Then, those classes can be defined in the following manner:
 
    class MemberDataExample 
    {
        public static IEnumerable<object[]> GetData   
        {
            get
            {
                yield return new object[] { 0 };
                yield return new object[] { 3 };
                yield return new object[] { -1 };
            }
        }
    }

    class ClassDataExample: ArgSource 
    {

        public override IEnumerable<object[]> Data  
        {
                get
                {
                    yield return new object[] { 0 };
                    yield return new object[] { 3 };
                    yield return new object[] { -1 };
                }
        }
    }

# UPDATE: Feature to perform search for multiple models using multiple search parameters added.

Try FindAll (IEnumerable\<ISearchParameter\> searchParameter) method.

# UPDATE: Added Exception Middleware.

Middleware type: IExceptionFiter type

# UPDATE: Added Support for IActionResult for controller. 

So, Now we have support for IActionResult and actual object return types.
Use conditional compiler constant: MODEL_USEACTION

# UPDATE: Feature: Choose database at model level.

To Use SQLServer:
1. define constant: MODEL_CONNECTSQLSERVER
2. In configuration, define connection string with key "SQLServer".
3. At your model level, use model attribute with ConnectionKey = ConnectionKey.SQLServer

To Use PostgreSQL:
1. define constant: MODEL_CONNECTPOSTGRESQL
2. In configuration, define connection string with key "PostgreSQL".
3. At your model level, use model attribute with ConnectionKey = ConnectionKey.PostgreSQL

To Use MySQL:
1. define constant: MODEL_CONNECTMYSQL
2. In configuration, define connection string with key "MySQL".
3. At your model level, use model attribute with ConnectionKey = ConnectionKey.MySQL

Please note that, regardless of any of these,
1. if connectionstring is empty, InMemory SQLLite will be used as default.
2. Don't worry about downloading relevant package from nuget.
3. Defining constant will automatically download the relevant package for you.

# UPDATE Controller class: 4th Type TInDTO included.

 So now it is Controller<TOutDTO, TModel, TID, TInDTO>

We can define different DTOs for Out (GET calls) and IN (POST, PUT calls).
We can still use any DTO for the both IN and OUT though.

# UPDATE Converted DBContext to non-generic

This is to allow single DBContext to hold multiple model sets..

# UPDATE Support for Query-Only-Controllers and Keyless models is added.

It is now possible to create separate controller for command and query purposes.

Use constant MODEL_NONREADABLE: this will create Command-only controller.
Then for the same model, call AddQueryModel() method, this will create Query-only controller.

# UPDATE Abstract Models for common primary key type: int, long, Guid, enum are added.

# UPDATE Adopted: CQRS pattern.


