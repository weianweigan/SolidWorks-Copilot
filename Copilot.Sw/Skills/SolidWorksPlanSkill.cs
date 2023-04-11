using CommunityToolkit.Mvvm.DependencyInjection;
using Copilot.Sw.Skills.SketchSkill;
using Copilot.Sw.Skills.SolidWorksSkill;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using SolidWorks.Interop.swconst;
using System;
using System.Threading.Tasks;

namespace Copilot.Sw.Skills;

public class SolidWorksPlanSkill
{
    #region Fields
    private readonly IKernel _kernel;
    private readonly ISkillsProvider _skillsProvider;
    private readonly ISKFunction _isThrereSwTaskFunc;

    [Obsolete("Use default planner")]
    private readonly ISKFunction _taskPlanFunc;
    private readonly ISKFunction _chatFunc;
    #endregion

    #region Consts
    private const string IsThereSwTask =
        """
        SolidWorks has some funcations listed:
        1.Create part,assembly,drawing and modify some settings.
        2.Sketch some sketch segment,such as line,arc,spline,slot.

        Determine whether the input is a function that can be performed in SolidWorks.

        BEGIN CONTENT TO SUMMARIZE:
        {{$INPUT}}
        END CONTENT TO SUMMARIZE.
        
        Answer with Y or N.
        """;

    private const string Chat =
        """
        You are an AI SolidWorks assistant.Your responses should be professional and helpful.
        {{$INPUT}}
        """;

    public static class Parameters
    {
        public const string ChatWithSolidWorks = nameof(ChatWithSolidWorks);
    }
    #endregion

    #region Ctor
    public SolidWorksPlanSkill(
        IKernel kernel,
        ISkillsProvider skillsProvider,
        int maxTokens = 1024)
    {
        _kernel = kernel;
        _skillsProvider = skillsProvider;

        _isThrereSwTaskFunc = kernel.CreateSemanticFunction(
            IsThereSwTask,
            "ChatOrTask",
            maxTokens: maxTokens,
            temperature:0d
            );

        var taskPlanPrompt = string.Format(SwSkillSelection.SemanticFuncation, SwSkillSelection.GetAvailavleSkills(_skillsProvider));
        _taskPlanFunc = kernel.CreateSemanticFunction(
            taskPlanPrompt,
            "SolidWorksTaskPlan",
            maxTokens:maxTokens,
            temperature:0.5d
            );

        _chatFunc = kernel.CreateSemanticFunction(
            Chat,
            "Chat",
            maxTokens: maxTokens,
            temperature: 0.8
            );
    }
    #endregion

    #region Methods
    [SKFunction("Chat with SolidWorks,parse solidworks perform tasks if there is a goal")]
    [SKFunctionName(Parameters.ChatWithSolidWorks)]
    public async Task<SKContext> ChatWithSolidWorksAsync(
        string input,
        SKContext context)
    {
        var askVariable = new ContextVariables(input);

        var result = await _isThrereSwTaskFunc.InvokeAsync(
            input, 
            new SKContext(
                askVariable,
                context.Memory,
                context.Skills,
                context.Log));

        if (result.Result.Trim() == "Y")
        {
            //Use Semantic Kernel Plan skill
            //var plan = await _taskPlanFunc.InvokeAsync(input);
            //plan.Variables.Set("Plan", plan.Result);
            _kernel.ImportSkill(new DocumentCreatationSkill(),nameof(DocumentCreatationSkill.SolidWorksLevelPlan));
            _kernel.ImportSkill(new SketchSegmentCreationSkill(), nameof(SketchSegmentCreationSkill.SketchLevelPlan));

            var planner = new PlannerSkill(_kernel);

            //set parameter
            context.Variables.Set(PlannerSkill.Parameters.MaxRelevantFunctions, "5");
            context.Variables.Set(PlannerSkill.Parameters.IncludedFunctions, $"{nameof(DocumentCreatationSkill.SolidWorksLevelPlan)},{nameof(SketchSegmentCreationSkill.SketchLevelPlan)}");  
            
            _kernel.ImportSkill(planner);
            return await planner.CreatePlanAsync(input, context);
        }
        else
        {           
            var chatResult = await _chatFunc.InvokeAsync(input, context);
            return chatResult;
        }
    }

    public static SwContext GetSwCurrentContext()
    {
        var addin = Ioc.Default.GetService<IAddin>();

        var doc = addin.Sw.IActiveDoc2;

        if (doc == null)
        {
            return SwContext.SolidWorks;
        }

        var ske = doc.SketchManager.ActiveSketch;
        if (ske != null)
        {
            return SwContext.Sketch;
        }

        var type = (swDocumentTypes_e)doc.GetType();
        if (type == swDocumentTypes_e.swDocASSEMBLY)
        {
            return SwContext.Assembly;
        }else if(type == swDocumentTypes_e.swDocDRAWING)
        {
            return SwContext.Drawing;
        }else if(type == swDocumentTypes_e.swDocPART)
        {
            return SwContext.Part;
        }

        return SwContext.UnKnown;
    }
    #endregion
}

/// <summary>
/// SolidWorks Current Context
/// </summary>
public enum SwContext
{
    UnKnown,
    SolidWorks,
    Part,
    Assembly,
    Drawing,
    Sketch,
}
