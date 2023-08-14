namespace NapAdminCore.Criteria.Trace;

using System;
using System.ComponentModel.DataAnnotations;
using Fanex.Data.Repository;
using Web;

public class GetAllLogCriteria : CriteriaBase
{
    [Display(Name = "Modified By")]
    public string Username { get; set; }

    public string Name { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:MMM dd,yyyy}", ApplyFormatInEditMode = true)]
    public DateTime From { get; set; }

    [DisplayFormat(DataFormatString = "{0:MMM dd,yyyy}", ApplyFormatInEditMode = true)]
    public DateTime To { get; set; }

    [Display(Name = "Filter", ResourceType = typeof(ViewResources))]
    public int FilterType { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalRows { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_LogExecSp_Search";
    }

    public override bool IsValid()
    {
        return true;
    }
}
