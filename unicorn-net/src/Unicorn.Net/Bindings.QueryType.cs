using Unicorn.Internal;

namespace Unicorn
{
    public partial class Bindings
    {
        /// <summary>
        /// Types of query.
        /// </summary>
        public enum QueryType
        {
            /// <summary>
            /// Queries the mode.
            /// </summary>
            Mode = uc_query_type.UC_QUERY_MODE,

            /// <summary>
            /// Queries the page size.
            /// </summary>
            PageSize = uc_query_type.UC_QUERY_PAGE_SIZE
        }
    }
}
