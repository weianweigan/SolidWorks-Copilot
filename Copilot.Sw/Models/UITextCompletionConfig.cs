using CommunityToolkit.Mvvm.ComponentModel;
using Copilot.Sw.Config;

namespace Copilot.Sw.Models;

public sealed partial class UITextCompletionConfig:ObservableObject
{
    /// <summary>
    /// name for this config
    /// </summary>
    [ObservableProperty]public string? _name;

    /// <summary>
    /// type openai of azure
    /// </summary>
    [ObservableProperty]public ServerType _type;

    /// <summary>
    /// the llm model
    /// </summary>
    [ObservableProperty] public string? _model;

    /// <summary>
    /// endpoint if using azure
    /// </summary>
    [ObservableProperty] public string? _endpoint;

    /// <summary>
    /// the api key for openai or azure
    /// </summary>
    [ObservableProperty] public string? _apikey;

    /// <summary>
    /// org,optional
    /// </summary>
    [ObservableProperty] public string? _org;

    /// <summary>
    /// 是否作为默认
    /// </summary>
    [ObservableProperty]private bool _isDefault;

    public override string ToString()
    {
        var str = $"{Type}:{Name}";
        if (!string.IsNullOrEmpty(Endpoint))
        {
            str += $"({Endpoint})";
        }
        return str;
    }

    public TextCompletionConfig ToTextCompletionConfig()
    {
        return new TextCompletionConfig()
        {
            Model = Model,
            Endpoint = Endpoint,
            Name = Name,
            Type = Type,
            Org = Org,
            Apikey = Apikey,
        };
    }
}
