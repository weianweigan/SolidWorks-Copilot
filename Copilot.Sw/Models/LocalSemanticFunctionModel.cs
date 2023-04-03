using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Copilot.Sw.Models;

/// <summary>
/// 本地语义函数
/// </summary>
public class LocalSemanticFunctionModel
{
    public LocalSemanticFunctionModel(string pathName,string category, string skillDir)
    {
        PathName = pathName;
        Category = category;
        SkillDir = skillDir;
        Name = new DirectoryInfo(Path.GetDirectoryName(pathName)).Name;

        var configFile = Path.Combine(
            Path.GetDirectoryName(pathName),
            "config.json");

        if (File.Exists(configFile))
        {
            var data = File.ReadAllText(configFile);

            var dic = JsonSerializer.Deserialize<Dictionary<string, object>>(data);
            if (dic?.TryGetValue("description", out var value) == true)
            {
                Description = value.ToString();
            }
        }
    }

    #region Properties
    /// <summary>
    /// Funcation Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// PathName
    /// </summary>
    public string PathName { get; }

    /// <summary>
    /// Funcation Description
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Skill Name
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Skill dir
    /// </summary>
    public string SkillDir { get; }
    #endregion

    public override string ToString() => Name;
}
