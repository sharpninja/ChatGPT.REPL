using ChatGPT.REPL.SimpleMVC;

using GPS.SimpleMVC.Views;

namespace ChatGPT.REPL.Pages;
public partial class Chat : ISimpleView, IChatView
{
    private List<PromptResponse> _results;
    private string _sessionName;

    [Inject]
    public ChatGptController Controller
    {
        get;
        set;
    }

    [Parameter]
    public string SessionName
    {
        get => _sessionName;
        set
        {
            if (_sessionName != value)
            {
                _sessionName = value;
                _results = null;
            }
        }
    }

    public string? SessionNameDecoded
        => HttpUtility.UrlDecode(SessionName);

    public List<PromptResponse> Results
    {
        get => _results ??
            (Results = GetResults?.Invoke(SessionNameDecoded));
        set
        {
            if (_results != value)
            {
                _results = value;
                StateHasChanged();
            }
        }
    }


    public Guid ViewKey
    {
        get;
    }

    [Inject]
    public NavigationManager NavManager
    {
        get;
        set;
    }

    protected override Task OnInitializedAsync()
    {
        Controller.AddChatPageView(this);
        base.OnInitializedAsync();
        return Task.CompletedTask;
    }

    public async Task DeleteResponse(PromptResponse response)
    {
        try
        {
            DeletePromptResponse?.Invoke(this, response);
        }
        catch (Exception ex)
        {
            ex.Data.Add(nameof(response), response);
            await Console.Error.WriteLineAsync(ex.ToString());
            throw;
        }
    }

    public event EventHandler<PromptResponse> DeletePromptResponse;
    public event Func<string, List<PromptResponse>> GetResults;
}