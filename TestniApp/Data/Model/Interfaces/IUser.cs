namespace Data.Model.Interfaces
{
    public interface IUser
    {
        public string DisplayName { get; }
        public string Email { get; }
        public string PasswordHash { get; }
    }
}
