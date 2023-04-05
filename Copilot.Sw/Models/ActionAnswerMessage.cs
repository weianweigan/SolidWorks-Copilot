namespace Copilot.Sw.Models;

public class ActionAnswerMessage : Message
{
    public ActionAnswerMessage(SwPlanModel planModel)
    {
        PlanModel = planModel;
        Content = "任务规划:";
    }

    public override MessageType MessageType => MessageType.ActionMessage;

    public SwPlanModel PlanModel { get; }
}