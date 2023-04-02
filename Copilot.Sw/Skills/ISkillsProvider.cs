using Copilot.Sw.Models;
using System.Collections.Generic;

namespace Copilot.Sw.Skills;

public interface ISkillsProvider
{
    /// <summary>
    /// default directory which save skills
    /// </summary>
    string SkillsLocation { get; }

    IEnumerable<SkillModel> GetSkills();
}
