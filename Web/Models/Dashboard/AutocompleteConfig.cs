namespace Web.Models.Dashboard;

public class AutocompleteConfig
{
    public string Label { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public string InputId { get; set; } = string.Empty;
    public string SearchUrl { get; set; } = string.Empty;
    public string ListUrl { get; set; } = string.Empty;
    public string LabelField { get; set; } = "name";
}
