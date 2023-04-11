using CommunityToolkit.Mvvm.DependencyInjection;
using SolidWorks.Interop.sldworks;

namespace Copilot.Sw.Skills;

public abstract class SldWorksSkillContext
{
    /// <summary>
    /// SolidWorks Interface
    /// </summary>
    public ISldWorks? Sw => Ioc.Default.GetService<IAddin>()?.Sw;

    /// <summary>
    /// SolidWorks Active Document
    /// </summary>
    public IModelDoc2 ActiveSwDoc => Sw?.IActiveDoc2;
}