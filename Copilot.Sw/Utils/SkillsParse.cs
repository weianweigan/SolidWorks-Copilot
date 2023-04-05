using System.Xml;

namespace Copilot.Sw.Utils;

public static class SkillsParse
{
    public static (int,string) Parse(string output)
    {
        output = output.Trim();
        if (int.TryParse(output,out var value))
        {
            return (value, string.Empty);
        }
        else
        {
            if (output.StartsWith("<"))
            {
                XmlDocument doc = new();
                doc.LoadXml(output);
                return (-1, doc.InnerText);
            }
            else
            {
                return (-1, output);
            }
        }
    }
}
