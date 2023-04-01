namespace Copilot.Sw.Config;

public enum ServerType
{
    OpenAI,
    Azure,
}

public sealed class TextCompletionConfig
{
    /// <summary>
    /// name for this config
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// type openai of azure
    /// </summary>
    public ServerType Type { get; set; }

    /// <summary>
    /// the llm model
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// endpoint if using azure
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// the api key for openai or azure
    /// </summary>
    public string? Apikey { get; set; }

    /// <summary>
    /// org,optional
    /// </summary>
    public string? Org { get; set; }
}
