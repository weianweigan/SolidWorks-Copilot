using Copilot.Sw.Skills;
using Copilot.Sw.Skills.SketchSkill;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Copilot.SwTests.Skills;

[TestClass]
public class CreateSketchSegmentSkillTest
{
    [TestMethod]
    public async Task Test()
    {
        var kernel = StandandAloneSw.S_Instance.InitKernel();

        var skill = kernel.ImportSemanticSkillFromDirectory(
            new SkillsProvider().SkillsLocation,
            "SketchSkill");

        var result = await kernel.RunAsync(
            "我需要在SolidWorks草图中绘制10个直径为100的圆，竖直排列，间距10",
            skill["CreateSketchSegment"]);

        Console.WriteLine(result);
    }

    //[TestMethod]
    //public async Task PlanTest()
    //{
    //    var kernel = StandandAloneSw.S_Instance.InitKernel();

    //    var planner = kernel.ImportSkill(new PlannerSkill(kernel));

    //    var skill = kernel.ImportSemanticSkillFromDirectory(
    //        new SkillsProvider().SkillsLocation,
    //        "SketchSkill");
    //    kernel.ImportSkill(new SketchSegmentCreationSkill());

    //    var result = await kernel.RunAsync(
    //        "我需要在SolidWorks草图中绘制10个直径为100的圆，竖直排列，间距10",
    //        skill["CreateSketchSegment"],
    //        skill["CreateCircle"],
    //        planner["CreatePlan"]);

    //    var executionResults = result;

    //    var planString = result.Variables.ToPlan().PlanString;
    //    Console.WriteLine(planString);
    //    int step = 1;
    //    int maxSteps = 10;
    //    while (!executionResults.Variables.ToPlan().IsComplete && step < maxSteps)
    //    {
    //        var results = await kernel.RunAsync(executionResults.Variables, planner["ExecutePlan"]);
    //        if (results.Variables.ToPlan().IsSuccessful)
    //        {
    //            Console.WriteLine($"Step {step} - Execution results:\n");
    //            Console.WriteLine(results.Variables.ToPlan().PlanString);

    //            if (results.Variables.ToPlan().IsComplete)
    //            {
    //                Console.WriteLine($"Step {step} - COMPLETE!");
    //                Console.WriteLine(results.Variables.ToPlan().Result);
    //                break;
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Step {step} - Execution failed:");
    //            Console.WriteLine(results.Variables.ToPlan().Result);
    //            break;
    //        }

    //        executionResults = results;
    //        step++;
    //        Console.WriteLine("");
    //    }
    //    Console.WriteLine(result.Variables.ToPlan().PlanString);
    //}
}
