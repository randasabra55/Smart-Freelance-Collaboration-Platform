using System.ComponentModel;

namespace Smart_Freelance_Infrastructure.Common.Pagination.BuilderPattern;
public interface IBasePaginationModel
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
    bool PaginationOff { get; set; }
    bool IsDescending { get; set; }
    public string? SearchTerm { get; set; }

}

public abstract class BasePaginationModel : IBasePaginationModel
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool PaginationOff { get; set; } = true;
    [DefaultValue(true)]
    public bool IsDescending { get; set; } = true;
    public string? SearchTerm { get; set; }
}
