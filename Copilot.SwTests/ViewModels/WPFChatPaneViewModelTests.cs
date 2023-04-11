using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using Copilot.Sw.Config;
using Copilot.Sw.Skills;

namespace Copilot.Sw.ViewModels.Tests;

[TestClass()]
public class KernelTests
{
    [TestMethod()]
    public async Task QuestionTest()
    {
        IKernel kernel = Microsoft.SemanticKernel.Kernel.Builder.Build();

        var config = new TextCompletionProvider().Load().FirstOrDefault();
        if (config == null)
        {
            Assert.Fail("Config your Api key");
        }

        kernel.Config.AddOpenAITextCompletionService(
            config.Name, 
            config.Model,
            config.Apikey, 
            config.Org);

        var skill = kernel.ImportSemanticSkillFromDirectory(
            new SkillsProvider().SkillsLocation, 
            "QASkill");

        var result = await kernel.RunAsync(
            "Tell me somthing about solidworks", 
            skill["Question"]);
                    
        Console.WriteLine(result);
    }
}