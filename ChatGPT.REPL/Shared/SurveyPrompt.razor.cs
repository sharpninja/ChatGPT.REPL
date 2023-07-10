using Microsoft.AspNetCore.Components.Forms;

namespace ChatGPT.REPL.Shared;
public partial class SurveyPrompt
{

    private OpenAI_API.Chat.Conversation? _chat = null;
    private bool _isTaskRunning;
    private string _status;
    private string _sessionName = DateTimeOffset.Now.ToString("g");

    [Inject]
    public OpenAI_API.OpenAIAPI API
    {
        get; set;
    }

    [Inject]
    public ChatHistoryDbContext DbContext
    {
        get; set;
    }

    [Parameter]
    public string Title
    {
        get; set;
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

                Results.ForEach(r => r.SessionName = SessionName);

                StateHasChanged();
            }
        }
    }
    public string Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                StateHasChanged();
            }
        }
    }

    public string StatusVisibility => Status is { Length: > 0 } ? "" : "hidden";

    public string Prompt
    {
        get; set;
    }

    public InputTextArea ITA
    {
        get; set;
    }

    public EditForm EditForm
    {
        get; set;
    }

    public bool IsTaskRunning
    {
        get => _isTaskRunning;
        set
        {
            if (_isTaskRunning != value)
            {
                _isTaskRunning = value;
                StateHasChanged();
            }
        }
    }

    public OpenAI_API.Chat.Conversation Chat => _chat ??= API.Chat.CreateConversation();

    public List<PromptResponse> Results { get; } = new();

    protected new void StateHasChanged()
    {
        base.StateHasChanged();
        NavMenu.Current.InvokeStateHasChanged();
    }

    public async Task SetPrompt(PromptResponse item)
    {
        Prompt = item.Prompt;
        StateHasChanged();
    }

    EventCallback<EditContext> Callback
        => new(null, AskPrompt);

    public async Task AskPrompt()
    {
        if (Prompt is { Length: > 0, })
        {
            try
            {
                IsTaskRunning = true;
                Status = $"Asking &quot;{Prompt}&quot;";

                Chat.AppendUserInput(Prompt);
                string result = await Chat.GetResponseFromChatbot();

                Results.Add(new PromptResponse(SessionName, Results.Count, Prompt, result));

                Status = Prompt = string.Empty;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex);
                Status = ex.Message;
            }
            finally
            {
                IsTaskRunning = false;
            }
        }
    }

    public async Task SaveChat()
    {
        try
        {
            IsTaskRunning = true;

            async void action(PromptResponse r)
            {
                PromptResponse existing = await DbContext.FindAsync<PromptResponse>(r.UID);

                if (existing is null)
                {
                    DbContext.Add(r);
                }
                else
                {
                    DbContext.Update(r);
                }
            }

            Results.ForEach(action);

            int rows = await DbContext.SaveChangesAsync();

            if (rows != Results.Count)
            {
                throw new InvalidOperationException($"Saved {rows} rows, expected {Results.Count}");
            }

            Status = $"Saved at {DateTimeOffset.Now:g}";
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Status = ex.Message;
        }
        finally
        {
            IsTaskRunning = false;
        }
    }
}
