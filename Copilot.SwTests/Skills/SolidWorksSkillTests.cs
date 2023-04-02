using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Copilot.SwTests.Skills;

[TestClass]
public class SolidWorksSkillTests:SkillTestbase
{
    [TestMethod]
    public async Task CreateTest()
    {
        var dir = SkillsDir();
        var skill = Kernel.ImportSemanticSkillFromDirectory(dir, "SolidWorksSkill");

        var skContext = await Kernel.RunAsync(
            "新建一个工程图",
            skill["CreateDocument"]
            );

        Console.WriteLine(skContext);
        Assert.AreNotEqual(skContext.ToString(), "Drawing");
    }
}
