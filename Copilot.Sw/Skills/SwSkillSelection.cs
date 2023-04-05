using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Copilot.Sw.Skills;

public static class SwSkillSelection
{
    private static StringBuilder? _skillBuilder;

    public const string SemanticFuncation =
        """
        Create an XML plan step by step, to satisfy the goal given, to run in SolidWorks.
        To create a plan, follow these steps:
        1. From a <goal> create a <task> as a series of <skill>.
        2. Use only the [AVAILABLE SKILLS] - do not create new skills or attribute values.
        3. Only use skills that are required for the given goal.
        4. Append an "END" XML comment at the end of the plan.

        Here are some good examples:      
        [AVAILABLE SKILLS]
          Properties:
            description: Add delete or modify solidworks custom properies.
          Close:
            description: Close current document.          
          CreateFeature:
            description: Create feature in solidworks part context.
        [END AVAILABLE SKILLS]
        
        <goal>Close current document and exit solidworks</goal>
        <plan>
          <skill skillname="Close" goal="Close current solidworks"/>
        </plan><!-- END -->

        [AVAILABLE SKILLS]
          Properties:
            description: Add delete or modify solidworks custom properies.
          Close:
            description: Close current document.          
          CreateFeature:
            description: Create feature in solidworks part context.
        [END AVAILABLE SKILLS]
        
        <goal>I need a bolt model</goal>
        <plan>
        </plan><!-- END -->

        End of examples.
        
        [AVAILABLE SKILLS]
        {0}
        [END AVAILABLE SKILLS]
        
        <goal>{{$input}}</goal>
        """;

    private const string SkillTemplate =
        """
        {0}:
            description: {1}.          
        """;

    public const string AvailavleSkillsVariable = "available_skills";

    public static string GetAvailavleSkills(this ISkillsProvider skillsProvider)
    {
        if (_skillBuilder != null)
        {
            return _skillBuilder.ToString();
        }

        _skillBuilder = new StringBuilder();

        var skills = skillsProvider
            .GetSkills();

        foreach (var item in skills
            .Where(p => p.Config != null))
        {
            _skillBuilder.Append(string.Format(SkillTemplate,item.Config.Name,item.Config.Rule));
        }

        return _skillBuilder.ToString();
    }
}
