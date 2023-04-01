﻿using Copilot.Sw.Config;
using Copilot.Sw.Skills;
using Copilot.Sw.Skills.SketchSkill;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Configuration;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Copilot.SwTests.Skills;

[TestClass]
public class CreateSketchSegmentSkillTest
{
    [TestMethod]
    public async Task Test()
    {
        IKernel kernel = Microsoft.SemanticKernel.Kernel.Builder.Build();

        var config = new TextCompletionProvider().Load().FirstOrDefault();
        if (config == null)
        {
            Assert.Fail("Config your Api key");
        }

        kernel.Config.AddOpenAITextCompletion(
            config.Name,
            config.Model,
            config.Apikey,
            config.Org);

        var skill = kernel.ImportSemanticSkillFromDirectory(
            new SkillsProvider().SkillsLocation,
            "SketchSkill");

        var result = await kernel.RunAsync(
            "我需要在SolidWorks草图中绘制10个直径为100的圆，竖直排列，间距10",
            skill["CreateSketchSegment"]);

        Console.WriteLine(result);
    }

    [TestMethod]
    public async Task PlanTest()
    {
        IKernel kernel = Microsoft.SemanticKernel.Kernel.Builder.Build();

        var config = new TextCompletionProvider().Load().FirstOrDefault();
        if (config == null)
        {
            Assert.Fail("Config your Api key");
        }

        kernel.Config.AddOpenAITextCompletion(
            config.Name,
            config.Model,
            config.Apikey,
            config.Org);

        var planner = kernel.ImportSkill(new PlannerSkill(kernel));

        var skill = kernel.ImportSemanticSkillFromDirectory(
            new SkillsProvider().SkillsLocation,
            "SketchSkill");
        kernel.ImportSkill(new SketchSegmentCreationSkill());

        var result = await kernel.RunAsync(
            "我需要在SolidWorks草图中绘制10个直径为100的圆，竖直排列，间距10",
            skill["CreateSketchSegment"],
            planner["CreatePlan"]);

        var executionResults = result;

        int step = 1;
        int maxSteps = 10;
        while (!executionResults.Variables.ToPlan().IsComplete && step < maxSteps)
        {
            var results = await kernel.RunAsync(executionResults.Variables, planner["ExecutePlan"]);
            if (results.Variables.ToPlan().IsSuccessful)
            {
                Console.WriteLine($"Step {step} - Execution results:\n");
                Console.WriteLine(results.Variables.ToPlan().PlanString);

                if (results.Variables.ToPlan().IsComplete)
                {
                    Console.WriteLine($"Step {step} - COMPLETE!");
                    Console.WriteLine(results.Variables.ToPlan().Result);
                    break;
                }
            }
            else
            {
                Console.WriteLine($"Step {step} - Execution failed:");
                Console.WriteLine(results.Variables.ToPlan().Result);
                break;
            }

            executionResults = results;
            step++;
            Console.WriteLine("");
        }
        Console.WriteLine(result.Variables.ToPlan().PlanString);
    }
}
