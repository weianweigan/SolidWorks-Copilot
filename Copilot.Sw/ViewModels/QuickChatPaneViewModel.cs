using CommunityToolkit.Mvvm.Input;
using Copilot.Sw.Config;
using Copilot.Sw.Skills;
using System;
using System.Windows;

namespace Copilot.Sw.ViewModels;

public partial class QuickChatPaneViewModel : WPFChatPaneViewModel
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
        CloseAction?.Invoke();
    }
}
