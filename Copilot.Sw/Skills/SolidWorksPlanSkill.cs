using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Threading.Tasks;

namespace Copilot.Sw.Skills;

public class SolidWorksPlanSkill
{
    private readonly IKernel _kernel;
    private readonly ISkillsProvider _skillsProvider;
    private readonly ISKFunction _isThrereSwTaskFunc;
    private readonly ISKFunction _taskPlanFunc;
    private readonly ISKFunction _chatFunc;
    private const string IsThereSwTask =
        """
        BEGIN CONTENT TO SUMMARIZE:
        {{$INPUT}}
        END CONTENT TO SUMMARIZE.

        Determine whether the input is a function that can be performed in SolidWorks.
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
            var plan = await _taskPlanFunc.InvokeAsync(input);
            plan.Variables.Set("Plan", plan.Result);
            return plan;
        }
        else
        {           
            var chatResult = await _chatFunc.InvokeAsync(input, context);
            return chatResult;
        }
    }
}
