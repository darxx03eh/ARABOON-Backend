namespace Araboon.Data.Wrappers
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; }
        public Int32 CurrentPage { get; set; }
        public Int32 TotalPages { get; set; }
        public Int32 TotalCount { get; set; }
        public Int32 PageSize { get; set; }
        public Boolean HasPreviousPage => CurrentPage > 1;
        public Boolean HasNextPage => CurrentPage < TotalPages;
        public PaginatedResult(List<T> data)
            => Data = data;
        internal PaginatedResult(List<T> data = default, List<String> messages = null,
                                 Int32 count = 0, Int32 page = 1, Int32 pageSize = 20)
            => (Data, CurrentPage, PageSize, TotalPages, TotalCount) 
            = (data, page, pageSize, (Int32)Math.Ceiling(count / (Double)pageSize), count);
        public static PaginatedResult<T> Success(List<T> data, Int32 count, Int32 page, Int32 pageSize)
            => new(data, null, count, page, pageSize);
    }
}
