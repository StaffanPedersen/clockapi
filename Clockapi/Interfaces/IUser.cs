namespace ClockApi.Interfaces
{
    public interface IUser
    {
        
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Roles { get; set; }
        public string Email { get; set; }
    }
}
