using GPS.SimpleMVC.Controllers;

using static ChatGPT.REPL.SimpleMVC.ModalDialogButtons;

namespace ChatGPT.REPL.SimpleMVC;

public class ChatGptController : SimpleControllerBase
{
    public ChatHistoryDbContext DbContext
    {
        get;
    }

    public ILogger<ChatGptController> Logger
    {
        get;
    }

    public IStatusView StatusView
        => Views
            .Values
            .OfType<IStatusView>()
            .FirstOrDefault();

    public IModalDialogView ModalDialogView
        => Views
            .Values
            .OfType<IModalDialogView>()
            .FirstOrDefault();

    public IModalDialogView<T> GetModalDialogView<T>()
        => Views
            .Values
            .OfType<IModalDialogView<T>>()
            .FirstOrDefault();

    public ChatGptController(
        ChatHistoryDbContext dbContext,
        ILogger<ChatGptController> logger)
        : base()
    {
        DbContext = dbContext;
        Logger = logger;
    }

    public void AddChatPageView(IChatView chatView)
    {
        if (AddOrUpdateView(chatView))
        {
            chatView.GetResults -= ChatView_GetResults;
            chatView.GetResults += ChatView_GetResults;
            chatView.DeletePromptResponse -= ChatView_DeletePromptResponse;
            chatView.DeletePromptResponse += ChatView_DeletePromptResponse;

            LogInformation($"Added IChatView {chatView.ViewKey}");
        }
    }

    public void AddModalDialogView(IModalDialogView modalDialogView)
    {
        if (AddOrUpdateView(modalDialogView))
        {
            LogInformation($"Added IModalDialogView {modalDialogView.ViewKey}");
        }
    }

    public void AddStatusPageView(IStatusView statusView)
    {
        if (AddOrUpdateView(statusView))
        {
            LogInformation($"Added IStatusView {statusView.ViewKey}");
        }
    }

    private void ChatView_DeletePromptResponse(object sender, PromptResponse promptResponse)
    {
        if (sender is IChatView chatView && ModalDialogView is not null)
        {
            Task.Run(async () =>
            {
                try
                {
                    PromptResponse toDelete = DbContext.Find<PromptResponse>(promptResponse.UID);

                    if (toDelete is not null)
                    {
                        const string TITLE = "Confirm Delete";
                        string content = $"Confirm deletion of {toDelete.Prompt}\n{toDelete.Response}";
                        const ModalDialogButtons BUTTONS = Yes | No;
                        ModalDialogResult<object> result =
                            await ModalDialogView.ShowModalDialogAsync(new(TITLE, content, BUTTONS, No));

                        if (!(result?.IsNegativeResponse ?? true))
                        {
                            LogInformation($"Deleting {promptResponse}");
                            DbContext.Remove(toDelete);
                            int rows = await DbContext.SaveChangesAsync(true);

                            if (rows != 1)
                            {
                                throw new InvalidOperationException(
                                    $"Row count of {rows} is not the expected count of 1.");
                            }

                            LogStatus($"Deleted [{promptResponse.Prompt}]");

                            chatView.NavManager.NavigateTo(chatView.NavManager.Uri, true, true);
                        }
                        else
                        {
                            LogStatus($"User cancelled deletion of [{promptResponse.Prompt}]");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, $"Error deleting response {promptResponse.UID}");
                }
            });
        }
    }

    public void LogStatus(string status)
    {
        LogInformation(status);

        if (StatusView is not null)
        {
            StatusView.Status = status;
        }
    }

    public void LogInformation(string information)
        => Logger.LogInformation(information);

    public void LogError(Exception ex, string message)
        => Logger.LogError(ex, message);

    private List<PromptResponse> ChatView_GetResults(string arg)
    {
        try
        {
            List<PromptResponse> result = DbContext
                .PromptResponses
                .Where(r => r.SessionName == arg)
                .ToList()
                .OrderBy(r => r.Timestamp)
                .ToList();

            LogStatus($"Found {result.Count} Responses for [{arg}].");

            return result;
        }
        catch (Exception ex)
        {
            ex.Data.Add("SessionName", arg);
            LogError(ex, $"Error finding SessionName {arg}.");
            throw;
        }
    }

    public override bool Initialize() => true;
}