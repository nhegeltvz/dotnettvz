namespace Web.Models;

public class LogEntryViewModel
{
    public string? Timestamp { get; set; }
    public string Level { get; set; } = "Information";
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new();

    public string LevelCssClass => Level switch
    {
        "Error" or "Fatal" => "log-level--error",
        "Warning" => "log-level--warning",
        "Debug" or "Verbose" => "log-level--debug",
        _ => "log-level--info"
    };

    public string LevelIcon => Level switch
    {
        "Error" or "Fatal" => "fa-circle-xmark",
        "Warning" => "fa-circle-info",
        "Debug" or "Verbose" => "fa-bug",
        _ => "fa-circle-check"
    };

    public string FormattedTimestamp
    {
        get
        {
            if (DateTime.TryParse(Timestamp, out var dt))
                return dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            return Timestamp ?? "-";
        }
    }
}
