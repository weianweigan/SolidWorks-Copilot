using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace Copilot.SwTests.Skills;

[TestClass]
public class GeneralSkill:SkillTestbase
{
    [TestMethod]
    public async Task TalkWithSolidWorks()
    {
        var dir = SkillsDir();
        var skill = Kernel.ImportSemanticSkillFromDirectory(dir, "GeneralSkill");

        var skContext = await Kernel.RunAsync(
            "新建一个工程图",
            skill["TalkWithSolidWorks"]
            );

        Console.WriteLine(skContext);
        Assert.AreNotEqual(skContext.ToString(), "1");

        skContext = await Kernel.RunAsync(
            "什么是盘盖类零件？",
            skill["TalkWithSolidWorks"]
            );

        XmlDocument doc = new();
        doc.LoadXml(skContext.ToString());

        var value = doc.InnerText;
        Console.WriteLine(value);
        Assert.IsTrue(!int.TryParse(value,out _));
    }
}
