using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.SemanticKernel.Orchestration;
using System;
using System.Collections.ObjectModel;

namespace Copilot.Sw.Models;

public class Conversation:ObservableObject
{
    public ObservableCollection<Message> Messages { get; set; } = new ();

    internal void AddAsk(string input)
    {
        Messages.Add(Message.CreateAsk(input));
    }

    internal void AddAnswer(SKContext result)
    {
        Messages.Add(Message.CreateAnswer(result.Result));
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
}
