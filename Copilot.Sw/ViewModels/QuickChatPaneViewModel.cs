using CommunityToolkit.Mvvm.Input;
using Copilot.Sw.Config;
using Copilot.Sw.Models;
using Copilot.Sw.Skills;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Copilot.Sw.ViewModels;

public partial class QuickChatPaneViewModel : 
    WPFChatPaneViewModel
{
    public QuickChatPaneViewModel(
            IAddin addin,
            ITextCompletionProvider textCompletionProvider,
            ISkillsProvider skillsProvider) :
            base(addin, textCompletionProvider, skillsProvider)
    {

    }

    public Action? CloseAction { get; internal set; }

    [RelayCommand]
    private void Exit()
    {
        if (SendCommand.IsRunning)
        {
            SendCommand.Cancel();
        }
        CloseAction?.Invoke();
    }

    protected async override Task SendAsync(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Question))
        {
            return;
        }

        OnPropertyChanged(nameof(HasItem));

        try
        {

            //check config
            if (!_configLoadResult)
            {
                OpenSettings();
            }

            if (Kernel == null)
            {
                return;
            }

            await Conversation.ChatAsync(
                Kernel,
                _skillsProvider,
                Question,
                cancellationToken);

            //clear
            Question = "";
        }
        catch (Exception ex)
        {
            Conversation.Messages.Add(Message.CreateError(ex.Message));
        }
        finally
        {
            OnPropertyChanged(nameof(HasItem));
        }
    }
}
