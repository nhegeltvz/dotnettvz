namespace TestniApp.Models.CustomSelectControl
{
    public class MultiSelectConfig
    {
        public string ControlId { get; set; }
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string FieldName { get; set; }
        public List<SelectableItem> AvailableItems { get; set; } = [];
        public List<SelectableItem> SelectedItems { get; set; } = [];
    }
    public class SelectableItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
