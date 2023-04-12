using CommunityToolkit.Mvvm.ComponentModel;
using Copilot.Sw.Extensions;
using Copilot.Sw.Skills;
using Copilot.Sw.Skills.SolidWorksSkill;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xarial.XCad.SolidWorks.Documents.Exceptions;

namespace Copilot.Sw.Models;

public class Conversation:ObservableObject
{
    private string _history = "";

    public Conversation()
    {
        Variables.Set("history", _history);
    }

    public ObservableCollection<Message> Messages { get; set; } = new ();

    public ContextVariables Variables { get; set; } = new();

    #region Chat
    public async Task ChatAsync(
        IKernel kernel,
        ISkillsProvider skillsProvider,
        string question,
        CancellationToken cancellationToken)
    {
        Variables.Set("input", question);

        Messages.Add(Message.CreateAsk(question));

        //Create a SwPlan skill
        var plan = new SolidWorksPlanSkill(kernel, skillsProvider);
        var skills = kernel.ImportSkill(plan);

        //send
        var result = await kernel.RunAsync(
            Variables,
            cancellationToken: cancellationToken,
            skills[SolidWorksPlanSkill.Parameters.ChatWithSolidWorks]
            );

        //check error
        if (result.ErrorOccurred)
        {
            Messages.Add(Message.CreateError(result.LastErrorDescription));
            return;
        }

        //update history
        var theNewChatExchange = $"Me: {question}\nAI:{result}\n";
        AddHistory(theNewChatExchange);

        if (result.Variables.Get("plan", out var planValue)
            && SwPlanModel.TryParse(planValue, out var planModel))
        {
            //plan
            var actionMessage = new ActionAnswerMessage(planModel);
            Messages.Add(actionMessage);
        }
        else
        {
            //normal response
            Messages.Add(Message.CreateAnswer(result.ToString()));
        }
    }

    public async Task ChatWithContextAsync(
        IKernel kernel,
        ISkillsProvider skillsProvider,
        string question,
        CancellationToken cancellationToken)
    {
        Variables.Set("input", question);

        Messages.Add(Message.CreateAsk(question));

        var workingContext = ISldWorksExtensions.GetSwCurrentContext();

        kernel.ImportSkill(new DocumentCreatationSkill());

        //Create a SwPlan skill
        var plan = new SolidWorksPlanSkill(kernel, skillsProvider);
        var swPlanSkill = kernel.ImportSkill(plan, SolidWorksPlanSkill.Parameters.ChatWithSolidWorks);

        //send
        var result = await kernel.RunAsync(
            Variables,
            cancellationToken: cancellationToken,
            swPlanSkill[SolidWorksPlanSkill.Parameters.ChatWithSolidWorks]
            );

        //check error
        if (result.ErrorOccurred)
        {
            Messages.Add(Message.CreateError(result.LastErrorDescription));
            return;
        }

        //update history
        var theNewChatExchange = $"Me: {question}\nAI:{result}\n";
        AddHistory(theNewChatExchange);

        if (result.Variables.Get("Plan", out var planValue)
            && SwPlanModel.TryParse(planValue, out var planModel))
        {
            //plan
            var actionMessage = new ActionAnswerMessage(planModel);
            Messages.Add(actionMessage);
        }
        else
        {
            //normal response
            Messages.Add(Message.CreateAnswer(result.ToString()));
        }
    }
    #endregion

    #region Add
    internal void AddHistory(string theNewChatExchange)
    {
        _history += theNewChatExchange;
        Variables.Set("history", _history);
    }
    #endregion
}
