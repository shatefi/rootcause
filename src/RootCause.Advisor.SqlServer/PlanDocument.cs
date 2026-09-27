
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class PlanDocument(XDocument xml)
{
    public static readonly XNamespace Ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";
    public XDocument Xml { get; } = xml;

    public IEnumerable<XElement> Descendants(string localName) => Xml.Descendants(Ns + localName);

    public bool HasRuntimeStats => Descendants(PlanXmlNames.Elements.RunTimeInformation).Any();

    public TargetContext ReadTargetContext()
    {
        var build = (string?)Xml.Root?.Attribute(PlanXmlNames.Attributes.Build);
        if (!Version.TryParse(build, out var version))
        {
            throw new InvalidOperationException($"The plan's Build attribute '{build}' is not a version number.");
        }
        var ceModelVersion = (int?)Xml.Descendants()
        .Attributes(PlanXmlNames.Attributes.CardinalityEstimationModelVersion)
        .FirstOrDefault();

        return new TargetContext(version.Major, null, ceModelVersion);
    }
}
