using System.Text.RegularExpressions;

namespace ChatGPT.REPL.Data;

public class PromptResponse
{
    public PromptResponse() : this("", -1, "", "") { }
    public PromptResponse(string sessionName, int index, string prompt, string response)
    {
        Index = index;
        SessionName = sessionName;
        Prompt = prompt;
        Response = response;
    }

    [Key]
    public Guid UID
    {
        get; set;
    } = Guid.NewGuid();

    public DateTimeOffset Timestamp
    {
        get; set;
    } = DateTimeOffset.UtcNow;

    [IgnoreDataMember]
    public DateTimeOffset LocalTimestamp
        => Timestamp.ToLocalTime();

    public int Index
    {
        get; set;
    }

    public string SessionName
    {
        get;set;
    }

    public string Prompt
    {
        get; set;
    }
    public string Response
    {
        get; set;
    }

    public string[] SplitByPattern(string pattern)
    {
        string result = Response;
        Regex regex = new (pattern);

        if (regex.IsMatch(Response))
        {
            Match match = regex.Match(Response);
            result = regex.Replace(Response, $"{match.Value}|||");
        }

        return result.Split("|||", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    [IgnoreDataMember]
    public string[] Lines => SplitByPattern("[\r\n]");

    public static implicit operator (string sessionName, int index, string prompt, string response)(PromptResponse value)
        => (value.SessionName, value.Index, value.Prompt, value.Response);
    public static implicit operator PromptResponse((string sessionName, int index, string prompt, string response) value)
        => new(value.sessionName, value.index, value.prompt, value.response);
}