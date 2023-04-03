using System.Xml;

namespace Copilot.Sw.Utils;

public static class SkillsParse
{
    public static (int,string) Parse(string output)
    {
        if (int.TryParse(output,out var value))
        {
            return (value, string.Empty);
        }
        else
        {
            XmlDocument doc = new();
            doc.LoadXml(output);
            return (value, doc.InnerText);
        }
    }
}
