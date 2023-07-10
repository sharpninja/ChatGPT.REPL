using static System.StringSplitOptions;
using static Microsoft.Maui.Primitives.LayoutAlignment;
using static ChatGPT.REPL.SimpleMVC.ModalDialogButtons;
using ChatGPT.REPL.SimpleMVC;
using CommunityToolkit.Maui.Views;
using Windows.Media.Devices;

namespace ChatGPT.REPL.Views;

public class DialogView : Popup
{
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

    public ModalDialogRequest Request
    {
        get;
    }

    public ModalDialogResult<object> ModalResult
        => (ModalDialogResult<object>)(Result.GetAwaiter().GetResult());

    public DialogView(ModalDialogRequest request)
    {
        Request = request;
        ModalHeader = request.Header;
        ModalContent = request.Content;

        StackLayout stackLayout = new();

        foreach (ModalDialogButtons button in request.ButtonList)
        {
            Button btn = new()
            {
                FontSize = 16,
                Text = $"{button}",
            };

            ModalDialogResult<object> result =
                new(request,
                    GetIsNegativeResponse(button),
                    button);

            btn.Clicked += (_, _) => Close(result);

            stackLayout.Add(btn);
        }

        Border header = new ()
        {
            Content = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = ModalHeader,
                FontSize = 26,
                TextColor = Colors.White,
            },
            Background = Brush.DarkGray,
        };

        Border content = new()
        {
            Content = new ScrollView
            {
                Content = new Label
                {
                    Text = ModalContent,
                    FontSize = 16,
                    BackgroundColor = Colors.Black,
                    TextColor = Colors.White,
                }
            }
        };

        Border footer = new ()
        {
            Content = stackLayout,
            Background = Brush.DarkGray
        };


        Content = new Grid
        {
            RowDefinitions = new RowDefinitionCollection(
            new RowDefinition(new GridLength(1, GridUnitType.Auto)),
            new RowDefinition(new GridLength(1, GridUnitType.Star)),
            new RowDefinition(new GridLength(1, GridUnitType.Auto))
        ),
            Children =
            {
                header, content, footer
            }
        };

        Grid.SetRow(header, 0);
        Grid.SetRow(content, 1);
        Grid.SetRow(footer, 2);
    }

    private static bool GetIsNegativeResponse(ModalDialogButtons button)
        => button switch
        {
            No or Cancel => true,
            _ => false
        };
}
