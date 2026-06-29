namespace Web.Models
{
    public class CustomDatePickerModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string? Value { get; set; }
        public bool EnableTime { get; set; } = true;
        public bool Disabled { get; set; }
    }
}
