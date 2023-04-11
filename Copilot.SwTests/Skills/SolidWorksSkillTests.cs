using Copilot.Sw.Skills;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Copilot.SwTests.Skills;

[TestClass]
public class SolidWorksPlanSkillTests : SkillTestbase
{
    [TestMethod]
    public async Task CreateTest()
    {
        var skContext = await Kernel.RunAsync(
            "什么是机械工程?",
            GetChatWithSwFunc());

        Console.WriteLine(skContext);
        Assert.AreNotEqual(skContext.ToString(), "Drawing");
    }

    [TestMethod]
    public async Task TaskTest()
    {
        var skContext = await Kernel.RunAsync(
            "草图中绘制三个圆形",GetChatWithSwFunc());

        var plan = skContext.Variables.ToPlan();

        Assert.IsTrue(skContext.Result.Contains("Nothing"));
    }

    private ISKFunction GetChatWithSwFunc()
    {
        var dir = SkillsDir();
        var skillProvider = new SkillsProvider(dir);

        var swPlanSkill = new SolidWorksPlanSkill(Kernel, skillProvider);
        var skill = Kernel.ImportSkill(swPlanSkill, SolidWorksPlanSkill.Parameters.ChatWithSolidWorks);

        var chatFunc = skill[SolidWorksPlanSkill.Parameters.ChatWithSolidWorks];
        return chatFunc;
    }
}
