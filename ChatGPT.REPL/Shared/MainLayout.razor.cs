using ChatGPT.REPL.SimpleMVC;

namespace ChatGPT.REPL.Shared;

public partial class MainLayout : IStatusView
{
    private string _status;

    [Inject]
    public ChatGptController Controller
    {
        get; set;
    }

    protected override Task OnInitializedAsync()
    {
        Controller?.AddStatusPageView(this);

        return base.OnInitializedAsync();
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
    public Guid ViewKey
    {
        get;
    } = Guid.NewGuid();

    public new void StateHasChanged()
        => InvokeAsync(base.StateHasChanged);

    public Task StateHasChangedAsync()
        => InvokeAsync(base.StateHasChanged);
}
