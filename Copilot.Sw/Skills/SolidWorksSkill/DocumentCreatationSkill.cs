using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Copilot.Sw.Skills.SolidWorksSkill;

public class DocumentCreatationSkill:SldWorksSkillContext
{
    [SKFunction("Create a solidworks's part doucment")]
    [SKFunctionName(nameof(CreatePart))]
    public void CreatePart()
    {

    }

    [SKFunction("Create a solidworks's assembly doucment")]
    [SKFunctionName(nameof(CreateAssembly))]
    public void CreateAssembly()
    {

    }

    [SKFunction("Create a solidworks's drawing doucment")]
    [SKFunctionName(nameof(CreateDrawing))]
    public void CreateDrawing()
    {

    }

    public void Setting(SKContext context)
    {

    }
}
