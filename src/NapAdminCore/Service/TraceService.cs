
namespace NapAdminCore.Service;

using System.Collections;
using System.Linq;
using System.Xml.Linq;
using Criteria.Trace;
using Models.ValueObject;
using Repository;

public interface ITraceService : IBaseService
{
    LogExecSp GetById(int logId);

    IQueryable<LogExecSp> SearchExecSp(GetAllLogCriteria criteria);

    IQueryable<LogUserLogin> SearchUserLogin(GetAllLogUserLoginCriteria criteria);

    IQueryable<LogFunction> SearchLogFunction(GetAllLogFunctionCriteria criteria);
}
public class TraceService : BaseService, ITraceService
{
    private readonly ITraceRepository _traceRepository;

    public TraceService(ITraceRepository traceRepository)
    {
        _traceRepository = traceRepository;
    }

    public LogExecSp GetById(int logId)
    {
        var criteria = new GetByIdLogCriteria
        {
            LogId = logId
        };

        if (criteria.IsValid())
        {
            var logExec = _traceRepository.GetById(criteria);

            if (!ReferenceEquals(logExec, null))
            {
                var hashItems = new Hashtable();
                var hashSubItems = new Hashtable();

                if (!string.IsNullOrWhiteSpace(logExec.Items))
                {
                    var xmlItems = XDocument.Parse(logExec.Items);
                    var xmlItemRows = xmlItems.Descendants("row");

                    foreach (var item in xmlItemRows)
                    {
                        var alias = item.Attribute("alias").Value;
                        if (!hashItems.Contains(alias))
                        {
                            hashItems.Add(alias, item.Attribute("item").Value);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(logExec.SubItems))
                {
                    var xmlItems = XDocument.Parse(logExec.SubItems);
                    var xmlItemRows = xmlItems.Descendants("row");

                    foreach (var item in xmlItemRows)
                    {
                        var alias = item.Attribute("alias").Value;
                        if (!hashSubItems.Contains(alias))
                        {
                            hashSubItems.Add(alias, item.Attribute("item").Value);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(logExec.Data))
                {
                    var xmlData = XDocument.Parse(logExec.Data);
                    var xmlElementData = new XElement("ROOT");

                    if (hashItems.Count > 0)
                    {
                        foreach (DictionaryEntry item in hashItems)
                        {
                            var attribute = xmlData.Root.Attribute(item.Key.ToString());
                            var value = attribute != null ? attribute.Value : string.Empty;

                            xmlElementData.SetAttributeValue(item.Value.ToString(), value);
                        }
                    }

                    if (hashSubItems.Count > 0)
                    {
                        var xmlElementValue = new XElement("ROW");
                        var xmlElementSubData = xmlData.Descendants("z");

                        foreach (var item in xmlElementSubData)
                        {
                            var xmlElementChildTemp = new XElement("ITEM");

                            foreach (DictionaryEntry itemAtt in hashSubItems)
                            {
                                var attribute = item.Attribute(itemAtt.Key.ToString());
                                var value = attribute != null ? attribute.Value : string.Empty;

                                xmlElementChildTemp.SetAttributeValue(itemAtt.Value.ToString(), value);
                            }

                            xmlElementValue.Add(xmlElementChildTemp);
                        }

                        xmlElementData.SetAttributeValue("Value", EncodeXMLValue(xmlElementValue.ToString()));
                    }

                    logExec.Data = xmlElementData.ToString();
                }

                if (!string.IsNullOrWhiteSpace(logExec.OldData))
                {
                    var xmlData = XDocument.Parse(logExec.OldData);
                    var xmlElementData = new XElement("ROOT");

                    if (hashItems.Count > 0)
                    {
                        foreach (DictionaryEntry item in hashItems)
                        {
                            var attribute = xmlData.Root.Attribute(item.Key.ToString());
                            var value = attribute != null ? attribute.Value : string.Empty;

                            xmlElementData.SetAttributeValue(item.Value.ToString(), value);
                        }
                    }

                    if (hashSubItems.Count > 0)
                    {
                        var xmlElementValue = new XElement("ROW");
                        var xmlElementSubData = xmlData.Descendants("z");

                        foreach (var item in xmlElementSubData)
                        {
                            var xmlElementChildTemp = new XElement("ITEM");
                            foreach (DictionaryEntry itemAtt in hashSubItems)
                            {
                                var attribute = item.Attribute(itemAtt.Key.ToString());
                                var value = attribute != null ? attribute.Value : string.Empty;

                                xmlElementChildTemp.SetAttributeValue(itemAtt.Value.ToString(), value);
                            }

                            xmlElementValue.Add(xmlElementChildTemp);
                        }

                        xmlElementData.SetAttributeValue("Value", EncodeXMLValue(xmlElementValue.ToString()));
                    }

                    logExec.OldData = xmlElementData.ToString();
                }
            }

            return logExec;
        }

        return null;
    }

    public IQueryable<LogExecSp> SearchExecSp(GetAllLogCriteria criteria)
    {
        return _traceRepository.SearchLogExecSp(criteria);
    }

    public IQueryable<LogUserLogin> SearchUserLogin(GetAllLogUserLoginCriteria criteria)
    {
        return _traceRepository.SearchLogUserLogin(criteria);
    }

    public IQueryable<LogFunction> SearchLogFunction(GetAllLogFunctionCriteria criteria)
    {
        return _traceRepository.SearchLogFunction(criteria);
    }

    private string EncodeXMLValue(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = value.Replace("&", "&amp;");
            value = value.Replace("<", "&lt;");
            value = value.Replace(">", "&gt;");
            value = value.Replace("\"", "&quot;");
            value = value.Replace("''", "&apos;");

            return value;
        }

        return value;
    }
}
