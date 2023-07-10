using ChatGPT.REPL.SimpleMVC;

namespace ChatGPT.REPL.Shared;
public partial class DialogButtonComponent : ComponentBase
{
    public AutoResetEvent ResetEvent
    {
        get;
        init;
    }

    public ModalDialogRequest Request
    {
        get; init;
    }
    public ModalDialogButtons Button
    {
        get;
        init;
    }

    public ModalDialogButtons DefaultButton
    {
        get;
        init;
    }

    public Task OnClick()
    {
        try
        {
            return Task.FromResult(Click?.Invoke(Request, Button));
        }
        finally
        {
            ResetEvent.Set();
        }
    }

    public Func<ModalDialogRequest, ModalDialogButtons, ModalDialogResult<object>> Click
    {
        get;
        set;
    }

    public string ButtonClass => $"btn {GetButtonStyle()}";

    public IModalDialogView ModalDialogView
    {
        get;
        internal set;
    }

    private string GetButtonStyle() => Button == DefaultButton ? "btn-primary" : "btn-secondary";
}
