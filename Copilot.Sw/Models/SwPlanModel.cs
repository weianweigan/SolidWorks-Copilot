using System;
using System.Collections.Generic;
using System.Xml;

namespace Copilot.Sw.Models
{
    public class ExecuteSkillModel
    {
        public string SkillName { get; set; }

        public string Description { get; set; }
    }

    public class SwPlanModel
    {
        public List<ExecuteSkillModel> ExecuteSkills { get; } = new List<ExecuteSkillModel>();

        public static bool TryParse(string input, out SwPlanModel planModel)
        {
            planModel = new SwPlanModel();
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(input);

                foreach (XmlNode childNode in xmlDoc.FirstChild.ChildNodes)
                {
                    //childNode.Attributes;
                    var skill = new ExecuteSkillModel()
                    {
                        SkillName = childNode.Attributes["skillname"].Value,
                        Description = childNode.Attributes["goal"].Value,
                    };

                    planModel.ExecuteSkills.Add(skill);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
