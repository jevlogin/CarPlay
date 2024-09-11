using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


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

        public async Task HandlePollingErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"An error occurred during handling admin message: {exception}");

            if (exception is ApiRequestException apiException)
            {
                await Console.Out.WriteLineAsync($"API error occurred: {apiException.ErrorCode} - {apiException.Message}");
            }
            else
            {
                await Console.Out.WriteLineAsync("An unknown error occurred.");
            }
            await Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Unknown:
                    break;
                case UpdateType.Message:
                    if (update.Message is not { } message) return;
                    _switchMessageType(message, cancellationToken);
                    break;
                case UpdateType.InlineQuery:
                    break;
                case UpdateType.ChosenInlineResult:
                    break;
                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery is not { } callbackQuery)
                        return;
                    await HandleCallBackQuery(callbackQuery, cancellationToken);
                    break;
                case UpdateType.EditedMessage:
                    break;
                case UpdateType.ChannelPost:
                    break;
                case UpdateType.EditedChannelPost:
                    break;
                case UpdateType.ShippingQuery:
                    break;
                case UpdateType.PreCheckoutQuery:
                    break;
                case UpdateType.Poll:
                    break;
                case UpdateType.PollAnswer:
                    break;
                case UpdateType.MyChatMember:
                    break;
                case UpdateType.ChatMember:
                    break;
                case UpdateType.ChatJoinRequest:
                    break;
            }
        }

        private async Task HandleCallBackQuery(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var callbackData = callbackQuery.Data;
            
        }

        


        public bool IsCanHadle(long userId)
        {
            if (_adminList.ContainsKey(userId))
            {
                return true;
            }
            return false;
        }

        #endregion


        #region Methods

        private async void _switchMessageType(Message message, CancellationToken cancellationToken)
        {
            switch (message.Type)
            {
                case MessageType.Unknown:
                    break;
                case MessageType.Text:
                    if (message.Text is not { } text) return;

                    if (text.StartsWith('/'))
                    {
                        await HandleCommandAsync(message, text, cancellationToken);
                    }
                    else
                    {
                        await HandleTextAsync(message, text, cancellationToken);
                    }

                    break;
                case MessageType.Photo:
                    break;
                case MessageType.Audio:
                    break;
                case MessageType.Video:
                    break;
                case MessageType.Voice:
                    break;
                case MessageType.Document:
                    break;
                case MessageType.Sticker:
                    break;
                case MessageType.Location:
                    break;
                case MessageType.Contact:
                    break;
                case MessageType.Venue:
                    break;
                case MessageType.Game:
                    break;
                case MessageType.VideoNote:
                    break;
                case MessageType.Invoice:
                    break;
                case MessageType.SuccessfulPayment:
                    break;
                case MessageType.WebsiteConnected:
                    break;
                case MessageType.ChatMembersAdded:
                    break;
                case MessageType.ChatMemberLeft:
                    break;
                case MessageType.ChatTitleChanged:
                    break;
                case MessageType.ChatPhotoChanged:
                    break;
                case MessageType.MessagePinned:
                    break;
                case MessageType.ChatPhotoDeleted:
                    break;
                case MessageType.GroupCreated:
                    break;
                case MessageType.SupergroupCreated:
                    break;
                case MessageType.ChannelCreated:
                    break;
                case MessageType.MigratedToSupergroup:
                    break;
                case MessageType.MigratedFromGroup:
                    break;
                case MessageType.Poll:
                    break;
                case MessageType.Dice:
                    break;
                case MessageType.MessageAutoDeleteTimerChanged:
                    break;
                case MessageType.ProximityAlertTriggered:
                    break;
                case MessageType.WebAppData:
                    break;
                case MessageType.VideoChatScheduled:
                    break;
                case MessageType.VideoChatStarted:
                    break;
                case MessageType.VideoChatEnded:
                    break;
                case MessageType.VideoChatParticipantsInvited:
                    break;
                case MessageType.Animation:
                    break;
                case MessageType.ForumTopicCreated:
                    break;
                case MessageType.ForumTopicClosed:
                    break;
                case MessageType.ForumTopicReopened:
                    break;
                case MessageType.ForumTopicEdited:
                    break;
                case MessageType.GeneralForumTopicHidden:
                    break;
                case MessageType.GeneralForumTopicUnhidden:
                    break;
                case MessageType.WriteAccessAllowed:
                    break;
                case MessageType.UserShared:
                    break;
                case MessageType.ChatShared:
                    break;
            }
        }

        private async Task HandleTextAsync(Message message, string text, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var adminId = message.From.Id;
            var currentAdmin = _adminList[adminId];

            if (message.ReplyToMessage is { } replyToMessage)
            {
                if (replyToMessage.ForwardFrom is { } forwardFrom && forwardFrom.Id != _bot.BotId)
                {
                    var userId = forwardFrom.Id;
                    var userFromServer = message.From;
                    
                    var userLocal = _userList[userId];

                    //var msgText = $"<b>{currentAdmin.Name}:{currentAdmin.Id}</b>: {userLocal.Name}:{userLocal.Id} - <i>{text}</i>";
                    var msgText = $"<b>{currentAdmin.Name}</b>:\n\n{userLocal.Name}:{userLocal.Id} - <i>{text}</i>";

                    try
                    {
                        await _bot.SendTextMessageAsync(userId, msgText,
                                                                parseMode: ParseMode.Html, replyToMessageId: replyToMessage.MessageId,
                                                                cancellationToken: cancellationToken);
                    }
                    catch (ApiRequestException ex)
                    {
                        await Console.Out.WriteLineAsync($"По не известной причине, блок метода - 'HandleTextAsync', не отработал.\n{ex.Message}");

                        await _bot.SendTextMessageAsync(userId, msgText, parseMode: ParseMode.Html,
                                                                cancellationToken: cancellationToken);
                    }
                }
                return;
            }
        }


        private async Task HandleCommandAsync(Message message, string text, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var adminId = message.From.Id;
            var currentAdmin = _adminList[adminId];

            var command = text.Split(' ')[0].ToLower();
            var args = text.Split(' ').Skip(1).ToArray();

            switch (command)
            {
                case "/start":
                    await _welcomeAdmin(chatId, currentAdmin);
                    break;
                case "/commands":
                    await _bot.SendTextMessageAsync(chatId,
                         "Вы можете использовать следующие команды:\n\n" +
                        "/start - Начать общение\n" +
                        "/deleteuser - Удалить пользователя\n" +
                        "/addadmin - Добавить администратора\n");
                    break;
                case "/addadmin":
                    await AddAdminCommandAsync(message, args, cancellationToken);
                    break;
                case "/deleteuser":
                    await _deleteUserAsync(message, args, cancellationToken);
                    break;
                default:
                    await _bot.SendTextMessageAsync(message.Chat.Id, DialogData.ANY_SOME_COMMANDS, cancellationToken: cancellationToken);
                    break;
            }
        }

        private async Task _deleteUserAsync(Message message, string[] args, CancellationToken cancellationToken)
        {
            if (message.From?.Id is not { } id) return;
            if (args.Length > 0)
            {
                long userId = long.Parse(args[0]);
                if (id == userId)
                {
                    await _bot.SendTextMessageAsync(id, "удаление себя..");
                    await _youCantDoIts(id);
                    return;
                }

                AppUser? deletedUser;
                if (_userList.TryGetValue(userId, out deletedUser))
                {
                    Console.WriteLine("Пользователь найден и скоро будет удален");
                }
                else if (_adminList.TryGetValue(userId, out deletedUser))
                {
                    Console.WriteLine("Администратор найден и скоро будет удален");
                }
                else
                {
                    await _bot.SendTextMessageAsync(id, $"Пользователя с [ID: {userId}] не существует. Проверьте запрос.");
                }
                if (deletedUser == null) return;

                if (deletedUser.IsSuperAdmin)
                {
                    await _youCantDoIts(id);
                    return;
                }
                else if (deletedUser.IsAdmin && !_adminList[id].IsSuperAdmin)
                {
                    await _youCantDoIts(id);
                    return;
                }
                var resultDeleted = await _db.DeleteUserAsync(deletedUser);
                if (resultDeleted)
                {
                    if (deletedUser.IsAdmin)
                    {
                        _adminList.Remove(deletedUser.Id);
                    }
                    else
                    {
                        _userList.Remove(deletedUser.Id);
                    }
                    await _bot.SendTextMessageAsync(id, "Пользователь был успешно удален");
                }
            }
            else
            {
                await _bot.SendTextMessageAsync(id,
                        "Пожалуйста введите корректные данные:\n\n[ID пользователя]",
                        cancellationToken: cancellationToken);
            }
        }


        private async Task _youCantDoIts(long id)
        {
            await _bot.SendTextMessageAsync(id, "ЗАПРЕЩЕНО");
        }

        private async Task AddAdminCommandAsync(Message message, string[] args, CancellationToken cancellationToken)
        {
            if (message.From?.Id is { } id)
            {
                if (args.Length > 0)
                {
                    long userId = long.Parse(args[0]);
                    string name = !string.IsNullOrEmpty(args[1]) ? args[1] : $"Admin_{userId}";

                    if (_adminList.TryGetValue(userId, out var admin))
                    {
                        var msgInfo = $"Такой администратор уже есть";
                        await Console.Out.WriteLineAsync(msgInfo);
                        await _bot.SendTextMessageAsync(message.Chat.Id, msgInfo);
                        return;
                    }
                    try
                    {
                        if (_userList.TryGetValue(userId, out var user))
                        {
                            user.IsAdmin = true;
                            await _db.AddUserAsync(user);
                            _userList.Remove(userId);
                        }
                        else
                        {
                            AppUser newAdmin  = new AppUser { Id = userId, IsAdmin = true, FirstName = name };
                            await _db.AddUserAsync(newAdmin);
                            _adminList.Add(newAdmin.Id, newAdmin);
                        }


                        await _bot.SendTextMessageAsync(id, $"Пользователь - {name} был успешно назначен Администратором.");
                    }
                    catch (Exception ex)
                    {
                        await _bot.SendTextMessageAsync(id, $"Не удалось назначить права пользователю - ${name}");
                    }
                }
                else
                {
                    await _bot.SendTextMessageAsync(id,
                        "Пожалуйста введите корректные данные:\n\n[ID пользователя] + [Имя пользователя]",
                        cancellationToken: cancellationToken);
                }
            }
        }
        private async Task _welcomeAdmin(long chatId, AppUser currentAdmin)
        {
            var adminName = !string.IsNullOrEmpty(currentAdmin.FirstName) ? currentAdmin.FirstName : currentAdmin.Name;
            await _bot.SendTextMessageAsync(chatId, $"Приветствую тебя {adminName}\n\nТы управляешь этим чатом.");
        }
        #endregion
    }
}