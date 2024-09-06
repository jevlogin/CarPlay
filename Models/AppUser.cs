using Telegram.Bot.Types;


namespace WORLDGAMDEVELOPMENT
{
    internal sealed class AppUser : User
    {
        #region Properties
        
        public long UserId { get; set; }
        public string? TelegramUsername { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; } 

        #endregion


        #region override
        public override string ToString()
        {
            return $"User ID: {UserId}\n" +
                   $"Telegram Username: {TelegramUsername}\n" +
                   $"First Name: {FirstName}\n" +
                   $"Last Name: {LastName}\n" +
                   $"Email: {Email}\n" +
                   $"Phone: {Phone}\n";
        }
        #endregion
    }
}
