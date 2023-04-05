using CommunityToolkit.Mvvm.ComponentModel;
using Copilot.Sw.Utils;
using Microsoft.SemanticKernel.Orchestration;
using System;
using System.Collections.ObjectModel;

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

    #region Add
    internal void AddAsk(string input)
    {
        Messages.Add(Message.CreateAsk(input));
    }

    internal void AddAnswer(SKContext result)
    {
        Messages.Add(Message.CreateAnswer(SkillsParse.Parse(result.Result).Item2));
    }

    internal void AddError(SKContext result)
    {
        Messages.Add(Message.CreateError(result.LastErrorDescription));
    }

    internal void AddError(Exception ex)
    {
        Messages.Add(Message.CreateError(ex.Message));
    }

    internal void AddAnswer(string goal)
    {
        Messages.Add(Message.CreateAnswer(goal));
    }

    internal void AddHistory(string theNewChatExchange)
    {
        _history += theNewChatExchange;
        Variables.Set("history", _history);
    }
    #endregion
}
