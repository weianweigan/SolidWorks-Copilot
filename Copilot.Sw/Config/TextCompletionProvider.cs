using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Copilot.Sw.Config;

public class TextCompletionProvider:ITextCompletionProvider
{
    public string SaveLocation { get; set; }

    public string FilePathName => Path.Combine(SaveLocation, "settings.json");

    public TextCompletionProvider()
    {
        SaveLocation = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AddIn.AddinName);
    }

    public IReadOnlyList<TextCompletionConfig>? Load()
    {
        Check();

        if (!File.Exists(FilePathName))
        {
            return null;
        }

        var text = File.ReadAllText(FilePathName);
        var configs = JsonSerializer.Deserialize<List<TextCompletionConfig>>(text);

        return configs;
    }

    public void Wirte(IList<TextCompletionConfig> textCompletionConfigs)
    {
        Check();

        if (textCompletionConfigs is null)
        {
            throw new ArgumentNullException(nameof(textCompletionConfigs));
        }

        var text = JsonSerializer.Serialize(textCompletionConfigs);

        File.WriteAllText(FilePathName, text);
    }

    private void Check()
    {
        if (!Directory.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }
    }
}
