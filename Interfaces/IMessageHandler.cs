﻿using Telegram.Bot.Types;


namespace WORLDGAMDEVELOPMENT
{
    internal interface IMessageHandler
    {
        public Task HandlePollingErrorAsync(Exception exception, CancellationToken cancellationToken);

        public Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
        public bool IsCanHandle(long userId);

    }
}