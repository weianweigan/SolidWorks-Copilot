
using SolidWorks.Interop.sldworks;
using System;

/// <summary>
/// Represent a SolidWorks Addin
/// </summary>
public interface IAddin
{
    /// <summary>
    /// Addin Installation dir
    /// </summary>
    string AddinDirectory { get; }

    /// <summary>
    /// Copilot Service
    /// </summary>
    IServiceProvider Services { get; }

    ISldWorks Sw { get; }

    nint SwHandle { get; }
}
