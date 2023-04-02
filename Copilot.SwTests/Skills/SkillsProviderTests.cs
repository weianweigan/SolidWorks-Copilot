using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Copilot.Sw.Skills.Tests;

[TestClass()]
public class SkillsProviderTests
{
    [TestMethod()]
    public void GetSkillsTest()
    {
        var skillProvider = new SkillsProvider();   
        Assert.IsNotNull(skillProvider);

        var skills = skillProvider.GetSkills().ToList();

        Assert.IsNotNull(skills);
        Assert.IsTrue(skills.Any());
    }
}