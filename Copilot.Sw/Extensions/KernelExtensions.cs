using Copilot.Sw.Config;
using Microsoft.SemanticKernel.Configuration;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Copilot.Sw.Extensions;

public static class KernelExtensions
{
    public static bool LoadConfigs(
        this KernelConfig kernelConfig, 
        IReadOnlyList<TextCompletionConfig> configs)
    {
        if (configs?.Any() != true)
        {
            return false;
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
                kernelConfig.AddAzureOpenAITextCompletion(
                    config.Name,
                    config.Model,
                    config.Apikey,
                    config.Org
                    );
            }
        }
        kernelConfig.SetDefaultTextCompletionService(configs.First().Name);

        return true;
    }
}
