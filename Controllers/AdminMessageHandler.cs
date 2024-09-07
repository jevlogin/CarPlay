using Telegram.Bot;
using Telegram.Bot.Types;


namespace WORLDGAMDEVELOPMENT
{
    internal class AdminMessageHandler : IMessageHandler
    {
        #region Fields
        
        private readonly TelegramBotClient _bot;
        private readonly DatabaseService _db;
        private Dictionary<long, AppUser> _userList;
        private Dictionary<long, AppUser> _adminList;

        #endregion

        
        #region ClassLifeCicles
        
        public AdminMessageHandler(TelegramBotClient bot, DatabaseService databaseService, Dictionary<long, AppUser> userList, Dictionary<long, AppUser> adminList)
        {
            _bot = bot;
            _db = databaseService;
            _userList = userList;
            _adminList = adminList;
        }

        #endregion


        #region IMessageHandler

        public Task HandlePollingErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool IsCanHadle(long userId)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Methods



        #endregion
    }
}