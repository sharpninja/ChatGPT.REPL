using GPS.SimpleMVC.Views;

namespace ChatGPT.REPL.SimpleMVC;

public interface IChatView : ISimpleView
{
    List<PromptResponse> Results
    {
        get;
        set;
    }

    string SessionName
    {
        get;
        set;
    }

    NavigationManager NavManager
    {
        get;
    }

    event EventHandler<PromptResponse> DeletePromptResponse;
    event Func<string, List<PromptResponse>> GetResults;
}
