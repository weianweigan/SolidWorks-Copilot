using CommunityToolkit.Mvvm.DependencyInjection;
using Copilot.Sw.Skills;
using SolidWorks.Interop.swconst;

namespace Copilot.Sw.Extensions;

public static class ISldWorksExtensions
{
    public static SwWorkingContext GetSwCurrentContext()
    {
        var addin = Ioc.Default.GetService<IAddin>();

        var doc = addin.Sw.IActiveDoc2;

        if (doc == null)
        {
            return SwWorkingContext.SolidWorks;
        }

        var ske = doc.SketchManager.ActiveSketch;
        if (ske != null)
        {
            return SwWorkingContext.Sketch;
        }

        var type = (swDocumentTypes_e)doc.GetType();
        if (type == swDocumentTypes_e.swDocASSEMBLY)
        {
            return SwWorkingContext.Assembly;
        }
        else if (type == swDocumentTypes_e.swDocDRAWING)
        {
            return SwWorkingContext.Drawing;
        }
        else if (type == swDocumentTypes_e.swDocPART)
        {
            return SwWorkingContext.Part;
        }

        return SwWorkingContext.UnKnown;
    }
}
