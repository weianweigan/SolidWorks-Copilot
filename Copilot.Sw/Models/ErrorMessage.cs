namespace Copilot.Sw.Models;

public class ErrorMessage : Message
{
    public override MessageType MessageType => MessageType.Error;
}