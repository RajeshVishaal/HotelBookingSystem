namespace Common.Dto;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalRecords { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}