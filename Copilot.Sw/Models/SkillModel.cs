using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Copilot.Sw.Models;

/// <summary>
/// AI Skill
/// </summary>
public class SkillModel
{
    /// <summary>
    /// Create a skill model for ui present
    /// </summary>
    /// <param name="location">propmpt file pathName</param>
    public SkillModel(string skillDir)
    {
        SkillDir = skillDir;
        Name = new DirectoryInfo(skillDir).Name;

        var files = Directory.GetFiles(skillDir, "*.txt", SearchOption.AllDirectories)
            .Where(f => Path.GetFileNameWithoutExtension(f).EndsWith("skprompt"));

        SemanticFunctions = files
            .Select(p => new LocalSemanticFunctionModel(p,Name))
            .ToList();
    }

    #region Properties
    public List<LocalSemanticFunctionModel> SemanticFunctions { get;}
    
    /// <summary>
    /// Skill Name
    /// </summary>
    public string Name { get;  }

    /// <summary>
    /// Skill PathName
    /// </summary>
    public string SkillDir { get; }

    /// <summary>
    /// Skill Description
    /// </summary>
    public string? Description { get; }
    #endregion

    public override string ToString() => Name;
}
