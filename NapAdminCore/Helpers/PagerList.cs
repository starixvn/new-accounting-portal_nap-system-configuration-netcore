namespace NapAdminCore.Helpers;

using System.Collections;
using System.Collections.Generic;

public interface IPagedList : IEnumerable
{
    int CurrentPageIndex { get; set; }

    int PageSize { get; set; }
        
    int TotalItemCount { get; set; }
}


public class PagerList<T> : List<T>, IPagedList
{
    public PagerList(int pageIndex, int pageSize, int totalItemCount, IEnumerable<T> items)
    {
        CurrentPageIndex = pageIndex;
        PageSize = pageSize;
        TotalItemCount = totalItemCount;
        AddRange(items);
    }

    public int CurrentPageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalItemCount { get; set; }
}
