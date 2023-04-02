using CommunityToolkit.Mvvm.DependencyInjection;
using SolidWorks.Interop.sldworks;

namespace Copilot.Sw.Skills;

public abstract class SldWorksSkillContext
{
    public ISldWorks? Sw => Ioc.Default.GetService<IAddin>()?.Sw;

    public IModelDoc2 ActiveSwDoc => Sw?.IActiveDoc2;
}
