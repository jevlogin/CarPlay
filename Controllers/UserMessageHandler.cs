using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WORLDGAMDEVELOPMENT
{
    internal sealed class UserMessageHandler : IMessageHandler
    {
        private readonly Dictionary<long, AppUser> _adminList;
        private TelegramBotClient _bot;
        private DatabaseService _databaseService;
        private Dictionary<long, AppUser> _userList;
        private Dictionary<string, int> _buttonMsgId = new();


        public UserMessageHandler(TelegramBotClient bot, DatabaseService databaseService, Dictionary<long, AppUser> userList, Dictionary<long, AppUser> adminList)
        {
            _bot = bot;
            _databaseService = databaseService;
            _userList = userList;
            _adminList = adminList;
        }




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
                    await HandleCallBackQuery(callbackQuery);
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



        public bool IsCanHandle(long userId)
        {
            if (!_adminList.ContainsKey(userId))
            {
                return true;
            }
            return false;
        }

        #endregion


        private async Task HandleTextMsgAsync(Message message, CancellationToken canToken)
        {
            await Console.Out.WriteLineAsync($"{message.Text}");

            var userId = message.From.Id;
            var isRegUser = _userList.ContainsKey(userId);
            if (isRegUser)
            {
                await MsgHasReceived(message, canToken);
            }
            else
            {
                await SendMsgUnknowUser(message, userId, canToken);
            }

            List<string> listPhones = new();
            foreach (var admin in _adminList.Values)
            {
                if (admin.UserId == 0) continue;
                if (!isRegUser)
                    await _bot.SendTextMessageAsync(admin.UserId, $"{string.Format(DialogData.USER_HAS_ID, userId)} {DialogData.USER_HAS_NOT_REGISTER} ", cancellationToken: canToken);
                else
                    await _bot.SendTextMessageAsync(admin.UserId, $"{string.Format(DialogData.USER_HAS_ID, userId)} {DialogData.USER_HAS_REGISTER} ", cancellationToken: canToken);
                await _bot.ForwardMessageAsync(admin.UserId, message.Chat.Id, message.MessageId, cancellationToken: canToken);

                if (admin.Phone is not { } phone) continue;
                listPhones.Add(phone);
            }

            if (isRegUser)
            {
                var currentDriver = _userList[userId];

                //var cars = new List<string> { "car_1", "car_2", "car_3", "car_4", "car_5", "car_6", "car_7", "car_8", "car_9", "car_10" };
                var cars = new List<string> { "A001AA 77", "B002BB 78", "C003CC 50", "E004EE 33", "K005KK 66", "M006MM 22", "H007HH 44", "P008PP 79", "T009TT 63", "X010XX 34" };

                var rows = (int)Math.Ceiling((double)cars.Count / 3);
                string prefics = $"car_selected_";

                var keyBoardRows = new List<List<InlineKeyboardButton>>();
                for (int i = 0; i < rows; i++)
                {
                    var row = cars.Skip(i * 3).Take(3).Select(car => new InlineKeyboardButton("Принять авто")
                    {
                        Text = car,
                        CallbackData = $"{prefics}{car}"
                    }).ToList();
                    keyBoardRows.Add(row);
                }
                var backButton = new InlineKeyboardButton("Назад") { CallbackData = "back" };
                var nextButton = new InlineKeyboardButton("Далее") { CallbackData = "next" };

                keyBoardRows.Add(new List<InlineKeyboardButton> { backButton, nextButton });
                var keyboard = new InlineKeyboardMarkup(keyBoardRows);

                var webAppinfo = new WebAppInfo();
                webAppinfo.Url = @"https://jevlogin.github.io/TruckBot/www/addUser.html";
                var button = new KeyboardButton("👽 Принять авто");
                button.WebApp = webAppinfo;
                var replyKeyboard = new ReplyKeyboardMarkup(button) { ResizeKeyboard = true };
                await _bot.SendTextMessageAsync(message.Chat.Id, "Чтобы выбрать авто, жмякай на кнопку", replyMarkup: replyKeyboard);

                var webAppinfo2 = new WebAppInfo();
                webAppinfo2.Url = @"https://ya.ru";
                var button2 = new InlineKeyboardButton("Открыть форму");
                button2.Url = webAppinfo2.Url;
                var keyboard2 = new InlineKeyboardMarkup(button2);
                var msg2 = await _bot.SendTextMessageAsync(message.Chat.Id, "Нажмите кнопку, чтобы открыть форму", replyMarkup: keyboard2);


                var msg = await _bot.SendTextMessageAsync(message.Chat.Id,
                   string.Format(DialogData.WELCOME_MESSAGE_DEFAULT, currentDriver.FirstName, currentDriver.UserId, currentDriver.LastName, listPhones.FirstOrDefault()),
                   replyMarkup: keyboard);

                _buttonMsgId.Clear();
                cars.ForEach(car =>
                {
                    var tmpMsg = $"{prefics}{car}";
                    Console.WriteLine(tmpMsg);
                    _buttonMsgId.Add($"{prefics}{car}", msg.MessageId);
                });

            }
        }

        private async Task HandleCommandMsgAsync(Message message, CancellationToken token)
        {
            if (message.Text is { } text)
            {
                await Console.Out.WriteLineAsync($"{text}");
                var commands = text.ToLower().Split(' ');
                var command1 = commands[0];

                switch (command1)
                {
                    case "/start":
                        if (_userList.ContainsKey(message.From.Id))
                        {
                            await _bot.SendTextMessageAsync(message.Chat.Id, "Не кипишуй, бронируй авто. Вызывай в меню пункт /my_command и да будет тебе счастье.", cancellationToken: token);
                        }
                        else
                        {
                            await SendMsgUnknowUser(message, message.Chat.Id, token);

                            //await _bot.SendTextMessageAsync(message.Chat.Id, "Хули тыкаешь?! Жди пока зарегают", cancellationToken: token);
                        }
                        break;

                    case "/my_command":

                        await _bot.SendTextMessageAsync(message.Chat.Id, "Жми /book чтобы забронировать авто, сдать авто, или посмотреть какое авто у тебя не сдано.", cancellationToken: token);
                        await _bot.SendTextMessageAsync(message.Chat.Id, "Жми /report чтобы сдать отчетность.", cancellationToken: token);

                        Console.WriteLine("Тут надо нарисовать кнопочки бронирования авто.");

                        break;

                    case "/help":
                        await _bot.SendTextMessageAsync(message.Chat.Id, "Что тебе надо старче?", cancellationToken: token);
                        await Pause.Wait(1000);
                        await _bot.SendTextMessageAsync(message.Chat.Id, $"Задавай свои вопросы админу. {DialogData.HELP_MSG_DEFAULT}", cancellationToken: token);
                        var admin = _adminList.Values.FirstOrDefault();

                        if (admin != null && !string.IsNullOrEmpty(admin.Phone))
                        {
                            await _bot.SendTextMessageAsync(message.Chat.Id, $"Вот тебе телефон менеджера - {admin?.Phone}", cancellationToken: token);
                        }


                        break;

                    case "/contacts":
                        await _bot.SendTextMessageAsync(message.Chat.Id, DialogData.CONTACTS_MSG_DEFAULT, cancellationToken: token);

                        break;
                }
            }
        }

        private async Task SendMsgUnknowUser(Message message, long userId, CancellationToken canToken)
        {
            await _bot.SendTextMessageAsync(message.Chat.Id, string.Format(DialogData.WELCOME_MESSAGE_TEMPLATE, message.From.Username, userId));
            await MsgHasReceived(message, canToken);
        }

        private async Task MsgHasReceived(Message message, CancellationToken canToken)
        {
            await _bot.SendTextMessageAsync(message.Chat.Id, DialogData.YOUR_MESSAGE_HAS_BEEN_RECEIVED, cancellationToken: canToken);
        }

        private async Task HandleCallBackQuery(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data is not { } data) return;
            Console.WriteLine($"Наша кнопка {data}");

            switch (data)
            {
                case "accept_car":

                    if (_buttonMsgId.TryGetValue(data, out var msgId))
                    {
                        await _bot.DeleteMessageAsync(callbackQuery.Message?.Chat.Id, msgId);
                    }
                    await _bot.SendTextMessageAsync(callbackQuery.Message?.Chat.Id, "Введите номер авто в формате x999xx");


                    break;

                case "car_selected_car_1":
                    Console.WriteLine("car_selected_car_1");
                    if (_buttonMsgId.TryGetValue(data, out var car_selected_car_1))
                    {
                        await _bot.DeleteMessageAsync(callbackQuery.Message?.Chat.Id, car_selected_car_1);
                    }
                    break;
                case "car_selected_car_2":
                    Console.WriteLine("car_selected_car_2");
                    if (_buttonMsgId.TryGetValue(data, out var car_selected_car_2))
                    {
                        await _bot.DeleteMessageAsync(callbackQuery.Message?.Chat.Id, car_selected_car_2);
                    }
                    break;
                case "car_selected_car_3":
                    Console.WriteLine("car_selected_car_3");
                    if (_buttonMsgId.TryGetValue(data, out var car_selected_car_3))
                    {
                        await _bot.DeleteMessageAsync(callbackQuery.Message?.Chat.Id, car_selected_car_3);
                    }
                    break;

            }
        }

    }
}