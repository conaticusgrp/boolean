using Discord;
using Discord.Interactions;

namespace Boolean.Util;

public class AppealModal : IModal
{
    public string Title => "Appeal Warning";
    
    [InputLabel("Appeal Reason")]
    [ModalTextInput("user_appeal", TextInputStyle.Paragraph, "Appeal Reason", maxLength: 500)]
    public string UserAppeal { get; set; }
}

public class AcceptModal : IModal
{
    public string Title => "Accept Appeal";
    
    [InputLabel("Accept Reason")]
    [ModalTextInput("accept_reason", TextInputStyle.Paragraph, "Message", maxLength: 500)]
    public string AcceptReason { get; set; }
}
