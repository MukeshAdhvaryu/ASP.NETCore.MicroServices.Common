﻿/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

namespace MicroService.Common.Models
{
    /// <summary>
    /// Provides status of an operation.
    /// </summary>
    public enum ResultStatus : byte
    {
        /// <summary>
        /// Indicates that operation is failed.
        /// </summary>
        Failure,

        /// <summary>
        /// Indicates that operation is successful.
        /// </summary>
        Sucess,

        /// <summary>
        /// Indicates that operation is ignored for a specified call.
        /// </summary>
        Ignored,

        /// <summary>
        /// Indicates that operation  for a specified call could not be performed because of missing value.
        /// </summary>
        MissingValue,

        /// <summary>
        /// Indicates that operation for a specified call could not be performed because of missing value.
        /// And, the value is required to be supplied.
        /// </summary>
        MissingRequiredValue,
    }

    #region AND/OR
    /// <summary>
    /// Enum AndOr
    /// </summary>
    public enum AndOr : byte
    {
        /// <summary>
        /// The and
        /// </summary>
        AND,

        /// <summary>
        /// The or
        /// </summary>
        OR,
    }
    #endregion

    #region CRITERIA
    /// <summary>
    /// Enum Criteria
    /// </summary>
    public enum Criteria : sbyte
    {
        /// <summary>
        /// The equal
        /// </summary>
        Equal = 0,
        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan = 1,
        /// <summary>
        /// The less than
        /// </summary>
        LessThan = 2,
        /// <summary>
        /// The occurs
        /// </summary>
        Occurs = 3,
        /// <summary>
        /// The begins with
        /// </summary>
        BeginsWith = 4,
        /// <summary>
        /// The ends with
        /// </summary>
        EndsWith = 5,
        /// <summary>
        /// The occurs no case
        /// </summary>
        OccursNoCase = 6,
        /// <summary>
        /// The begins with no case
        /// </summary>
        BeginsWithNoCase = 7,
        /// <summary>
        /// The ends with no case
        /// </summary>
        EndsWithNoCase = 8,
        /// <summary>
        /// The string equal
        /// </summary>
        StringEqual = 9,
        /// <summary>
        /// The string equal no case
        /// </summary>
        StringEqualNoCase = 10,
        /// <summary>
        /// The string number greater than
        /// </summary>
        StringNumGreaterThan = 11,
        /// <summary>
        /// The string number less than
        /// </summary>
        StringNumLessThan = 12,
        /// <summary>
        /// The not equal
        /// </summary>
        NotEqual = -1,
        /// <summary>
        /// The not greater than
        /// </summary>
        NotGreaterThan = -2,
        /// <summary>
        /// The not less than
        /// </summary>
        NotLessThan = -3,
        /// <summary>
        /// The not occurs
        /// </summary>
        NotOccurs = -4,
        /// <summary>
        /// The not begins with
        /// </summary>
        NotBeginsWith = -5,
        /// <summary>
        /// The not ends with
        /// </summary>
        NotEndsWith = -6,
        /// <summary>
        /// The not occurs no case
        /// </summary>
        NotOccursNoCase = -7,
        /// <summary>
        /// The not begins with no case
        /// </summary>
        NotBeginsWithNoCase = -8,
        /// <summary>
        /// The not ends with no case
        /// </summary>
        NotEndsWithNoCase = -9,
        /// <summary>
        /// The not string equal
        /// </summary>
        NotStrEqual = -10,
        /// <summary>
        /// The not string equal no case
        /// </summary>
        NotStrEqualNoCase = -11,
        /// <summary>
        /// The not string greater than
        /// </summary>
        NotStringGreaterThan = -12,
        /// <summary>
        /// The not string less than
        /// </summary>
        NotStringLessThan = -13
    }
    #endregion

    #region MULTI CRITERIA
    /// <summary>
    /// Enum MultCriteria
    /// </summary>
    public enum MultCriteria : sbyte
    {
        /// <summary>
        /// The between
        /// </summary>
        Between = 0,
        /// <summary>
        /// The not between
        /// </summary>
        NotBetween = -1,
    }
    #endregion

    #region EXCEPTION TYPES
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
    #endregion

    #region CONNECTION KEYS
    public enum ConnectionKey
    {
        InMemory,
        //-:cnd:noEmit
#if MODEL_CONNECTSQLSERVER        
        SQLServer = 1,
#elif MODEL_CONNECTPOSTGRESQL
        PostgreSQL = 1,
#elif MODEL_CONNECTMYSQL
        MySQL = 1,
#endif
        //+:cnd:noEmit
    }
    #endregion
}
namespace MicroService.Common.Services
{
    /// <summary>
    /// Provides Scope options for injecting service repository.
    /// </summary>
    public enum ServiceScope : byte
    {
        /// <summary>
        /// Injects scoped service.
        /// </summary>
        Scoped,

        /// <summary>
        /// Injects transient service.
        /// </summary>
        Transient,

        /// <summary>
        /// Injects singleton service.
        /// </summary>
        Singleton,
    }
}
