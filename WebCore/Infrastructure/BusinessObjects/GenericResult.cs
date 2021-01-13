using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace WebCore.Infrastructure.BusinessObjects
{
    public sealed class GenericResult<T>
    {
        #region Single Result

        /// <summary>
        /// Gets successful result with default http status code = 200.
        /// </summary>
        public static GenericResult<T> Succeeded => Succeed(null, HttpStatusCode.OK);

        /// <summary>
        /// Helper to return successful result with an item with default http status code = 200.
        /// </summary>
        /// <param name="item">The item.</param>
        public static GenericResult<T> Succeed(T item)
        {
            return Succeed(item, HttpStatusCode.OK);
        }

        /// <summary>
        /// Helper to return successful result with an item with explicit http status code.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public static GenericResult<T> Succeed(T item, HttpStatusCode httpStatusCode)
        {
            return Succeed(new List<T> { item }, httpStatusCode, true);
        }

        #endregion

        #region List Result

        /// <summary>
        /// Helper to return successful result with list of items.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        /// <param name="isSingleResource">if set to <c>true</c> [is single resource].</param>
        public static GenericResult<T> Succeed(IEnumerable<T> itemList, bool isSingleResource = false)
        {
            return Succeed(itemList, HttpStatusCode.OK, isSingleResource);
        }

        /// <summary>
        /// Helper to return successful result with list of items and explicit single item result and total records.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="isSingleResource">if set to <c>true</c> [is single resource].</param>
        /// <param name="totalRecord">The total record of the item list.</param>
        public static GenericResult<T> Succeed(IEnumerable<T> itemList, HttpStatusCode httpStatusCode, bool isSingleResource = false, int totalRecord = 0)
        {
            return new GenericResult<T>
            {
                Metadata = new ResponseInfo
                {
                    Code = httpStatusCode,
                    IsSingleResource = isSingleResource,
                    Success = true,
                    TotalRecord = totalRecord
                },
                Results = itemList
            };
        }

        #endregion

        #region Failed Results

        /// <summary>
        /// Gets failed result with default http status code = 400.
        /// </summary>
        public static GenericResult<T> BadRequest => Fail(string.Empty, HttpStatusCode.BadRequest);

        /// <summary>
        /// Gets failed result with default http status code = 404.
        /// </summary>
        public static GenericResult<T> NotFound => Fail(string.Empty, HttpStatusCode.NotFound);

        /// <summary>
        /// Gets failed result with default http status code = 204.
        /// </summary>
        public static GenericResult<T> Failed => Fail(string.Empty, HttpStatusCode.NoContent);

        /// <summary>
        /// Gets result with a message and default http status code = 400.
        /// </summary>
        /// <param name="message">The message.</param>
        public static GenericResult<T> Fail(string message)
        {
            return Fail(message, HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Gets failed result with a message from exception and default http status code = 400.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="methodBase">The method base.</param>
        public static GenericResult<T> Fail(Exception ex, MethodBase methodBase)
        {
            // Todo: Log exception
            return Fail(ex.Message, HttpStatusCode.BadRequest, ex);
        }

        /// <summary>
        /// Gets failed result with a http status code.
        /// </summary>
        /// <param name="code">The code.</param>
        public static GenericResult<T> Fail(HttpStatusCode code)
        {
            return Fail(string.Empty, code);
        }

        /// <summary>
        /// Helper to return failed result with explicit a message and http status code.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="httpStatusCode">The http status code.</param>
        /// <param name="ex">The exception.</param>
        public static GenericResult<T> Fail(string message, HttpStatusCode httpStatusCode, Exception ex = null)
        {
            return new GenericResult<T>
            {
                Metadata = new ResponseInfo
                {
                    InnerEx = ex,
                    Success = false,
                    Message = message,
                    Code = httpStatusCode
                }
            };
        }

        /// <summary>
        /// Helper to return successful result with an item with explicit http status code and meessage.
        /// </summary>
        /// <param name="message">The return message.</param>
        public static GenericResult<T> SucceedWithMessage(string message)
        {
            return new GenericResult<T>
            {
                Metadata = new ResponseInfo
                {
                    Code = HttpStatusCode.OK,
                    Success = true,
                    Message = message
                }
            };
        }

        #endregion

        /// <summary>
        /// Gets or sets header information from an API execute action.
        /// </summary>
        public ResponseInfo Metadata { get; set; }

        /// <summary>
        /// Gets or sets data in collection (list) format.
        /// </summary>
        public IEnumerable<T> Results { get; set; }
    }
}