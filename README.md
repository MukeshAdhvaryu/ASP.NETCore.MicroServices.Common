# ASP.NETCore.MicroServices.Common.Template

## dotnet new install ASP.NETCore.Dynamic.MicroService 

then anywhere you want to create the template use:

## dotnet new install micro
OR
## dotnet new install microservice

Using WebAPI template for creating microservices is a faster and more efficient way.
Creating a microservice by choosing from .NET templates is a standard way to get started. 
However, after creating our project from the template, we have to perform a lot of other tasks to customize it. 
For example, installing NuGet packages, creating controllers and service repositories etc. 
Since a controller can have standard HTTP calls such as HttpGet, HttpPost, etc., we already know the possible methods of the controller class. 
The only variable I can see is your model, which can differ from project to project. 
So, the idea is: To use MicroService.Common shared project in your actual project and then define your model.
Then make the follwing two calls in your Program.cs:
MicroService.Common.Core.Configuration.AddMvc(builder);
MicroService.Common.Core.Configuration.AddModel\<Your Model Interface, Your Model Implemetation, Primary Key Type of Your Model\>(builder);

This two calls should be enough to bind your model in the system.
For each AddModel(...) call, the specified model can be associated with dynamically created IService repository and a generic Controller.
In development mode, each call will also made the API availalable in swagger front end.

However, we must have a choice not to use automatically created controllers and to choose to use our own controllers instead.
There are 2 ways this can be done:
1. By using conditional compiler constant: MODEL_USEMYOWNCONTROLLER
2. By providing AutoController = false in Model Attribute and adorn your model with the \[Model()\] attribute.

To control a contract of operations on a project-to-project basis, we can use the following conditional compilation symbols:
1. MODEL_APPENDABLE - this is to have HttpPost capability.
2. MODEL_DELETABLE - this is to have HttpDelete capability.
3. MODEL_UPDATABLE - this is to have HttpPut capability.
4. MODEL_USEDTO - this is to allow the usage of DTOs.
5. MODEL_NONREADABLE - this is to skip HTTPGet methods from getiing defined.
    
The follwing Generic Type definitions are used throughout the project:
TID where TID : struct. (To represent a primary key type of the model).

TModelDTO  where TModelDaTO : IModel (Interface implemented by your model).

TModel where TModel : Model\<TID\>,

#if (!MODEL_USEDTO)

TModelDTO,

#endif

new()

Concrete (non abstract) model defined by you must derive from an abstract Model\<TID\> class provided.

you can choose to provide your own service repository implementation by:
1. Inheriting from and then overriding methods of Service\<TModelDTO, TModel, TID, TModelCollection\>
2. Create your brand new service repository by implementing IService\<TModelDTO, TModel, TID\>

If you want to customize controller binding by not using auto generated controllers then.. 
1. You must create your controller - inherited from Controller\<TModelDTO, TModel, TID\> class.
2. Adorn your model implementation with attribute:
   \[Model(AutoController = false)\]. 

If you want your model to provide seed data when DBContext\<\> is empty then.. 
1. You must override GetInitialData() method to provide a list of created models.
2. Adorn your model implementation with attribute: \[Model(ProvideSeedData = true)].

If you want your model to specify a scope of attached service then.. 
1.  Adorn your model implementation with attribute:
2.  \[Model(Scope = ServiceScope.Your Choice)].

By default, DBContext\<TModel, TID\> uses InMemory SqlLite by using "InMemory" connection string stored in configuration.

That's it. 

## UPDATE: A single test project is created.

### This project is with bare minimum code and can be used commonly for all frameworks i.e. xUnit, NUnit or MSTest.

Which framework will be used can be decided by a user simply by defining compiler constants MODEL_USEXUNIT or MODEL_USENUNIT. 
If neither of those constants defined then MSTest will be used.

## UPDATE: Criteria based search feature for models added.

Try FindAll (ISearchParameter searchParameter) method.
  
## UPDATE: Support for ClassDta and MemberData attributes added.

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

## UPDATE: Feature to perform search for multiple models using multiple search parameters added.

Try FindAll (IEnumerable\<ISearchParameter\> searchParameter) method.
