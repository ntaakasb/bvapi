using System.Net;

namespace WebCore.Infrastructure.BusinessObjects
{
    public sealed class ResponseInfo
    {
        /// <summary>
        /// Original returning HTTP Status Code within the API execution.
        /// </summary>
        public HttpStatusCode Code { get; set; }

        /// <summary>
        /// Business Logic Status.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response Message (e.g. Success message or Error message).
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Returning data always be stored in collection (list) format.
        /// It means, "IsSingleResource = false" by default.
        /// If IsSingleResource = true, the expected data will be found at the first item of the returning list.
        /// </summary>
        public bool IsSingleResource { get; set; }

        /// <summary>
        /// This property is used for paging purpose, presenting for all matching data items belong to a particular business logic.
        /// </summary>
        public int TotalRecord { get; set; }

        /// <summary>
        /// This property is used for paging purpose, presenting for number of records to return.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// This property is used for paging purpose, presenting for number of records to skip before taking the actual result list.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Supports to indicate newly created resource id.
        /// </summary>
        public string ReturnId { get; set; }

        /// <summary>
        /// Original exception object within API execution.
        /// </summary>
        public object InnerEx { get; set; }
    }
}