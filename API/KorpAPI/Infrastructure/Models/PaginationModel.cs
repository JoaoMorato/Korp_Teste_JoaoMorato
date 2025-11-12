namespace Infrastructure.Models;

public class PaginationModel
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; } = 0;

    public int GetSkipCount()
    {
        return (CurrentPage - 1) * PageSize;
    }
}
