using ChatGPT.REPL.SimpleMVC;
using ChatGPT.REPL.Views;

using CommunityToolkit.Maui.Views;

using static Microsoft.Maui.Primitives.LayoutAlignment;

namespace ChatGPT.REPL;

public partial class MainPage : ContentPage, IModalDialogView
{
    public MainPage()
    {
        InitializeComponent();
        Controller = MauiProgram.Services.GetRequiredService<ChatGptController>();
        Controller.AddModalDialogView(this);
    }

    public string ModalContent
    {
        get;
        set;
    }
    public string ModalHeader
    {
        get;
        set;
    }
    public ModalDialogResult<object> ModalDialogResult
    {
        get;
        private set;
    }
    public Guid ViewKey
    {
        get;
    } = Guid.NewGuid();
    public ChatGptController Controller
    {
        get;
    }

    public async Task<ModalDialogResult<object>> ShowModalDialogAsync(ModalDialogRequest request)
    {
        await Dispatcher.DispatchAsync(async () => await ShowDialog(request));
        return ModalDialogResult;
    }

    private async Task ShowDialog(ModalDialogRequest request)
    {
        DialogView content = new(request);

        ModalDialogResult =
            (await this.ShowPopupAsync(content))
                as ModalDialogResult<object>;
    }
}
