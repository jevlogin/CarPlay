using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace WORLDGAMDEVELOPMENT
{
    internal sealed class UpdateDispatcher : IUpdateHandler
    {
        #region Fields

        private readonly List<IMessageHandler> _handlers;

        #endregion


        #region ClassLifeCicles

        public UpdateDispatcher()
        {
            _handlers = new();
        }

        ~UpdateDispatcher()
        {
            _handlers?.Clear();
        }

        #endregion


        #region IUpdateHandler

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"An error occurred during dispatching update: {exception}");

            if (exception is ApiRequestException apiException)
            {
                await Console.Out.WriteLineAsync($"API error occurred: {apiException.ErrorCode} - {apiException.Message}");
            }
            else
            {
                await Console.Out.WriteLineAsync("An unknown error occurred.");
            }
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:

                    if (update.Message is { } message)
                    {
                        var userId = message.From.Id;
                        await SendUpdateMessage(botClient, update, userId, cancellationToken);
                    }
                    break;
                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery is { } callback)
                    {
                        var userId = callback.From.Id;
                        await SendUpdateMessage(botClient, update, userId, cancellationToken);
                    }
                    break;
                case UpdateType.EditedMessage:
                    break;
            }
        }

        #endregion


        #region Methods

        internal void AddHandler(IMessageHandler handler)
        {
            _handlers.Add(handler);
        }

        private async Task SendUpdateMessage(ITelegramBotClient botClient, Update update, long userId, CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
            {
                var canHandle = handler.IsCanHadle(userId);
                if (canHandle)
                {
                    await handler.HandleUpdateAsync(update, cancellationToken);
                    break; 
                }
            }
        }

        #endregion



    }
}
