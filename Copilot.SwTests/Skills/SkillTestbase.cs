using Microsoft.SemanticKernel;

namespace Copilot.SwTests.Skills;

public class SkillTestbase
{
    private IKernel _kernel;

    public IKernel Kernel => _kernel ??= StandandAloneSw.S_Instance.InitKernel();

    public string SkillsDir()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        
        while (!dir.GetDirectories().Any(p => p.Name == ".git"))
        {
            dir = dir.Parent;
        }

        return Path.Combine(dir.FullName, "Copilot.Sw", "Skills");
    }
}
