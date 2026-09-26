
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class PlanDocument(XDocument xml)
{
    public static readonly XNamespace Ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";
    public XDocument Xml { get; } = xml;

    public IEnumerable<XElement> Descendants(string localName) => Xml.Descendants(Ns + localName);

    public bool HasRuntimeStats => Descendants(PlanXmlNames.Elements.RunTimeInformation).Any();
}
