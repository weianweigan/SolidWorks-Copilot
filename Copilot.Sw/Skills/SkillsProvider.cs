using System.IO;

namespace Copilot.Sw.Skills;

public class SkillsProvider : ISkillsProvider
{
    ///<inheritdoc/>
    public string SkillsLocation { get; }

    public SkillsProvider()
    {
        SkillsLocation = Path.Combine(
            Path.GetDirectoryName(typeof(SkillsProvider).Assembly.Location), 
            "Skills");
    }
}