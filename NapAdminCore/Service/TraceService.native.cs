
namespace NapAdminCore.Service;

using System.Collections;
using System.Linq;
using System.Xml.Linq;
using Criteria.Trace;
using Fanex.Data.Repository;
using Models.ValueObject;

public interface ITraceService 
{
    LogExecSp GetById(int logId);

    IList<LogExecSp> SearchExecSp(GetAllLogCriteria criteria);

    IList<LogUserLogin> SearchUserLogin(GetAllLogUserLoginCriteria criteria);
}
public class TraceService(IDynamicRepository dynamicRepository) : ITraceService
{
    public LogExecSp GetById(int logId)
    {
        var criteria = new GetByIdLogCriteria
        {
            LogId = logId
        };

        if (criteria.IsValid())
        {
            var logExec = dynamicRepository.Get<LogExecSp>(criteria);

            if (logExec is not null)
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

    public IList<LogExecSp> SearchExecSp(GetAllLogCriteria criteria)
    {
        return [.. dynamicRepository.Fetch<LogExecSp>(criteria)];
    }

    public IList<LogUserLogin> SearchUserLogin(GetAllLogUserLoginCriteria criteria)
    {
        return [.. dynamicRepository.Fetch<LogUserLogin>(criteria)];
    }
    
    private static string EncodeXMLValue(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = value.Replace("&", "&amp;");
            value = value.Replace("<", "&lt;");
            value = value.Replace(">", "&gt;");
            value = value.Replace("\"", "&quot;");
            value = value.Replace("''", "&apos;");
        }

        return value;
    }
}
