using Microsoft.SemanticKernel.SkillDefinition;

namespace Copilot.Sw.Skills.SolidWorksSkill;

public class DocumentCreatationSkill:SldWorksSkillContext
{
    [SKFunction("Create SolidWorks Document")]
    [SKFunctionName(nameof(DocumentCreatationSkill))]
    public string CreateSolidWorksDocument(string type)
    {
        

        return string.Empty;
    }
}
