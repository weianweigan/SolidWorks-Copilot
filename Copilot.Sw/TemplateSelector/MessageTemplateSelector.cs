using Copilot.Sw.Models;
using System.Windows;
using System.Windows.Controls;

namespace Copilot.Sw.TemplateSelector;

public class MessageTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(
        object item, 
        DependencyObject container)
    {
        if (item is Message message)
        {
            return message.MessageType switch
            {
                MessageType.Answer => AnswerDataTemplate,
                MessageType.Ask => AskDataTemplate,
                MessageType.Error => ErrorDataTemplate,
                _ => null
            };
        }

        return null;
    }

    public DataTemplate? AnswerDataTemplate { get; set; }

    public DataTemplate? AskDataTemplate { get; set; }

    public DataTemplate? ErrorDataTemplate {  get; set; }
}
