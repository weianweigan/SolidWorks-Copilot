using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Copilot.Sw.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Configuration;
using System.Threading;
using Microsoft.SemanticKernel.KernelExtensions;
using System.Linq;
using Copilot.Sw.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Copilot.Sw.Config;
using MvvmDialogs;
using Copilot.Sw.Skills;
using System.Collections.Generic;
using Microsoft.SemanticKernel.Memory;
using Copilot.Sw.Extensions;
using Microsoft.SemanticKernel.CoreSkills;

namespace Copilot.Sw.ViewModels;

public partial class WPFChatPaneViewModel : ObservableObject
{
    #region Fields
    private AsyncRelayCommand? _sendCommand;
    private string? _question;
    private readonly ILogger? _logger;
    private readonly IAddin _addin;
    private readonly ITextCompletionProvider _textCompletionProvider;
    private readonly ISkillsProvider _skillsProvider;
    private bool _configLoadResult;
    private LocalSemanticFunctionModel _selectedSkill;
    #endregion

    #region Ctor
    public WPFChatPaneViewModel(
        IAddin addin,
        ITextCompletionProvider textCompletionProvider,
        ISkillsProvider skillsProvider)
    {
        _addin = addin;
        _textCompletionProvider = textCompletionProvider;
        _skillsProvider = skillsProvider;
        Skills = _skillsProvider.GetSkills()
            .SelectMany(p => p.SemanticFunctions)
            .ToList();
        //_logger = logger;
    }
    #endregion

    #region Properties
    public string? Question
    {
        get => _question; set
        {
            SetProperty(ref _question, value);
            SendCommand.NotifyCanExecuteChanged();
        }
    }

    public List<LocalSemanticFunctionModel> Skills { get; set; }

    public Conversation Conversation { get; set; } = new();

    public IKernel? Kernel { get; private set; }

    public bool HasItem => Conversation?.Messages?.Any() == true;

    public AsyncRelayCommand SendCommand { get => _sendCommand ??= new AsyncRelayCommand(SendAsync, CanSend); }
    #endregion

    #region Public Methods
    public void Init()
    {
        BuildKernel();
    }

    private void BuildKernel()
    {
        var configs = _textCompletionProvider.Load();

        if (configs?.Any() != true)
        {
            _configLoadResult = false;
            return;
        }

        Kernel = Microsoft.SemanticKernel.Kernel.Builder
            .Configure(c =>
            {
                c.LoadConfigs(configs);
            })
            .WithMemoryStorage(new VolatileMemoryStore())
            .Build();

        _configLoadResult = Kernel.Config.AllTextCompletionServices?.Any() == true;
    }
    #endregion

    #region Private Methods
    [RelayCommand]
    private void Clear()
    {
        Question = "";
    }

    [RelayCommand]
    private void OpenSettings()
    {
        try
        {
            var viewModel = _addin.Services.GetService<SettingsWindowViewModel>();
            if (viewModel == null)
            {
                throw new NullReferenceException();
            }

            var settingWindow = new SettingsWindow() { DataContext = viewModel };
            if (settingWindow.ShowDialog() == true)
            {
                settingWindow.Save();
            }

            BuildKernel();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private bool CanSend() => !string.IsNullOrEmpty(Question);

    private async Task SendAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_question))
        {
            return;
        }

        try
        {
            //check config
            if (!_configLoadResult)
            {
                OpenSettings();
            }

            //copy question
            var question = _question;
            Conversation.Variables.Set("input", question);

            //clear
            Question = "";

            //record question
            Conversation.AddAsk(question);

            //hidden shortitem
            OnPropertyChanged(nameof(HasItem));

            var plan = new SolidWorksPlanSkill(Kernel,_skillsProvider);            
            var skills = Kernel.ImportSkill(plan);

            //send
            var result = await Kernel.RunAsync(
                Conversation.Variables,
                cancellationToken: cancellationToken,
                skills[SolidWorksPlanSkill.Parameters.ChatWithSolidWorks]
                );

            //check error
            if (result.ErrorOccurred)
            {
                Conversation.AddError(result);
                return;
            }

            //update history
            var theNewChatExchange = $"Me: {question}\nAI:{result}\n";
            Conversation.AddHistory(theNewChatExchange);

            if (result.Variables.Get("Plan",out var planValue)
                && SwPlanModel.TryParse(planValue,out var planModel))
            {
                var actionMessage = new ActionAnswerMessage(planModel);
                Conversation.Messages.Add(actionMessage);
            }
            else
            {
                //response
                Conversation.AddAnswer(result);
            }
        }
        catch (Exception ex)
        {
            Conversation.AddError(ex);
        }
        finally
        {
            OnPropertyChanged(nameof(HasItem));
        }
    }
    #endregion
}