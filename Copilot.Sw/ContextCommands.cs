using Copilot.Sw.Properties;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.UI.Commands.Attributes;
using Xarial.XCad.UI.Commands.Enums;

[Title("SolidWorks Copilot")]
[Icon(typeof(Resources),nameof(Resources.SolidWorksCopilot))]
public enum ContextCommands
{
    [Title("Copilot")]
    [Icon(typeof(Resources), nameof(Resources.SolidWorksCopilot))]
    SolidWorksCopilot,
}

[Title("SolidWorks Copilot")]
[Icon(typeof(Resources), nameof(Resources.SolidWorksCopilot))]
public enum Commands
{
    [Title("Copilot")]
    [Icon(typeof(Resources), nameof(Resources.SolidWorksCopilot))]
    [CommandItemInfo(true,true,WorkspaceTypes_e.All,false)]
    SolidWorksCopilot,
}