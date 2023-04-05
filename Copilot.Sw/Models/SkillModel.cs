using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

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
            .Select(p => new LocalSemanticFunctionModel(p,Name,skillDir))
            .ToList();

        var configPathName = Path.Combine(skillDir, "config.json");
        if (File.Exists(configPathName))
        {
            Config = JsonSerializer.Deserialize<SkillConfigModel>(File.ReadAllText(configPathName));
        }
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

    /// <summary>
    /// Config file for this skill dir.
    /// </summary>
    /// <remarks>
    /// Only for solidworks related skill
    /// </remarks>
    public SkillConfigModel? Config { get; }

    /// <summary>
    /// index when loop
    /// </summary>
    public int Index { get; internal set; }
    #endregion

    public override string ToString() => Name;
}
