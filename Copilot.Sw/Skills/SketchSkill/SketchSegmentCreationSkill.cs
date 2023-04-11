using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.SemanticKernel.SkillDefinition;
using System;

namespace Copilot.Sw.Skills.SketchSkill;

public class SketchSegmentCreationSkill
{
    [SKFunction("Draw or create some sketch segment is solidworks sketch,such as line,arc,and other segment.")]
    [SKFunctionName("SketchSkill")]
    public void SketchLevelPlan(string goal)
    {

    }

    [SKFunction("CreateCircle")]
    [SKFunctionName("CreateCircle")]
    public void CreateCircle(string postion)
    {
        var addin = Ioc.Default.GetService<IAddin>();

        var doc = addin.Sw.IActiveDoc2;
        var sketch = doc.SketchManager.ActiveSketch;
        if (sketch == null)
        {
            throw new System.ArgumentNullException("No Active Sketch");
        }

        if (!postion.StartsWith("("))
        {
            postion = postion.Split(':')[1];
        }
        var pair = postion.Trim().Split('-');
        var xyz = pair[0]
            .Replace("(", "")
            .Replace(")", "")
            .Split(',');

        doc.SketchManager.CreateCircleByRadius(
            double.Parse(xyz[0]) / 1000,
            double.Parse(xyz[1]) / 1000,
            double.Parse(xyz[2]) / 1000,
            double.Parse(xyz[3]) / 1000
            );
    }
}
