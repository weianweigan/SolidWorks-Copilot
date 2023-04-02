using Copilot.Sw.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public IEnumerable<SkillModel> GetSkills()
    {
        if (!Directory.Exists(SkillsLocation))
        {
            throw new DirectoryNotFoundException($"找不到Skill：{SkillsLocation}");
        }

        var skillDirs = Directory.GetDirectories(SkillsLocation)
            .Where(p => p.EndsWith("Skill"));

        foreach (var dir in skillDirs)
        {
            yield return new SkillModel(dir);
        }
    }
}