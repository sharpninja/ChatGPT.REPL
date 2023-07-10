using static System.StringSplitOptions;
using static Microsoft.Maui.Primitives.LayoutAlignment;
using static ChatGPT.REPL.SimpleMVC.ModalDialogButtons;

using GPS.SimpleMVC.Views;

namespace ChatGPT.REPL.SimpleMVC;

public interface IModalDialogView : ISimpleView
{
    string ModalContent
    {
        get;
        set;
    }
    string ModalHeader
    {
        get;
        set;
    }

    ModalDialogResult<object> ModalDialogResult
    {
        get;
    }

    Task<ModalDialogResult<object>> ShowModalDialogAsync(ModalDialogRequest request);
}

public interface IModalDialogView<T> : IModalDialogView
{
    T ModalResponse
    {
        get;set;
    }

    Task<ModalDialogResult<T>> ShowTypedModalDialogAsync(ModalDialogRequest request);
}

[Flags]
public enum ModalDialogButtons
{
    Unknown=0, Yes=1, No=2, OK=4, Cancel=8, Help=16, Custom1=32, Custom2=64, Custom3=128
}

public record ModalDialogResult<T>(ModalDialogRequest Request, bool IsNegativeResponse, T Response)
{
}

public record struct ModalDialogRequest(string Header, string Content, ModalDialogButtons Buttons, ModalDialogButtons DefaultButton, params string[] Custom)
{
    public ModalDialogButtons[] ButtonList =>
        Buttons
                .ToString()
                .Split(",", RemoveEmptyEntries | TrimEntries)
        .Select(s => Enum.Parse<ModalDialogButtons>(s))
        .ToArray();

    public string Custom1 => Custom.Length >= 1 ? Custom[0] : "Custom1";
    public string Custom2 => Custom.Length >= 2 ? Custom[1] : "Custom2";
    public string Custom3 => Custom.Length >= 3 ? Custom[2] : "Custom3";
    public static implicit operator (string header, string content, ModalDialogButtons buttons, ModalDialogButtons defaultButton)(ModalDialogRequest value)
        => (value.Header, value.Content, value.Buttons, value.DefaultButton);
    public static implicit operator ModalDialogRequest((string header, string content, ModalDialogButtons buttons, ModalDialogButtons defaultButton) value)
        => new(value.header, value.content, value.buttons, value.defaultButton);
}