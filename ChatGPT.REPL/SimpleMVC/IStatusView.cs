using GPS.SimpleMVC.Views;

namespace ChatGPT.REPL.SimpleMVC;

public interface IStatusView : ISimpleView
{
    string Status
    {
        get; set;
    }
}