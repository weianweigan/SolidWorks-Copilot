using System.Collections.Generic;

namespace Copilot.Sw.Models;

public class SkillConfigModel
{
    public string? Name { get; set; }

    public string? Rule { get; set; }

    public List<string>? Samples { get; set; }
}
