namespace NapAdminCore.Criteria.Trace;

using System;
using System.ComponentModel.DataAnnotations;
using Fanex.Data.Repository;
using Web;

public class GetAllLogFunctionCriteria : CriteriaBase
{
    public string Name { get; set; }

    [DisplayFormat(DataFormatString = "{0:MMM dd,yyyy}", ApplyFormatInEditMode = true)]
    public DateTime From { get; set; }

    [DisplayFormat(DataFormatString = "{0:MMM dd,yyyy}", ApplyFormatInEditMode = true)]
    public DateTime To { get; set; }

    [Display(Name = "Site", ResourceType = typeof(ViewResources))]
    public int SiteId { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalRows { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_LogFunction_Search";
    }

    public override bool IsValid()
    {
        return true;
    }
}
