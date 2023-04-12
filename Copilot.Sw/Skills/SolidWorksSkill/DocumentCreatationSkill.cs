using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Copilot.Sw.Skills.SolidWorksSkill;

public class DocumentCreatationSkill:SldWorksSkillContext
{
    //[SKFunction("SolidWorks document creation or modify settings skill")]
    //[SKFunctionName("SolidWorks")]
    //public void SolidWorksLevelPlan(SKContext context)
    //{

    //}

    [SKFunction("Create a solidworks's part doucment")]
    [SKFunctionName(nameof(CreatePart))]
    public void CreatePart()
    {
        var doc = Sw.NewPart();
    }

    [SKFunction("Create a solidworks's assembly doucment")]
    [SKFunctionName(nameof(CreateAssembly))]
    public void CreateAssembly()
    {
        var doc = Sw.NewAssembly();
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
