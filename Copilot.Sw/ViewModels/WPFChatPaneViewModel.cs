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
using Copilot.Sw.Skills.SketchSkill;
using Copilot.Sw.Skills;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration.Extensions;
using System.Collections.Generic;
using Microsoft.SemanticKernel.Memory;
using System.IO;

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
    private readonly IDialogService _dialogService;
    private bool _configLoadResult;
    private SkillModel _selectedSkill;
    #endregion

    #region Ctor
    public WPFChatPaneViewModel(
        IAddin addin,
        ITextCompletionProvider textCompletionProvider,
        ISkillsProvider skillsProvider,
        IDialogService dialogService)
    {
        _addin = addin;
        _textCompletionProvider = textCompletionProvider;
        _skillsProvider = skillsProvider;
        _dialogService = dialogService;
        Skills = _skillsProvider.GetSkills().ToList();
        SelectedSkill = Skills.FirstOrDefault();
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

    public List<SkillModel> Skills { get; set; }

    public Conversation Conversation { get; set; } = new();

    public IKernel? Kernel { get; private set; }

    public bool HasItem => Conversation?.Messages?.Any() == true;

    public AsyncRelayCommand SendCommand { get => _sendCommand ??= new AsyncRelayCommand(SendAsync, CanSend); }

    public SkillModel SelectedSkill { get => _selectedSkill; set => SetProperty(ref _selectedSkill, value); }
    #endregion

    #region Public Methods
    public void Init()
    {
        BuildKernel();
    }

    private void BuildKernel()
    {
        Kernel = Microsoft.SemanticKernel.Kernel.Builder
            .Configure(c =>
            {
                LoadConfigs(c);
            })
            .WithMemoryStorage(new VolatileMemoryStore())
            .Build();
    }

    private bool LoadConfigs(KernelConfig kernelConfig)
    {
        var configs = _textCompletionProvider.Load();

        if (configs?.Any() != true)
        {
            return _configLoadResult = false;
        }

        kernelConfig.RemoveAllTextCompletionServices();
        kernelConfig.RemoveAllTextEmbeddingServices();
        foreach (var config in configs)
        {
            if (config.Type == ServerType.OpenAI)
            {
                kernelConfig.AddOpenAITextCompletion(
                    config.Name,                       // alias used in the prompt templates' config.json
                    config.Model,                     // OpenAI Model Name
                    config.Apikey,            // OpenAI API key
                    config.Org
                    );
                kernelConfig.AddOpenAIEmbeddingGeneration(
                    config.Name,
                    "text-embedding-ada-002",
                    config.Apikey,
                    config.Org
                    );
            }
            else if (config.Type == ServerType.Azure)
            {
                Kernel.Config.AddAzureOpenAITextCompletion(
                    config.Name,
                    config.Model,
                    config.Apikey,
                    config.Org
                    );
            }
        }
        kernelConfig.SetDefaultTextCompletionService(configs.First().Name);

        return _configLoadResult = true;
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

            ////use skill
            var skill = Kernel.ImportSemanticSkillFromDirectory(_skillsProvider.SkillsLocation, SelectedSkill.Name);

            //send
            var result = await Kernel.RunAsync(
                Conversation.Variables,
                cancellationToken: cancellationToken,
                skill[SelectedSkill.SemanticFunctions.First().Name]
                );

            //update history
            var theNewChatExchange = $"Me: {question}\nAI:{result}\n";
            Conversation.AddHistory(theNewChatExchange);

            //var planner = Kernel.ImportSkill(new PlannerSkill(Kernel));

            //var skill = Kernel.ImportSemanticSkillFromDirectory(
            //    new SkillsProvider().SkillsLocation,
            //    "SketchSkill");
            //Kernel.ImportSkill(new SketchSegmentCreationSkill());

            //var result = await Kernel.RunAsync(
            //    question,
            //    skill["CreateSketchSegment"],
            //    planner["CreatePlan"]);

            //var executionResults = result;

            //int step = 1;
            //int maxSteps = 10;
            //while (!executionResults.Variables.ToPlan().IsComplete && step < maxSteps)
            //{
            //    var results = await Kernel.RunAsync(executionResults.Variables, planner["ExecutePlan"]);
            //    if (results.Variables.ToPlan().IsSuccessful)
            //    {
            //        Console.WriteLine($"Step {step} - Execution results:\n");
            //        Console.WriteLine(results.Variables.ToPlan().PlanString);

            //        if (results.Variables.ToPlan().IsComplete)
            //        {
            //            Console.WriteLine($"Step {step} - COMPLETE!");
            //            Console.WriteLine(results.Variables.ToPlan().Result);
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Step {step} - Execution failed:");
            //        Console.WriteLine(results.Variables.ToPlan().Result);
            //        break;
            //    }

            //    executionResults = results;
            //    step++;
            //    Console.WriteLine("");
            //}
            //Console.WriteLine(result.Variables.ToPlan().PlanString);            

            //check error
            if (result.ErrorOccurred)
            {
                Conversation.AddError(result);
            }

            //response
            Conversation.AddAnswer(result);
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