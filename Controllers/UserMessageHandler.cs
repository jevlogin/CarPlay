using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WORLDGAMDEVELOPMENT
{
    internal sealed class UserMessageHandler : IMessageHandler
    {
        #region Fields

        private readonly Dictionary<long, AppUser> _adminList;
        private TelegramBotClient _bot;
        private DatabaseService _databaseService;
        private Dictionary<long, AppUser> _userList;
        private Dictionary<string, int> _buttonMsgId = new();
        private AppUser? _currentUser;
        private bool _isCanQuerry = false;

        #endregion


        #region ClassLifeCycles

        public UserMessageHandler(TelegramBotClient bot, DatabaseService databaseService, Dictionary<long, AppUser> userList, Dictionary<long, AppUser> adminList)
        {
            _bot = bot;
            _databaseService = databaseService;
            _userList = userList;
            _adminList = adminList;
        }

        #endregion


        #region IMessageHandler

        public async Task HandlePollingErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"An error occurred during handling user message: {exception}");

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
                    Console.WriteLine("Пришли не известные данные от пользователя");
                    break;
                case UpdateType.Message:
                    if (update.Message is not { } message)
                        return;

                    switch (message.Type)
                    {
                        case MessageType.Unknown:
                            Console.WriteLine("Пришли не известные данные в сообщении от пользователя");
                            break;
                        case MessageType.Text:
                            if (message.Text is not { } text)
                                return;
                            if (text.StartsWith('/'))
                            {
                                await HandleCommandMsgAsync(message, cancellationToken);
                            }
                            else
                            {
                                await HandleTextMsgAsync(message, cancellationToken);
                            }

                            if (_isCanQuerry)
                                _isCanQuerry = false;
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


        #endregion


        private async Task HandleTextMsgAsync(Message message, CancellationToken canToken)
        {
            await Console.Out.WriteLineAsync($"{message.Text}");

            var userId = message.From.Id;
            var isRegUser = false;

            if (_userList.TryGetValue(userId, out _currentUser) && !string.IsNullOrEmpty(_currentUser.Name))
            {
                isRegUser = true;
            }

            if (message.ReplyToMessage is { } replyToMessage || _isCanQuerry)
            {
                await SendMsgAllAdmins(message.Chat.Id, message.MessageId, canToken);
                return;
            }

            if (isRegUser)
            {
                await ThirdMsgAfterRegister(message, canToken, _currentUser?.Name);
            }
            else
            {
                await SendMsgUnknowUser(message, userId, canToken);
            }
        }

        private async Task SendMsgAllAdmins(long chatId, int messageId, CancellationToken canToken)
        {
            foreach (var adminId in _adminList.Keys)
            {
                await _bot.ForwardMessageAsync(
                        adminId,
                        chatId,
                        messageId,
                        cancellationToken: canToken);
            }
        }

        private async Task ThirdMsgAfterRegister(Message message, CancellationToken canToken, string? name)
        {
            var replyMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Gurdini", "Gurdini"),
                    InlineKeyboardButton.WithCallbackData("Carplay Adapter", "CarplayAdapter")
                },
            });
            string msg = string.Format(DialogData.THIRD_MESSAGE, name);
            var msgButton = await _bot.SendTextMessageAsync(message.Chat.Id, msg, replyMarkup: replyMarkup, cancellationToken: canToken);

            _buttonMsgId["productSelection"] = msgButton.MessageId;

        }

        private async Task DeleteButtonMessageAsync(ChatId chatId)
        {
            if (_buttonMsgId.TryGetValue("productSelection", out int msgId))
            {
                await _bot.DeleteMessageAsync(chatId, msgId);
                _buttonMsgId.Remove("productSelection");
            }
        }

        private async Task HandleCommandMsgAsync(Message message, CancellationToken token)
        {
            if (message.Text is { } text)
            {
                await Console.Out.WriteLineAsync($"{text}");
                var commands = text.ToLower().Split(' ');
                var command1 = commands[0];
                var userId = message.From.Id;

                switch (command1)
                {
                    case "/start":
                        if (_userList.TryGetValue(userId, out _currentUser) && !string.IsNullOrEmpty(_currentUser.Name))
                        {
                            await ThirdMsgAfterRegister(message, token, _userList[message.From.Id].Name);
                        }
                        else
                        {
                            await SendMsgUnknowUser(message, message.Chat.Id, token);
                        }
                        break;

                    case "/contacts":
                        await _bot.SendTextMessageAsync(message.Chat.Id, DialogData.CONTACTS_MSG_DEFAULT, cancellationToken: token);
                        break;

                    case "/help":
                        _isCanQuerry = true;
                        await _bot.SendTextMessageAsync(message.Chat.Id, "Что тебя интересует? Можешь просто написать свой вопрос..");

                        break;

                    default:
                        await _bot.SendTextMessageAsync(message.Chat.Id, DialogData.ANY_SOME_COMMANDS, cancellationToken: token);
                        break;
                }
            }
        }

        private async Task SendMsgUnknowUser(Message message, long userId, CancellationToken canToken)
        {
            if (_userList.ContainsKey(userId))
            {
                var user = _userList[userId];
                user.Name = message.Text;
                await _databaseService.AddUserAsync(user);

                await _bot.SendTextMessageAsync(message.Chat.Id, $"Приятно познакомиться {user.Name}");
                await Pause.Wait(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, $"Если что, ты всегда можешь поменя имя позже...");
            }
            else
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, string.Format(DialogData.FIRST_MESSAGE, message.From?.Username, userId));
                await Pause.Wait(3000);
                await _bot.SendTextMessageAsync(message.Chat.Id, string.Format(DialogData.SECOND_MESSAGE, message.From?.Username, userId));
                await _createTempUser(message.From);
            }
        }

        private async Task _createTempUser(User? from)
        {
            _currentUser = new AppUser()
            {
                Id = from?.Id ?? 0,
                TelegramUsername = from?.Username,
            };

            await _databaseService.AddUserAsync(_currentUser);

            _userList.Add(_currentUser.Id, _currentUser);
        }

        private async Task HandleCallBackQuery(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery.Data is not { } data) return;
            Console.WriteLine($"Нажата кнопка {data}");
            var chatId = callbackQuery.Message?.Chat.Id;

            switch (data)
            {
                case "Gurdini":
                    await _deleteMessageId(chatId, "productSelection");
                    InlineKeyboardMarkup replyMarkup = _switchGurdiniOctaButtons(chatId);
                    await _whatsYourProblem(chatId, replyMarkup: replyMarkup, cancellationToken);
                    break;

                case "LOW_CAPACITY":
                    await _bot.SendTextMessageAsync(chatId, DialogData.LOW_CAPACITY);
                    await _deleteMessageId(chatId, "whatsYourProblem");

                    await _didOurAnswerHelp(chatId);
                    break;
                case "RETURN_PROCESS":
                    await _bot.SendTextMessageAsync(chatId, DialogData.RETURN_PROCESS);
                    await _deleteMessageId(chatId, "whatsYourProblem");

                    await _didOurAnswerHelp(chatId);

                    break;
                case "SLOW_CHARGING":
                    await _bot.SendTextMessageAsync(chatId, DialogData.SLOW_CHARGING);
                    await _deleteMessageId(chatId, "whatsYourProblem");

                    await _didOurAnswerHelp(chatId);
                    break;
                case "NOT_CHARGING_POWERBANK":
                    await _bot.SendTextMessageAsync(chatId, DialogData.NOT_CHARGING_POWERBANK);
                    await _deleteMessageId(chatId, "whatsYourProblem");

                    await _didOurAnswerHelp(chatId);
                    break;
                case "MISSING_CABLE":
                    await _bot.SendTextMessageAsync(chatId, DialogData.MISSING_CABLE);
                    await _deleteMessageId(chatId, "whatsYourProblem");

                    await _didOurAnswerHelp(chatId);
                    break;
                case "SLOW_CHARGING_DEVICE":
                    await _bot.SendTextMessageAsync(chatId, DialogData.SLOW_CHARGING_DEVICE);
                    await _deleteMessageId(chatId, "whatsYourProblem");

                    await _didOurAnswerHelp(chatId);
                    break;
                case "SMALL_CAPACITY_AKB":
                    await _bot.SendTextMessageAsync(chatId, DialogData.SMALL_CAPACITY_AKB);
                    await _deleteMessageId(chatId, "whatsYourProblem");
                    await _didOurAnswerHelp(chatId);

                    break;
                case "DONT_CHARGE_GADGET":
                    await _bot.SendTextMessageAsync(chatId, DialogData.DONT_CHARGE_GADGET);
                    await _deleteMessageId(chatId, "whatsYourProblem");

                    await _didOurAnswerHelp(chatId);

                    break;

                case "CarplayAdapter":
                    await _deleteMessageId(chatId, "productSelection");

                    InlineKeyboardMarkup replyMarkupCarplayAdapter = _switchCarplayAdapterthreeButtons(chatId);
                    var changeModel = await _bot.SendTextMessageAsync(chatId, "Выберите модель", replyMarkup: replyMarkupCarplayAdapter);
                    _buttonMsgId["changeModelCarplayAdapter"] = changeModel.MessageId;

                    break;



                case "carplayButton":
                case "carplayAndroidAutoButton":
                case "Carplay2In1Button":
                    await _deleteMessageId(chatId, "changeModelCarplayAdapter");
                    InlineKeyboardMarkup replyMarkupAnswerButtons = _switchCarplayAdapterAnswerButtons(chatId);
                    await _whatsYourProblem(chatId, replyMarkupAnswerButtons, cancellationToken);
                    break;

                case "NOT_WORKING":
                    await _bot.SendTextMessageAsync(chatId, DialogData.NOT_WORKING);
                    await _deleteMessageId(chatId, "whatsYourProblem");
                    await _didOurAnswerHelp(chatId);
                    break;
                case "STOPPED_WORKING":
                    await _bot.SendTextMessageAsync(chatId, DialogData.STOPPED_WORKING);
                    await _deleteMessageId(chatId, "whatsYourProblem");
                    await _didOurAnswerHelp(chatId);
                    break;
                case "CANT_FIND_BLUETOOTH":
                    await _bot.SendTextMessageAsync(chatId, DialogData.CANT_FIND_BLUETOOTH);
                    await _deleteMessageId(chatId, "whatsYourProblem");
                    await _didOurAnswerHelp(chatId);
                    break;


                case "DID_OUR_ANSWER_YES":
                    await _deleteMessageId(chatId, "DID_OUR_ANSWER");
                    await Pause.Wait(500);
                    await _afterOurAnswerYes(chatId);
                    break;
                case "DID_OUR_ANSWER_NO":
                    await _deleteMessageId(chatId, "DID_OUR_ANSWER");
                    await _bot.SendTextMessageAsync(chatId, string.Format(DialogData.DID_OUR_ANSWERS_NO, _currentUser?.Name));
                    await Pause.Wait(500);
                    await _userHaveAnyQuerry(callbackQuery, chatId);

                    break;

                case "LEAVE_FEEDBACK":
                    await _deleteMessageId(chatId, "OUR_ANSWER_YES");
                    await _bot.SendTextMessageAsync(chatId, DialogData.LEAVE_REVIEW);

                    break;

                case "CONTACT_SUPPORT":
                    await _deleteMessageId(chatId, "OUR_ANSWER_YES");
                    await _userHaveAnyQuerry(callbackQuery, chatId);
                    break;
            }
        }

        private async Task _userHaveAnyQuerry(CallbackQuery callbackQuery, long? chatId)
        {
            _isCanQuerry = true;
            if (chatId is not { } id) return;
            AppUser? user;
            if (_userList.TryGetValue(id, out user))
            {
                foreach (var admin in _adminList.Values)
                {
                    await _bot.SendTextMessageAsync(chatId, "Задай свой вопрос");
                    await _bot.SendTextMessageAsync(admin.Id, $"Пользователь {user.Name} собирается задать вопрос.");
                }
            }
        }

        private async Task _afterOurAnswerYes(ChatId chatId)
        {
            var replyOAY = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(DialogData.LEAVE_FEEDBACK, "LEAVE_FEEDBACK"),
                    InlineKeyboardButton.WithCallbackData(DialogData.CONTACT_SUPPORT, "CONTACT_SUPPORT"),
                },
            });
            var didOAYMsg = await _bot.SendTextMessageAsync(chatId, string.Format(DialogData.DID_OUR_ANSWERS_YES, _currentUser?.Name), replyMarkup: replyOAY);
            _buttonMsgId["OUR_ANSWER_YES"] = didOAYMsg.MessageId;
        }

        private async Task _didOurAnswerHelp(ChatId chatId)
        {
            var replyM_DOA = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Да", "DID_OUR_ANSWER_YES"),
                    InlineKeyboardButton.WithCallbackData("Нет", "DID_OUR_ANSWER_NO"),
                },
            });
            var didOurAnswerMsg = await _bot.SendTextMessageAsync(chatId, string.Format(DialogData.DID_OUR_ANSWERS_HELP, _currentUser?.Name), replyMarkup: replyM_DOA);
            _buttonMsgId["DID_OUR_ANSWER"] = didOurAnswerMsg.MessageId;
        }

        private InlineKeyboardMarkup _switchCarplayAdapterAnswerButtons(ChatId? chatId)
        {
            var replyMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("не работает", "NOT_WORKING"),
                    InlineKeyboardButton.WithCallbackData("перестал работать через ĸаĸое-то время", "STOPPED_WORKING"),
                },
                [
                    InlineKeyboardButton.WithCallbackData("не могу найти bluetooth не подĸлючается", "CANT_FIND_BLUETOOTH"),
                    InlineKeyboardButton.WithCallbackData("оформить возврат", "RETURN_PROCESS"),
                ],
            });

            return replyMarkup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId">Chat Id From</param>
        /// <param name="msgKey">A set of buttons from Dict</param>
        /// <returns></returns>
        private async Task _deleteMessageId(ChatId? chatId, string msgKey)
        {
            if (chatId == null) return;

            if (_buttonMsgId.TryGetValue(msgKey, out var _msgKValue))
            {
                await _bot.DeleteMessageAsync(chatId, _msgKValue);
            }
            await Pause.Wait(500);
        }

        private InlineKeyboardMarkup _switchCarplayAdapterthreeButtons(ChatId? chatId)
        {
            var replyMarkup = new InlineKeyboardMarkup(new[]
           {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Carplay", "carplayButton"),
                    InlineKeyboardButton.WithCallbackData("Android auto", "carplayAndroidAutoButton"),
                },
                [
                    InlineKeyboardButton.WithCallbackData("Адптер 2in1 Carplay Android Auto ", "Carplay2In1Button"),
                ],
            });

            return replyMarkup;
        }

        private InlineKeyboardMarkup _switchGurdiniOctaButtons(ChatId? chatId)
        {
            var replyMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Не заряжает гаджеты", "DONT_CHARGE_GADGET"),
                    InlineKeyboardButton.WithCallbackData("Маленьĸая емĸость у нового аĸĸумулятора", "LOW_CAPACITY"),

                },
                [
                    InlineKeyboardButton.WithCallbackData("оформить возврат", "RETURN_PROCESS"),
                    InlineKeyboardButton.WithCallbackData("долго заряжается сам АКБ", "SLOW_CHARGING"),

                ],
                [
                    InlineKeyboardButton.WithCallbackData("не заряжается powerbank", "NOT_CHARGING_POWERBANK"),
                    InlineKeyboardButton.WithCallbackData("нет ĸабеля в ĸомплеĸте", "MISSING_CABLE"),
                ],
                [
                    InlineKeyboardButton.WithCallbackData("медленно дает заряд ", "SLOW_CHARGING_DEVICE"),
                    InlineKeyboardButton.WithCallbackData("маленьĸая емĸость АКБ ", "SMALL_CAPACITY_AKB"),
                ],

            });

            return replyMarkup;
        }

        private async Task _whatsYourProblem(ChatId? chatId, InlineKeyboardMarkup replyMarkup, CancellationToken canToken)
        {
            var msgProblem = await _bot.SendTextMessageAsync(chatId, DialogData.WHATS_YOUR_PROBLEM,
                replyMarkup: replyMarkup, cancellationToken: canToken);

            _buttonMsgId["whatsYourProblem"] = msgProblem.MessageId;
            await Pause.Wait(500);
        }

        public bool IsCanHadle(long userId)
        {
            if (!_adminList.ContainsKey(userId))
            {
                return true;
            }
            return false;
        }
    }
}