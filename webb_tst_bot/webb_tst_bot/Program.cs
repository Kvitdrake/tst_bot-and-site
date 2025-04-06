using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace webb_tst_bot
{
    class Program
    {
        private static readonly string BotToken = "7782818485:AAEwavcB7aPECAo4j57xtgwZjvjUyzasIBc";
        private static readonly string WebAppUrl = "https://35c1-31-58-141-50.ngrok-free.app"; //ngrok http https://localhost:7123
        private static TelegramBotClient botClient;

        static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient(BotToken);

            // Настройки обработчика обновлений
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // Получаем все типы обновлений
            };

            // Создаем CancellationTokenSource
            var cts = new CancellationTokenSource();

            // Запускаем прием сообщений (новая версия API)
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            Console.WriteLine("Bot is running...");
            Console.ReadLine();

            // Остановка бота при завершении
            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            // Обрабатываем только текстовые сообщения
            if (update.Message is not { } message)
                return;

            // Обрабатываем только команду /start
            if (message.Text != "/start")
                return;

            var inlineKeyboard = new InlineKeyboardMarkup(
                InlineKeyboardButton.WithWebApp("Open Web App", WebAppUrl)
            );

            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Click the button to open the Web App!",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );
        }

        private static Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}