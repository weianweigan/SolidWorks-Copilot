using System.Collections.Generic;

namespace Copilot.Sw.Config;

public interface ITextCompletionProvider
{
    /// <summary>
    /// config location
    /// </summary>
    string SaveLocation { get; set; }

    /// <summary>
    /// config file pathname
    /// </summary>
    string FilePathName { get; }

    /// <summary>
    /// load file
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<TextCompletionConfig>? Load();

    /// <summary>
    /// wirte file
    /// </summary>
    /// <param name="textCompletionConfigs"></param>
    void Wirte(IList<TextCompletionConfig> textCompletionConfigs);
}
