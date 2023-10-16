# ASP.NETCore.MicroServices.Common.Template

## dotnet new install ASP.NETCore.Dynamic.MicroService 

then anywhere you want to create the template use:

## dotnet new install micro
OR
## dotnet new install microservice

Using WebAPI template for creating microservices is a faster and more efficient way.
Creating a microservice by choosing from .NET templates is a standard way to get started. 

## Index
[WHY?](#WHY)

[WHAT?](#WHAT)

[HOW?](#HOW)

[Design](#Design)

[Design_Of_Model](#Design_Of_Model)

[UPDATE1: Common test project for all three frameworks i.e. xUnit, NUnit or MSTest.](#UPDATE1)

[UPDATE2: Criteria based search feature for models added.](#UPDATE2)

[UPDATE3: Support for ClassData and MemberData test attributes added.](#UPDATE3)

[UPDATE4: Feature to perform search for multiple models using multiple search parameters added.](#UPDATE4)

[UPDATE5: Added Exception Middleware.](#UPDATE5)

[UPDATE6: Added Support for IActionResult for controller. ](#UPDATE6)

[UPDATE7: Feature: Choose database at model level.](#UPDATE7)

[UPDATE8: Controller class: 4th Type def TInDTO included (input for POST/PUT calls).](#UPDATE8)

[UPDATE9: Converted DBContext from generic to non-generic class.](#UPDATE9)

[UPDATE10: Support for Query-Only-Controllers and Keyless models is added.](#UPDATE10)

[UPDATE11: Abstract Models for common primary key type: int, long, Guid, enum are added.](#UPDATE11)

[UPDATE12: Adopted Command and Query Segregation pattern.](#UPDATE12)


## WHY?
We already know that a controller can have standard HTTP calls such as HttpGet, HttpPost, etc.
So, we know the possible methods of the controller class. 

The problem is: definition of model (entity) which can differ from project to project as different business domains have different entities.  
However, it pained me to start a new project from 'WeatherForecast' template and then write almost everything from the scratch.

We do need something better than that. A template powerful enough to get adopted in most common cases with bare minimum modifications.

## WHAT?
On one fine day, it dawned upon me that I need to explore a possibility of creating something which addresses the moving part of the microservice device: Model.
The goal I set was: to make a model centric project where everything which I will need: 
a controller, a DbContext, a repository, an exception middleware can be defined and controlled at the model level only.

The hard part was to define a generic controller and generic DbContext to work for any given model.
A generic service repository should not be a problem though.

### I wanted that a user need to do the minimum to get every thing bound together and work without any issue.

### I also wanted that the user should have a choice to define a contract of operations; 
for example:
To have a read-only (query) controller generated dynamically.
Or
To have a write-only (command) controller generated dynamically
Or to have both.
At last, the user should be able choose to use dynamically generated controller or their own controller (going back to 'WeatherForecast')

### TDD is also an integral part of any project. Creating a common template that handles the three most prominent testing frameworks (xUNIT, NUNIT and MSTest) should not be easy.

The goal was also to include a common test project to handle all three without the user need to change much except any custom test they want to write.
It would be an apt thing to do to define custom attributes to map important attributes from all three testing frameworks.

### Support for keyless (query) models was also to be provided (perhaps not at the begining of the project).
Keyed models should be flexible enough to use various common keys such as int, long, Guid, enum etc.

## To handle under-fetching and over-fetching problems,
Option was to be provided to use interfaces and DTOs as input argument in POST/PUT and as an output argument in GET calls.

## The project was to end with CQRS (Command and Query Segregation) adaptation.

## HOW

To provide supports for the above mentioned, the following CCC (Conditional Compilation Constants) were came to my mind:

To control a contract of operations on a project-to-project basis:
1. MODEL_APPENDABLE - this is to have HttpPost capability.
2. MODEL_DELETABLE - this is to have HttpDelete capability.
3. MODEL_UPDATABLE - this is to have HttpPut capability.
4. MODEL_NONREADABLE - this is to skip HttpGet methods from getting defined.
5. MODEL_NONQUERYABLE - this is to skip HttpGet methods from getting defined in regular controller as well as query controller as well.
6. MODEL_USEMYOWNCONTROLLER - this is to prevent the template from generating dynamic controllers under the fair presumption that
   you will provide your own controller class implementation.

For testing:
1. TDD - this is to stay in TDD environment (before writing any ASP WEB-API code).
2. MODEL_ADDTEST - this is to enable support for service and standard test templates
3. TEST_USERMODELS - this is to test actual models defined by the user as opposed to test model provided.
4. MODEL_USEXUNIT - this is to choose xUnit as testing framework.
5. MODEL_USENUNIT - this to choose NUnit as testing framework

In absence of any of the last two CCCs, MSTest should be used as default test framework.


To handle under-fetching and over-fetching:

1. MODEL_USEDTO - this is to allow the usage of DTOs.

    
The following Generic Type definitions are used throughout the project:

     TID where TID : struct. (To represent a primary key type of the model - no string key support sorry!).
     TOutDTO  where TOutDTO : IModel (Root interface of all models).
     TInDTO  where TInDTO : IModel (Root interface of all models).
     
     TModel where TModel : class, ISelfModel<TID, TModel>, new()
     #if (!MODEL_USEDTO)
     , TOutDTO,
     #endif
     
     For keyless support:
     TModel where TModel : class, ISelfModel<TModel>, new()
 

Concrete (non-abstract) keyed model defined by you must derive from an abstract Model\<TID, TModel\> class provided.
Concrete (non-abstract) keyless model defined by you must derive from an abstract Model\<TModel\> class provided.

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

## Design_Of_Model

1. Defined an abstract layer called Microserives.Common
    This layer will have no awareness of any Web API controllers or DbContext. 
    One should be able to use it for something else. 
    For example, for minimal APIs we do not use controllers and DbContexts.
    In TDD mode, all we need is to make sure that contract operations on a given model works.
    We should be able to create test projects before we even create an actual Web API project.
 2. Operation contracts are defined in three categories:
     a. Query contract (read-only)
     b. Command contract (write-only)
     c. Contract - (read-only or read - write)
 3. Contract interfaces are formed by inheriting either Command interface or Query interface or both.
 
        public interface IContract<TOutDTO, TModel, TID> : IContract
            #if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
                , IQuery<TOutDTO, TModel, TID>
            #endif
            #if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
                , ICommand<TOutDTO, TModel, TID>
            #endif
        where TOutDTO : IModel
        where TModel : class, ISelfModel<TID, TModel>,
            #if (!MODEL_USEDTO)
                TOutDTO,
            #endif
        new()
        where TID : struct
        {
        }
        
        public interface IQueryContract<TOutDTO, TModel> : IContract
            #if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
                , IQuery<TOutDTO, TModel>
            #endif
        where TOutDTO : IModel
        where TModel : ISelfModel<TModel>, new()
            #if (!MODEL_USEDTO)
                , TOutDTO
            #endif
        
        { 
        }

        #if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
            public interface ICommand<TOutDTO, TModel, TID> : IModelCount
                #if MODEL_APPENDABLE
                    , IAppendable<TOutDTO, TModel, TID>
                #endif
                #if MODEL_UPDATABLE
                    , IUpdatable<TOutDTO, TModel, TID>
                #endif
                #if MODEL_DELETABLE
                    , IDeleteable<TOutDTO, TModel, TID>
                #endif

            where TOutDTO : IModel
            where TModel : class, ISelfModel<TID, TModel>, new()
                #if (!MODEL_USEDTO)
                    , TOutDTO
                #endif
            where TID : struct
            {

            }
        #endif

        #if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
            public partial interface IQuery<TOutDTO, TModel> : IModelCount, IFind<TOutDTO, TModel>, IFirstModel<TModel>
            where TModel : ISelfModel<TModel>
            where TOutDTO : IModel
            { 
            }

            public interface IQuery<TOutDTO, TModel, TID> : IQuery<TOutDTO, TModel>, IFindByID<TOutDTO, TModel, TID>
            where TOutDTO : IModel
            where TModel : class, ISelfModel<TID, TModel>, new()
                #if (!MODEL_USEDTO)
                    , TOutDTO
                #endif
            where TID : struct
            {
            }
        #endif

 4. Single repsonsibility interfaces:
      1. IAppendable\<TOutDTO, TModel, TID\>
      2. IUpdatable\<TOutDTO, TModel, TID\>
      3. IDeleteable\<TOutDTO, TModel, TID\>
      4. IFindByID\<TOutDTO, TModel, TID\>
      5. IFind\<TOutDTO, TModel\>

        #if MODEL_DELETABLE
            public interface IDeleteable<TOutDTO, TModel, TID>
            where TOutDTO : IModel
            where TModel : ISelfModel<TID, TModel>, new()
                #if (!MODEL_USEDTO)
                    , TOutDTO
                #endif
            where TID : struct
            {       
                Task<TOutDTO?> Delete(TID id);
            }
        #endif
        
        #if MODEL_APPENDABLE
            public interface IAppendable<TOutDTO, TModel, TID>
            where TOutDTO : IModel
            where TModel : ISelfModel<TID, TModel>, new()
                #if (!MODEL_USEDTO)
                    , TOutDTO
                #endif            
            where TID : struct
            {        
                Task<TOutDTO?> Add(IModel? model);
            }
        #endif
        
        #if MODEL_UPDATABLE
            public interface IUpdatable<TOutDTO, TModel, TID>
            where TOutDTO : IModel
            where TModel : ISelfModel<TID, TModel>, new()
                #if (!MODEL_USEDTO)
                    TOutDTO,
                #endif            
            where TID : struct
            {     
                Task<TOutDTO?> Update(TID id, IModel? model);
            }
        #endif

        #if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
            public interface IFindByID<TOutDTO, TModel, TID>
            where TOutDTO : IModel
            where TModel : ISelfModel<TID, TModel>
                #if (!MODEL_USEDTO)
                    , TOutDTO
                #endif
            where TID : struct
            {
                Task<TOutDTO?> Get(TID? id);
            }
            
            public interface IFind<TOutDTO, TModel>
            where TOutDTO : IModel
            where TModel : IModel
            {
                Task<IEnumerable<TOutDTO>?> GetAll(int limitOfResult = 0);
                Task<IEnumerable<TOutDTO>?> GetAll(int startIndex, int limitOfResult);
                Task<TOutDTO?> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = 0);
                Task<TOutDTO?> Find(ISearchParameter? parameter);
                Task<IEnumerable<TOutDTO>?> FindAll(ISearchParameter? parameter);
                Task<IEnumerable<TOutDTO>?> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = 0);
            }
        #endif     

## Design: Model
   1. IModel
   2. IModel\<TID\>
   3. ISelfModel\<TModel\>
   4. ISelfModel\<TID, TModel\> 

    /// <summary>
    /// This interface represents a model.
    /// Highly customizable by using the following conditional compilation symbols:
    /// MODEL_DELETABLE; MODEL_APPENDABLE; MODEL_UPDATABLE; MODEL_USEMYOWNCONTROLLER
    /// </summary>
    public interface IModel
    { 
    }

    /// <summary>
    /// This interface represents a model with primary key named as ID.
    /// Highly customizable by using the following conditional compilation symbols:
    /// MODEL_DELETABLE;
    /// MODEL_APPENDABLE;
    /// MODEL_UPDATABLE;
    /// MODEL_USEMYOWNCONTROLLER
    /// </summary>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IModel<TID> : IModel, IMatch
        where TID : struct
    {
        /// <summary>
        /// Gets primary key of this model.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        TID ID { get; }
    }
 
    /// <summary>
    /// This interface represents a self-referencing model.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public partial interface ISelfModel<TModel> : IModel, IMatch
        where TModel : ISelfModel<TModel>
    {
    }
    
    /// <summary>
    /// This interface represents a self-referencing model with the primary key of type TID.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public partial interface ISelfModel<TID, TModel> : ISelfModel<TModel>, IModel<TID>
        where TModel : ISelfModel<TID, TModel>, IModel<TID>
        where TID : struct
    {
    }

As we already talked about a model centric approach, the following internal interfaces are the key to achieve that.
    1. IExModel 
    2. IExModel\<TID\>

    /// <summary>
    /// This interface represents a model with primary key named as ID.
    /// </summary>
    internal partial interface IExModel : IModel, IExCopyable, IExParamParser, IExModelExceptionSupplier
    #if MODEL_USEDTO
        , IExModelToDTO
    #endif
    {
        /// <summary>
        /// Provides a list of names of properties - must be handled while copying from data supplied from model binder's BindModelAsync method.
        /// If the list is not provided, System.Reflecteion will be used to obtain names of the properties defined in this model.
        /// </summary>
        IReadOnlyList<string> GetPropertyNames(bool forSearch = false);

        /// <summary>
        /// Gets initial data.
        /// </summary>
        /// <returns>IEnumerable\<IModel\> containing list of initial data.</returns>
        IEnumerable<IModel> GetInitialData();
    }
        
    /// <summary>
    /// This interface represents a model with primary key named as ID.
    /// Highly customizable by using the following conditional compilation symbols:
    /// MODEL_DELETABLE;
    /// MODEL_APPENDABLE;
    /// MODEL_UPDATABLE;
    /// MODEL_USEMYOWNCONTROLLER
    /// </summary>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    internal interface IExModel<TID> : IModel<TID>, IExModel
        where TID : struct
    {
        /// <summary>
        /// gets primary key value of this model.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        new TID ID { get; set; }

        /// <summary>
        /// Gets unique id.
        /// </summary>
        /// <returns>Newly generated id.</returns>
        TID GetNewID();

        /// <summary>
        /// Tries to parse the given value to the type of ID
        /// Returns parsed value if succesful, otherwise default value.
        /// </summary>
        /// <param name="value">Value to be parsed as TID.</param>
        /// <param name="newID">Parsed value.</param>
        /// <returns>True if succesful, otherwise false</returns>
        bool TryParseID(object value, out TID newID);
    }
    #endregion

Single repsonsibility interfaces:
   1. IMatch
   2. IExCopyable
   3. IExParamParser
   4. IExModelExceptionSupplier       

    /// <summary>
    /// Reprents an object which checks if certqain value exists.
    /// </summary>
    public interface IMatch
    {
        /// <summary>
        /// Finds whether the given value matches the current value of property found by specified property name.
        /// </summary>
        /// <param name="searchParameter">Search parameter to use to match records in this object.</param>
        /// <returns>True if values match, otherwise false.</returns>
        
        bool IsMatch(ISearchParameter? searchParameter);
    } 
    
    /// <summary>
    /// This interface represents an object that copies data from another model.
    /// </summary>
    internal interface IExCopyable
    {
        /// <summary>
        /// Copies model data from the given model parameter.
        /// </summary>
        /// <param name="model">Model to copy data from.</param>
        /// <returns>True if the copy operation is successful; otherwise, false.</returns>
        
        Task<bool> CopyFrom(IModel model);
    }

    /// <summary>
    /// This interface represents an object that offers parameter parsing capability.
    /// Provided, the given property exist as one of its members.
    /// </summary>
    internal interface IExParamParser
    {
        /// <summary>
        /// Parses the specified parameter and if possible emits the value compitible with
        /// the property this object posseses.
        /// </summary>
        /// <param name="parameter">Parameter to parse.</param>
        /// <param name="currentValue">Current value exists for the given property in this object.</param>
        /// <param name="parsedValue">If succesful, a compitible value parsed using supplied value from parameter.</param>
        /// <param name="updateValueIfParsed">If succesful, replace the current value with the compitible parsed value.</param>
        /// <returns>Result Message with Status of the parse operation.</returns>
        /// <param name="criteria">Criteria to be used when parsing value.</param>
       
       Message Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed = false, Criteria criteria = 0);
    }
    
    /// <summary>
    /// This interface represents an object which supplies an appropriate exception for a failure in a specified method.
    /// </summary>
    internal interface IExModelExceptionSupplier
    {
        /// <summary>
        /// Supplies an appropriate exception for a failure in a specified method.
        /// </summary>
        /// <param name="exceptionType">Type of exception to get.</param>
        /// <param name="additionalInfo">Additional information to aid the task of exception supply.</param>
        /// <param name="innerException">Inner exception which is already thrown.</param>
        /// <returns>Instance of SpecialException class.</returns>
       
       ModelException GetModelException(ExceptionType exceptionType, string? additionalInfo = null, Exception? innerException = null);
    }

Now consider an implementation of all of the above to conjure up the model centric design:

    public abstract partial class Model<TModel> : ISelfModel<TModel>, IExModel, IMatch
        where TModel : Model<TModel>, ISelfModel<TModel>
    {
        #region VARIABLES
        readonly string modelName;
        #endregion

        #region CONSTRUCTOR
        protected Model()
        {
            var type = GetType();
            modelName = type.Name;
        }
        #endregion

        #region PROPERTIES
        public string ModelName => modelName;
        #endregion

        #region GET PROPERTY NAMES
        /// <summary>
        /// Provides a list of names of properties - must be handled while copying from data supplied from model binder's BindModelAsync method.
        /// If the list is not provided, System.Reflecteion will be used to obtain names of the properties defined in this model.
        /// </summary>
        protected virtual IReadOnlyList<string> GetPropertyNames(bool forSearch = false) => null;
        
        IReadOnlyList<string> IExModel.GetPropertyNames(bool forSearch)
        {
            var propertyNames = GetPropertyNames(forSearch);

            if (propertyNames == null || propertyNames.Count == 0)
            {
                propertyNames = GetType().GetProperties().Where(p =>
                {
                    var attr = p.GetType().GetCustomAttribute<DatabaseGeneratedAttribute>();
                    if (attr?.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed)
                        return false;
                    return true;
                }).Select(p => p.Name).ToArray();
            }
            return propertyNames;
        }
        #endregion

        #region PARSE
        /// <summary>
        /// Parses the specified parameter and if possible emits the value compitible with
        /// the property this object posseses.
        /// </summary>
        /// <param name="parameter">Parameter to parse.</param>
        /// <param name="currentValue">Current value exists for the given property in this object.</param>
        /// <param name="parsedValue">If succesful, a compitible value parsed using supplied value from parameter.</param>
        /// <param name="updateValueIfParsed">If succesful, replace the current value with the compitible parsed value.</param>
        /// <returns>Result Message with Status of the parse operation.</returns>
        protected abstract Message Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed = false);
        Message IExParamParser.Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed, Criteria criteria)
        {
            var name = parameter.Name;
            parsedValue = null;
            object? value;
            
            switch (name)
            {
                default:
                    switch (criteria)
                    {
                        case Criteria.Occurs:
                        case Criteria.BeginsWith:
                        case Criteria.EndsWith:
                        case Criteria.OccursNoCase:
                        case Criteria.BeginsWithNoCase:
                        case Criteria.EndsWithNoCase:
                        case Criteria.StringEqual:
                        case Criteria.StringEqualNoCase:
                        case Criteria.StringNumGreaterThan:
                        case Criteria.StringNumLessThan:
                        case Criteria.NotOccurs:
                        case Criteria.NotBeginsWith:
                        case Criteria.NotEndsWith:
                        case Criteria.NotOccursNoCase:
                        case Criteria.NotBeginsWithNoCase:
                        case Criteria.NotEndsWithNoCase:
                        case Criteria.NotStrEqual:
                        case Criteria.NotStrEqualNoCase:
                        case Criteria.NotStringGreaterThan:
                        case Criteria.NotStringLessThan:
                            value = parameter is IModelParameter ? ((IModelParameter)parameter).FirstValue : parameter.Value;
                            parsedValue = value?.ToString();
                            Parse(parameter, out currentValue, out _, updateValueIfParsed);
                            return Message.Sucess(name);
                        default:
                            break;
                    }
                    break;
            }
            return Parse(parameter, out currentValue, out parsedValue, updateValueIfParsed);
        }
        #endregion

        #region COPY FROM
        /// <summary>
        /// Copies data from the given model to this instance.
        /// </summary>
        /// <param name="model">Model to copy data from.</param>
        /// <returns></returns>
        protected abstract Task<bool> CopyFrom(IModel model);

        Task<bool> IExCopyable.CopyFrom(IModel model) =>
            CopyFrom(model);
        #endregion

        #region GET INITIAL DATA
        /// <summary>
        /// Gets initial data.
        /// </summary>
        /// <returns>IEnumerable\<IModel\> containing list of initial data.</returns>
        protected abstract IEnumerable<IModel> GetInitialData();

        IEnumerable<IModel> IExModel.GetInitialData() =>
            GetInitialData();
        #endregion

        #region IsMATCH
        /// <summary>
        /// Matches property with specified name using criteria given and emits current value exists for the given property
        /// and a compitible value parsed using supplied value from parameter.
        /// </summary>
        /// <param name="propertyName">Name of property which to match for.</param>
        /// <param name="criteria">Criteria enum to specify on the grounds the match should perform.</param>
        /// <param name="currentValue">Current value exists for the given property.</param>
        /// <param name="parsedValue">If succesful, a compitible value parsed using supplied value from parameter.</param>
        /// <returns>True if values match, otherwise false.</returns>
        protected virtual bool IsMatch(string propertyName, Criteria criteria, object? currentValue, object? parsedValue)
        {
            if (parsedValue == null)
            {
                if (currentValue == null && criteria == Criteria.Equal)
                    return true;
                    
                return false;
            }
            return Operations.Compare(currentValue, criteria, parsedValue);
        }

        bool IMatch.IsMatch(ISearchParameter? parameter)
        {
            if(parameter == null) 
                return false; 
            var result = ((IExParamParser)this).Parse(parameter, out var currentValue, out var newValue, false, parameter.Criteria);
            switch (result.Status)
            {
                case ResultStatus.Sucess:
                    return IsMatch(parameter.Name, parameter.Criteria, currentValue, newValue);
                case ResultStatus.Failure:
                case ResultStatus.Ignored:
                case ResultStatus.MissingValue:
                case ResultStatus.MissingRequiredValue:
                default:
                    return false;
            }
        }
        #endregion

        #region GET APPROPRIATE EXCEPTION
        /// <summary>
        /// Supplies an appropriate exception for a failure in a specified method.
        /// </summary>
        /// <param name="exceptionType">Type of exception to get.</param>
        /// <param name="additionalInfo">Additional information to aid the task of exception supply.</param>
        /// <returns>Instance of SpecialException class.</returns>
        protected virtual ModelException GetAppropriateException(ExceptionType exceptionType, string? additionalInfo = null, Exception? innerException = null)
        {
            bool noAdditionalInfo = string.IsNullOrEmpty(additionalInfo);

            switch (exceptionType)
            {
                case ExceptionType.NoModelFound:
                case ExceptionType.NoModelFoundForID:
                    return ModelException.Create(string.Format("No {0} is found additional info: {1}", modelName, noAdditionalInfo ? "None" : "ID = " + additionalInfo), exceptionType, innerException);

                case ExceptionType.NoModelsFound:
                    return ModelException.Create(string.Format("No {0} are found additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.NoModelSupplied:
                    return ModelException.Create(string.Format("Null {0} can not be supplied additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.NegativeFetchCount:
                    return ModelException.Create(string.Format("{0} fetch count must be > 0; provided: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.ModelCopyOperationFailed:
                    return ModelException.Create(string.Format("Copy operation of {0} is failed; model provided: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.NoParameterSupplied:
                    return ModelException.Create(string.Format("Null parameter for searching a {0} is not allowed; additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.NoParametersSupplied:
                    return ModelException.Create(string.Format("Null parameters for searching  {0}s are not allowed; additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.AddOperationFailed:
                    return ModelException.Create(string.Format("Add operation for adding new {0} is failed; additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.UpdateOperationFailed:
                    return ModelException.Create(string.Format("Update operation for updating the {0} is failed; additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.DeleteOperationFailed:
                    return ModelException.Create(string.Format("Delete operation for deleting the {0} is failed; additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.InternalServerError:
                    return ModelException.Create(string.Format("Model {0}: internal server error; additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.ExpectationFailed:
                    return ModelException.Create(string.Format("Model {0}: expectation failed; additional info: {1}", modelName, additionalInfo ?? " None"), exceptionType, innerException);

                case ExceptionType.InvalidContext:
                    return ModelException.Create(string.Format("The supplied model context is not valid or compitible with the {0}", additionalInfo ?? " None"), exceptionType, innerException);

                default:
                    return ModelException.Create("Need to supply your message", ExceptionType.Unknown);
            }
        }

        ModelException IExModelExceptionSupplier.GetModelException(ExceptionType exceptionType, string? additionalInfo, Exception? innerException) =>
            GetAppropriateException(exceptionType, additionalInfo, innerException);
        #endregion

        #if MODEL_USEDTO
            #region IModelToDTO
            /// <summary>
            /// Provides compitible DTO of given type from this model.
            /// You must override this method to support dtos.
            /// </summary>
            /// <param name="type"></param>
            /// <returns>Compitible DTO.</returns>
            protected virtual IModel? ToDTO(Type type)
            {
                var t = GetType();
                if (type == t || t.IsAssignableTo(type))
                    return this;
                return null;
            }
            IModel? IExModelToDTO.ToDTO(Type type) =>
                ToDTO(type);
            #endregion
        #endif
    }



    public abstract partial class Model<TID, TModel> : Model<TModel>,
        IExModel<TID>, IExModel, ISelfModel<TID, TModel>
        where TID : struct
        where TModel : Model<TID, TModel>
    {
        #region VARIABLES
        TID id;
        #endregion

        #region CONSTRUCTOR
        protected Model(bool generateNewID)
        {
            if (generateNewID)
                id = GetNewID();
        }
        #endregion

        #region PROPERTIES
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TID ID { get => id; protected set => id = value; }
       
       TID IExModel<TID>.ID { get => id; set => id = value; }
        #endregion

        #region PARSE
        Message IExParamParser.Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed, Criteria criteria)
        {
            var name = parameter.Name;
            parsedValue = null;
            object? valueFromParameter;

            switch (name)
            {
                case nameof(ID):
                    currentValue = id;
                    valueFromParameter = parameter is IModelParameter ? ((IModelParameter)parameter).FirstValue : parameter.Value;
                    switch (criteria)
                    {
                        case Criteria.Occurs:
                        case Criteria.BeginsWith:
                        case Criteria.EndsWith:
                        case Criteria.OccursNoCase:
                        case Criteria.BeginsWithNoCase:
                        case Criteria.EndsWithNoCase:
                        case Criteria.StringEqual:
                        case Criteria.StringEqualNoCase:
                        case Criteria.StringNumGreaterThan:
                        case Criteria.StringNumLessThan:
                        case Criteria.NotOccurs:
                        case Criteria.NotBeginsWith:
                        case Criteria.NotEndsWith:
                        case Criteria.NotOccursNoCase:
                        case Criteria.NotBeginsWithNoCase:
                        case Criteria.NotEndsWithNoCase:
                        case Criteria.NotStrEqual:
                        case Criteria.NotStrEqualNoCase:
                        case Criteria.NotStringGreaterThan:
                        case Criteria.NotStringLessThan:
                            parsedValue = valueFromParameter?.ToString();
                            return Message.Sucess(name);
                        default:
                            break;
                    }
                    if (valueFromParameter == null)
                        goto EXIT_ID;
                    if (((IExModel<TID>)this).TryParseID(valueFromParameter, out TID newID))
                    {
                        parsedValue = newID;
                        if (updateValueIfParsed && Equals(id, default(TID)))
                            id = newID;

                        return Message.Sucess(name);
                    }
                    EXIT_ID:
                    return Message.Ignored(name);
                default:
                    switch (criteria)
                    {
                        case Criteria.Occurs:
                        case Criteria.BeginsWith:
                        case Criteria.EndsWith:
                        case Criteria.OccursNoCase:
                        case Criteria.BeginsWithNoCase:
                        case Criteria.EndsWithNoCase:
                        case Criteria.StringEqual:
                        case Criteria.StringEqualNoCase:
                        case Criteria.StringNumGreaterThan:
                        case Criteria.StringNumLessThan:
                        case Criteria.NotOccurs:
                        case Criteria.NotBeginsWith:
                        case Criteria.NotEndsWith:
                        case Criteria.NotOccursNoCase:
                        case Criteria.NotBeginsWithNoCase:
                        case Criteria.NotEndsWithNoCase:
                        case Criteria.NotStrEqual:
                        case Criteria.NotStrEqualNoCase:
                        case Criteria.NotStringGreaterThan:
                        case Criteria.NotStringLessThan:
                            valueFromParameter = parameter is IModelParameter ? ((IModelParameter)parameter).FirstValue : parameter.Value;
                            parsedValue = valueFromParameter?.ToString();
                            Parse(parameter, out currentValue, out _, updateValueIfParsed);
                            return Message.Sucess(name);
                        default:
                            break;
                    }
                    break;
            }
            return Parse(parameter, out currentValue, out parsedValue, updateValueIfParsed);
        }
        #endregion

        #region COPY FROM
        Task<bool> IExCopyable.CopyFrom(IModel model)
        {
            if (model is IModel<TID>)
            {
                var m = (IModel<TID>)model;
                if (Equals(id, default(TID)))
                    id = m.ID;
                return CopyFrom(model);
            }

            //-:cnd:noEmit            
            #if MODEL_USEDTO
            if (model is IModel)
            {
                if (Equals(id, default(TID)))
                    id = GetNewID();
                return CopyFrom(model);
            }
            
            #endif
            //+:cnd:noEmit
            return Task.FromResult(false);
        }
        #endregion

        #region GET NEW ID
        protected abstract TID GetNewID();
        TID IExModel<TID>.GetNewID() =>
            GetNewID();
        #endregion

        #region TRY PARSE ID
        /// <summary>
        /// Tries to parse the given value to the type of ID
        /// Returns parsed value if succesful, otherwise default value.
        /// </summary>
        /// <param name="value">Value to be parsed as TIDType.</param>
        /// <param name="newID">Parsed value.</param>
        /// <returns>True if succesful, otherwise false</returns>
        protected abstract bool TryParseID(object value, out TID newID);
        bool IExModel<TID>.TryParseID(object value, out TID newID)
        {
            if (value is TID)
            {
                newID = (TID)value;
                return true;
            }
            if (value == null)
            {
                newID = default(TID);
                return false;
            }            
            return TryParseID(value, out newID);
        }
        #endregion
    }


That's it. 

## UPDATE1 
A single test project is created for each TDD and Non TDD environment.
One for testing a service in TDD environment:

        public abstract class ServiceTest<TOutDTO, TModel, TID>  
        where TOutDTO : IModel
        where TModel : Model<TID, TModel>,
        #if (!MODEL_USEDTO)
        TOutDTO,
        #endif
        new()
        where TID : struct
    {
        #region VARIABLES
        readonly IContract<TOutDTO, TModel, TID> Contract;
        protected readonly IFixture Fixture;

        static readonly IExModelExceptionSupplier DummyModel = new TModel();
        #if MODEL_USEDTO
        static readonly Type DTOType = typeof(TOutDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
        #endif
        #endregion

        #region CONSTRUCTOR
        public ServiceTest()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            Contract = CreateService();
        }
        #endregion

        #region CREATE SERVICE
        protected abstract IContract<TOutDTO, TModel, TID> CreateService();
        #endregion

        /*
         *Usual test methods with [WithArgs] or [NoArgs] attributes.
        */

        #region TO DTO
        #if (MODEL_USEDTO)
        protected TOutDTO? ToDTO(TModel? model)
        {
            if (model == null)
                return default(TOutDTO);
            if (NeedToUseDTO)
                return (TOutDTO)((IExModel)model).ToDTO(DTOType);
            return (TOutDTO)(object)model;
        }
        #else
        protected TOutDTO? ToDTO(TModel? model)
        {
            if(model == null)
                return default(TOutDTO);
            return (TOutDTO)(object)model;
        }
        #endif
        #endregion       
    }

One for Standard Web API testing (Controller via Service repository)
    
    public abstract class TestStandard<TOutDTO, TModel, TID, TInDTO>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TInDTO : IModel
        where TModel : Model<TID, TModel>,
        #if (!MODEL_USEDTO)
        TOutDTO,
        #endif
        new()
        where TID : struct
        #endregion
    {
        #region VARIABLES
        protected readonly Mock<IService<TOutDTO, TModel, TID>> MockService;
        readonly IContract<TOutDTO, TModel, TID> Contract;
        protected readonly List<TModel> Models;
        protected readonly IFixture Fixture;

        static readonly IExModelExceptionSupplier DummyModel = new TModel();
        #if MODEL_USEDTO
        static readonly Type DTOType = typeof(TOutDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
        #endif
        #endregion

        #region CONSTRUCTOR
        public TestStandard()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            MockService = Fixture.Freeze<Mock<IService<TOutDTO, TModel, TID>>>();
            Contract = CreateContract(MockService.Object);
            var count = DummyModelCount;
            if (count < 5)
                count = 5;
            Models = Fixture.CreateMany<TModel>(count).ToList();
        }
        #endregion

        #region PROPERTIES
        #if (MODEL_USEDTO)
        protected IEnumerable<TOutDTO> Items => Models.Select(x => ToDTO(x));
        #else
        protected IEnumerable<TOutDTO> Items => (IEnumerable<TOutDTO>)Models;
        #endif

        protected virtual int DummyModelCount => 5;
        #endregion

        #region CREATE CONTROLLER
        protected abstract IContract<TOutDTO, TModel, TID> CreateContract(IService<TOutDTO, TModel, TID> service);
        #endregion

        #region SETUP FUNCTION
        protected void Setup<TResult>(Expression<Func<IService<TOutDTO, TModel, TID>, Task<TResult>>> expression, TResult returnedValue)
        {
            MockService.Setup(expression).ReturnsAsync(returnedValue);
        }
        protected void Setup<TResult>(Expression<Func<IService<TOutDTO, TModel, TID>, Task<TResult>>> expression, Exception exception)
        {
            MockService.Setup(expression).Throws(exception);
        }
        #endregion        

        #region TO DTO
        #if (MODEL_USEDTO)
        protected TOutDTO? ToDTO(TModel? model)
        {
            if (model == null)
                return default(TOutDTO);
            if (NeedToUseDTO)
            {
                var result = ((IExModel)model).ToDTO(DTOType);
                if(result == null)
                    return default(TOutDTO);

                return (TOutDTO)result;
            }
            return (TOutDTO)(object)model;
        }
        #else
        protected TOutDTO? ToDTO(TModel? model)
        {
            if(model == null)
                return default(TOutDTO);
            return (TOutDTO)(object)model;
        }
        #endif
        #endregion
    }

### These classes can be used commonly for all frameworks i.e. xUnit, NUnit or MSTest.

Which framework will be used can be decided by a user simply by defining compiler constants MODEL_USEXUNIT or MODEL_USENUNIT. 
If neither of those constants defined then MSTest will be used.

## UPDATE2
Criteria based search feature for models added.

Try FindAll (ISearchParameter searchParameter) method.
Have a look at the Operations.cs class to know how generic comparison methods are defined.

## UPDATE3
Support for ClassData and MemberData test attributes added.

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
    [ArgSource<ClassDataExample>]
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

## UPDATE4
Feature to perform search for multiple models using multiple search parameters added.

Try FindAll (IEnumerable\<ISearchParameter\> searchParameter) method.
ParamBinder class code updated to handle parsing of multiple parameters.

    public sealed class ParamBinder: ModelCreator, IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var descriptor = (ControllerActionDescriptor)bindingContext.ActionContext.ActionDescriptor;
            /* 
                The following call will create an empty model according to TModel type defined in a controller
                The method 'GetModel' is defined in ModelCreator class.
            */
            var model = (IExModel)GetModel(descriptor.ControllerTypeInfo, out _);

            var Query = bindingContext.ActionContext.HttpContext.Request.Query;
            bool multiple = Query.ContainsKey(bindingContext.OriginalModelName);

            List<ISearchParameter> list = new List<ISearchParameter>();
            var PropertyNames = model.GetPropertyNames();
            
            if (multiple)
            {
                var items = Query[bindingContext.OriginalModelName];
                foreach (var item in items)
                {
                    if (item == null)
                        continue;
                    JsonObject? result = JsonNode.Parse(item)?.AsObject();
                    if(result == null)
                        continue;
                    string? Name = result["name"]?.GetValue<string>()?.ToLower();
                    if (!string.IsNullOrEmpty(Name))
                    {
                        foreach (var name in PropertyNames)
                        {
                            if (Name == name.ToLower())
                            {
                                var pvalue = result["value"]?.GetValue<object>();
                                var parameter = new ObjParameter(pvalue, name);
                                Enum.TryParse(result["criteria"]?.GetValue<string>(), true, out Criteria criteria);

                                var message = model.Parse(parameter, out _, out object? value, false, criteria);
                                if (message.Status == ResultStatus.Sucess)
                                    list.Add(new SearchParameter(name, value, criteria));
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                var Name = Query["name"][0]?.ToLower();

                foreach (var name in PropertyNames)
                {
                    if (Name == name.ToLower())
                    {
                        var parameter = new ModelParameter(Query["value"], name);
                        Enum.TryParse(Query["criteria"], true, out Criteria criteria);
                        var message = model.Parse(parameter, out _, out object? value, false, criteria);

                        if (message.Status == ResultStatus.Sucess)
                            list.Add(new SearchParameter(name, value, criteria));
                        break;
                    }
                }
            }
            if(list.Count ==0)
            {
                bindingContext.Result = ModelBindingResult.Success(SearchParameter.Empty);

                return Task.CompletedTask;
            }
            if (multiple)
            {
                bindingContext.Result = ModelBindingResult.Success(list);
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(list[0]);
            return Task.CompletedTask;
        }
    }

## UPDATE5
Added Exception Middleware. Middleware type: IExceptionFiter type
First, out own exception class and exception type enum are needed:

      /// <summary>
    /// Represents an exception object customized for model operations.
    /// </summary>
    public class ModelException: Exception, IModelException
    {
        #region CONSTRUCTORS
        public ModelException(string message, ExceptionType type) :
            base(message)
        {
            Type = type;
        }
        public ModelException(ExceptionType type, Exception exception) :
            base(exception.Message, exception)
        {
            Type = type;
        }
        public ModelException(string message, ExceptionType type, Exception exception) :
           base(message, exception)
        {
            Type = type;
        }
        #endregion

        #region PROPERTIES
        public ExceptionType Type { get; }

        public virtual int Status
        {
            get
            {
                switch (Type)
                {
                    case ExceptionType.Unknown:
                    case ExceptionType.NoModelFound:
                    case ExceptionType.NoModelFoundForID:
                    case ExceptionType.NoModelsFound:
                        return 404;
                    case Models.ExceptionType.NoModelSupplied:
                    case ExceptionType.NegativeFetchCount:
                    case ExceptionType.ModelCopyOperationFailed:
                    case ExceptionType.NoParameterSupplied:
                    case ExceptionType.NoParametersSupplied:
                    case ExceptionType.AddOperationFailed:
                    case ExceptionType.UpdateOperationFailed:
                    case ExceptionType.DeleteOperationFailed:
                        return 400;
                    case ExceptionType.InternalServerError:
                        return 500;
                    default:
                        return 400;
                }
            }
        }
        #endregion

        #region GET CONSOLIDATED MESSAGE
        public void GetConsolidatedMessage(out string Title, out string Type, out string? Details, out string[]? stackTrace, bool isProductionEnvironment = false)
        {
            Title = Message;
            Type = this.Type.ToString();
            Details =  "None" ;
            stackTrace = new string[] { };

            if (InnerException != null)
            {
                Details = InnerException.Message;

                if (InnerException.StackTrace != null && !isProductionEnvironment)
                {
                    stackTrace = InnerException.StackTrace.Split(System.Environment.NewLine);
                }
            }
        }
        #endregion

        #region CREATE
        /// <summary>
        /// Creates an instance of ModelException.
        /// </summary>
        /// <param name="message">Custom message provided by user.</param>
        /// <param name="type">Type of the model exception.</param>
        /// <param name="exception">Original exception raised by some operation performed on model.</param>
        /// <returns></returns>
        public static ModelException Create(string message, ExceptionType type, Exception? exception = null)
        {
            if(exception == null) 
                return new ModelException(message, type);
            return new ModelException(message, type, exception);
        }
        #endregion
    }
    
    public enum ExceptionType : ushort
    {
        Unknown = 0,

        /// <summary>
        /// Represents an exception to indicate that no model is found in the collection for a given search or the collection is empty.
        /// </summary>
        NoModelFound,

        /// <summary>
        /// Represents an exception to indicate that no model is found in the collection while searching it with specific ID.
        /// </summary>
        NoModelFoundForID,

        /// <summary>
        /// Represents an exception to indicate that the query to search multiple models returned no models.
        /// </summary>
        NoModelsFound,

        /// <summary>
        /// Represents an exception to indicate that no model is supplied where it required for example Add or Update functions.
        /// </summary>
        NoModelSupplied,

        /// <summary>
        /// Represents an exception to indicate that a negative number is supplied as a count of models to be returned.
        /// </summary>
        NegativeFetchCount,

        /// <summary>
        /// Represents an exception to indicate that a copy operation from either DTO or another model is failed.
        /// </summary>
        ModelCopyOperationFailed,

        /// <summary>
        /// Represents an exception to indicate that no valid paramter is supplied in a model search intended to find multiple or single models with single search criteria.
        /// </summary>
        NoParameterSupplied,

        /// <summary>
        /// Represents an exception to indicate that no valid paramters are supplied in a model search intended to find multiple or single models with multiple search criteria.
        /// </summary>
        NoParametersSupplied,

        /// <summary>
        /// Represents an exception to indicate that an operation of adding a model in the collection failed.
        /// </summary>
        AddOperationFailed,

        /// <summary>
        /// Represents an exception to indicate that an operation of updating a model in the collection failed.
        /// </summary>
        UpdateOperationFailed,

        /// <summary>
        /// Represents an exception to indicate that an operation of deleting a model in the collection failed.
        /// </summary>
        DeleteOperationFailed,

        /// <summary>
        /// Represents an exception to indicate that server is failed due to an internal error.
        /// </summary>
        InternalServerError,

        /// <summary>
        /// Represents an exception to indicate that the current operation failed an expectation of the requirement for conducting the operation.
        /// </summary>
        ExpectationFailed,

        /// <summary>
        /// Represents an exception to indicate that the supplied model context is not valid or compitible with the object intended to use it.
        /// </summary>
        InvalidContext,
    }

You can define more excetions types based on your needs.
Finally,

    public class HttpExceptionFilter : IExceptionFilter
    {
        void IExceptionFilter.OnException(ExceptionContext context)
        {
            ProblemDetails problem;

            if (context.Exception is IModelException)
            {
                var modelException = ((IModelException)context.Exception);                 

                modelException.GetConsolidatedMessage(out string title, out string type, out string? details, out string[]? stackTrace, Configuration.IsProductionEnvironment);
                problem = new ProblemDetails()
                {
                    Type = ((HttpStatusCode)modelException.Status).ToString() + ": " + type, // customize
                    Title = title, //customize
                    Status = modelException.Status, //customize
                    Detail = details,
                };
                var response = context.HttpContext.Response;
                response.ContentType = Contents.JSON;
                response.StatusCode = modelException.Status;
                if(stackTrace != null)
                {
                    response.WriteAsJsonAsync(new object[] { problem, stackTrace }).Wait();
                }
                else
                {
                    response.WriteAsJsonAsync(problem).Wait();
                }
                response.CompleteAsync().Wait();
                return;
            }
            
            problem = new ProblemDetails()
            {
                Type = "Error", // customize
                Title = "Error", //customize
                Status = (int)HttpStatusCode.ExpectationFailed, //customize
                Detail = context.Exception.Message,
            };
            context.Result = new JsonResult(problem);
        }
    }

## UPDATE6
Added Support for IActionResult for controller. 

    #if MODEL_DELETABLE
    /// <summary>
    /// This interface represents an object that allows deleting a single model with a specified ID.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IDeleteable<TModel, TID>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Deletes the model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to delete.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Delete(TID id);
    }
    #endif

    #region IAppendable<TModel, TID>
    #if MODEL_APPENDABLE
    /// <summary>
    /// This interface represents an object that has a list of models to which a new model can be appended.
    /// Any object that implements the IModel interface can be provided. This allows DTOs to be used instead of an actual model object.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IAppendable<TModel, TID>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Adds a new model based on the given model.
        /// If the given model is not TModel, then a new appropriate model will be created by copying data from the given model.
        /// </summary>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Add(IModel? model);
    }
    #endif

    #region IUpdatable<TModel, TID>
    #if MODEL_UPDATABLE
    /// <summary>
    /// This interface represents an object that has a list of models and allows a model with a specified ID to be updated with data from the given model parameter.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IUpdatable<TModel, TID>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Updates a model specified by the given ID with the data of the given model.
        /// </summary>
        /// <param name="id">ID of the model to be updated.</param>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Update(TID id, IModel? model);
    }
    #endif

    #region IFind<TModel>
    #if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
    public interface IFind<TModel>
        where TModel : ISelfModel<TModel>
    {
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        Task<IActionResult> GetAll(int limitOfResult = 0);

        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        Task<IActionResult> GetAll(int startIndex, int limitOfResult);

        /// <summary>
        /// Finds a model based on given paramters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        Task<IActionResult> Find(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin = 0);

        /// <summary>
        /// Finds all models matched based on given paramter.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        Task<IActionResult> FindAll(ISearchParameter parameter);

        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        Task<IActionResult> FindAll(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin = 0);

        /// <summary>
        /// Finds a model based on given paramter.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        Task<IActionResult> Find(ISearchParameter? parameter);
    }
    #endif

    #if !MODEL_NONQUERYABLE || !MODEL_NONREADABLE
    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IFindByID<TModel, TID>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Get(TID? id);
    }
    #endif

    public interface IActionContract<TModel, TID>: IContract
    #if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        , IActionQuery<TModel>
        , IFindByID<TModel, TID>
    #endif
    #if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
    , IActionCommand<TModel, TID>
    #endif
    where TModel : ISelfModel<TID, TModel>, new()
    where TID : struct
    {
    }
These interfaces are identical to the regular ones except they all returns Task\<IActionResult\>
So, Now we have support for IActionResult and actual object return types.
Use conditional compiler constant: MODEL_USEACTION

## UPDATE7
Feature: Choose database at model level.
    
    public enum ConnectionKey
    {
        InMemory,
    #if MODEL_CONNECTSQLSERVER        
        SQLServer = 1,
    #elif MODEL_CONNECTPOSTGRESQL
        PostgreSQL = 1,
    #elif MODEL_CONNECTMYSQL
        MySQL = 1,
    #endif
    }
Feel free to add more dabase options to the enum above if you desire so.
Create appropriate CCC for any option that you add. Idealy, there should
be only 2 options available: one is 'InMemory' and the other is for actual database.

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

## UPDATE8
Controller class: 4th Type TInDTO included.

So now it is Controller<TOutDTO, TModel, TID, TInDTO>

    [ApiController]
    [Route("[controller]")]
    public class Controller<TOutDTO, TModel, TID, TInDTO> : ControllerBase, IExController
    #if !MODEL_USEACTION
        , IContract<TOutDTO, TModel, TID>
    #else
        , IActionContract<TModel, TID>
    #endif
    where TOutDTO : IModel
    where TModel : class, ISelfModel<TID, TModel>,
    #if (!MODEL_USEDTO)
        TOutDTO,
    #endif
    new()
    where TID : struct
    where TInDTO : IModel
    {
        // controller code
    }
We can define different DTOs for Out (GET calls) and IN (POST, PUT calls).
We can still use any DTO for the both IN and OUT though.

## UPDATE9 
Converted DBContext from generic to non-generic class.
This is to allow single DBContext to hold multiple model sets..

    public partial class DBContext : DbContext, IModelContext
    {
        #region CONSTRUCTOR
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        { }
        #endregion

        #region ON MODEL CREATION
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBContext).Assembly);
        }
        #endregion

        #if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
        /// <summary>
        /// Creates new instance of ModelSet<TModel, TID>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        /// <returns>An instance of ModelSet<TModel, TID></returns>
        ICommand<TID, TModel> IModelContext.CreateCommand<TID, TModel>()
        {
            return new ModelList<TID, TModel>(this, Set<TModel>());
        }
        #endif
        
        #if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Creates new instance of QuerySet<TModel>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model></typeparam>
        /// <returns>An instance of QuerySet<TModel, TID></returns>
        IQuery<TModel> IModelContext.CreateQuery<TModel>()
        {
            var list = Set<TModel>();
            var set = new QuerySet<TModel, DbSet<TModel>>(list, list.AddRange);
            SaveChanges();
            return set;
        }
        #endif
    }

OnModelCreating(ModelBuilder modelBuilder) method is the most important method, 
beacause we are adding DbSets dynamically, we must make sure that our DBContext recognises them
and does not throw an error. To achieve that, we already created partial Model\<TModel\> class and implemented
ISelfModel\<TModel\> interface so, another partial declaration in Web-API/Models folder:
Learned it from here: https://learn.microsoft.com/en-us/ef/core/modeling/ and then I had to make breaking changes
all the way upto the model class and interfaces to define Model\<TModel\> and ISelfModel\<TModel\> 

    partial class Model<TModel> : IEntityTypeConfiguration<TModel>, IExModel
    {
        protected virtual void Configure(EntityTypeBuilder<TModel> builder) { }
        void IEntityTypeConfiguration<TModel>.Configure(EntityTypeBuilder<TModel> builder)
        {
            Configure(builder);            
        }
    }
IEntityTypeConfiguration\<TModel\> is the key. Now every model that inherits from Model\<TModel\>
will not need to worry about getting associated with DBContext.

## UPDATE10 
Support for Query-Only-Controllers and Keyless models is added.

    [Keyless]
    public abstract class KeylessModel<TModel>: Model<TModel>, IEntityTypeConfiguration<TModel>
        where TModel : Model<TModel>
    {
        int PsuedoID;

        void IEntityTypeConfiguration<TModel>.Configure(EntityTypeBuilder<TModel> builder)
        {
            base.Configure(builder);
            builder.HasKey("PsuedoID");
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class QueryController<TOutDTO, TModel> : ControllerBase, IExController
    #if !MODEL_USEACTION
        , IQueryContract<TOutDTO, TModel>
    #else
        , IQueryContract<TModel>
    #endif
    where TOutDTO : IModel
    where TModel : Model<TModel>,
    #if (!MODEL_USEDTO)
        TOutDTO,
    #endif
    new()
    {
        // controller code
    }

It is now possible to create separate controller for command and query purposes.

Use constant MODEL_NONREADABLE: this will create Command-only controller.
Then for the same model, call AddQueryModel() method, which is located in Configuration class, will create Query-only controller.

## UPDATE11 
Abstract Models for common primary key type: int, long, Guid, enum are added.

    public abstract class ModelEnum<TEnum, TModel> : Model<TEnum, TModel>
        where TModel : ModelEnum<TEnum, TModel>
        where TEnum : struct, Enum
    {
        #region VARIABLES
        static HashSet<TEnum> Used  = new HashSet<TEnum>();
        static List<TEnum> Available = new List<TEnum>();
        static volatile int index;
        #endregion

        #region CONSTRUCTORS
        static ModelEnum()
        {
            Available = new List<TEnum>(Enum.GetValues<TEnum>());
        }
        protected ModelEnum() :
            base(false)
        { }
        protected ModelEnum(bool generateNewID) :
            base(generateNewID)
        { }
        #endregion

        #region GET NEW ID
        protected override TEnum GetNewID()
        {
           if(Available.Count == 0)
                return default(TEnum);
            var newID = Available[index];
            if (!Used.Contains(newID))
            {
                Used.Add(newID);
                return newID;
            }
            while (index < Available.Count)
            {
                newID = Available[++index];

                if (!Used.Contains(newID))
                {
                    Used.Add(newID);
                    return newID;
                }
            }
            return default(TEnum);
        }
        #endregion

        #region TRY PARSE ID
        protected override bool TryParseID(object value, out TEnum newID)
        {
            return Enum.TryParse(value.ToString(), true, out newID);
        }
        #endregion
    }
    
    public abstract class ModelInt32<TModel> : Model<int, TModel>
        where TModel : ModelInt32<TModel>
    {
        #region VARIABLES
        static volatile int IDCounter;
        #endregion

        #region CONSTRUCTORS
        protected ModelInt32() :
            base(false)
        { }
        protected ModelInt32(bool generateNewID) :
            base(generateNewID)
        { }
        #endregion

        #region GET NEW ID
        protected override int GetNewID()
        {
            return ++IDCounter;
        }
        #endregion

        #region TRY PARSE ID
        protected override bool TryParseID(object value, out int newID)
        {
            return int.TryParse(value.ToString(), out newID);
        }
        #endregion
    }
In similiar fashion ModelInt64\<TModel\> and ModelGuid\<TModel\> classes are created.
If you want string keyed model, you will need to create a struct which contains string value
and use as 'TID' because TID can only be struct. 
Also note that when you are using an actual database GetNewID() method implementation might change;
You may want to get unique ID from the database itself. 
    
## UPDATE12 
Adapted: CQRS pattern.
Adapted Command and Query Segregation pattern.
The follwing interfaces were created and the solution was re-designed
around them.
For Query Part:
IQuery\<TModel\>
IQuery\<TOutDTO, TModel, TID\>

    /// <summary>
    /// Represents an object which holds a enumerables of keyless models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of keyless Model></typeparam>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    public partial interface IQuery<TOutDTO, TModel> : IModelCount, IFind<TOutDTO, TModel>, IFirstModel<TModel>
        where TModel : ISelfModel<TModel>
        where TOutDTO : IModel
    { }

    #region IExQuery<TOutDTO, TModel>
    internal partial interface IExQuery<TOutDTO, TModel> : IQuery<TOutDTO, TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>
        where TOutDTO : IModel
        #endregion
    { }
    #endregion

    /// <summary>
    /// Represents an object which holds a enumerables of keyless models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of keyless Model></typeparam>
    public partial interface IQuery<TModel> : IQuery<TModel, TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>
        #endregion
    { }

    internal partial interface IExQuery<TModel> : IQuery<TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>
        #endregion
    { }

    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IQuery<TOutDTO, TModel, TID> : IQuery<TOutDTO, TModel>,
        IFindByID<TOutDTO, TModel, TID>
    where TOutDTO : IModel
    where TModel : class, ISelfModel<TID, TModel>, new()
    #if (!MODEL_USEDTO)
        , TOutDTO
    #endif
    where TID : struct
    {
    }
    #endregion

For Command Part:
ICommand\<TID, TModel\>
ICommand\<TOutDTO, TModel, TID\>

    /// <summary>
    /// Represents an object which holds a enumerables of keyless models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of keyless Model></typeparam>
    public partial interface ICommand<TID, TModel> : IModelCount
    #if MODEL_DELETABLE
        , IDelete<TModel>
    #endif
    #if MODEL_APPENDABLE
        , IAdd<TModel>
    #endif
    #if MODEL_UPDATABLE
        , IUpdate<TModel>
    #endif
    #if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        , IModifiable
    #endif
        //+:cnd:noEmit
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TID, TModel>, new()
        where TID : struct
        #endregion
    { }
    #endregion

    #region IExCommand<TID, TModel>
    internal partial interface IExCommand<TID, TModel> : ICommand<TID, TModel>,
        IFindByID<TModel, TModel, TID>, IFirstModel<TModel>
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TID, TModel>, new()
        where TID : struct
        #endregion
    {
        #if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
            IExQuery<TModel> GetQueryObject();
        #endif
    }
    #endregion

    #if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface ICommand<TOutDTO, TModel, TID> : IModelCount
    #if MODEL_APPENDABLE
        , IAppendable<TOutDTO, TModel, TID>
    #endif
    #if MODEL_UPDATABLE
        , IUpdatable<TOutDTO, TModel, TID>
    #endif
    #if MODEL_DELETABLE
        , IDeleteable<TOutDTO, TModel, TID>
    #endif
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : class, ISelfModel<TID, TModel>, new()
        //-:cnd:noEmit
    #if (!MODEL_USEDTO)
        , TOutDTO
    #endif
        where TID : struct
        #endregion
    {
    }
    #endif
