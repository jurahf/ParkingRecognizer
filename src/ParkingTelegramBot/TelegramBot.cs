using Microsoft.Extensions.Options;
using ParkingPlaces.Models;
using ParkingPlaces.Repositories;
using Recognizer.Commands;
using Recognizer.Recognizing;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ParkingTelegramBot
{
    public class TelegramBot : ITelegramBot
    {
        private readonly string token = "";
        private readonly IRepository<Parking> repository;
        private readonly IParkingRecognizer recognizer;

        public TelegramBot(
            IOptions<TelegramConfig> config,
            IRepository<Parking> repository,
            IParkingRecognizer recognizer)
        {
            token = config.Value.Token;
            this.repository = repository;
            this.recognizer = recognizer;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            ITelegramBotClient botClient = new TelegramBotClient(token);
            ReceiverOptions receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery,
                },

                DropPendingUpdates = true,
            };

            // Запускаем бота
            botClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cancellationToken);

            var me = await botClient.GetMe();

            Log.Logger.Information($"Бот \"{me.FirstName}\" запущен!");
        }

        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Log.Logger.Error(ErrorMessage);
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await HandleTextMessage(client, update);
                        break;
                    default:
                        Log.Logger.Warning($"Неожиданный тип сообщения: {update.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                await TrySendError(client, update, ex);
                Log.Logger.Error(ex, ex.Message);
            }
        }

        private async Task TrySendError(ITelegramBotClient client, Update update, Exception ex)
        {
            try
            {
                IReplyMarkup keyboard = GetStartKeyboard();

                await client.SendMessage(
                    update.Message.Chat.Id,
                    $"При обработке запроса возникла ошибка. Попробуйте позднее.",
                    replyMarkup: keyboard);
            }
            catch
            {
                Log.Logger.Error(ex, $"Ошибка при отправке ошибки: {ex.Message}");
            }
        }

        private async Task HandleTextMessage(ITelegramBotClient client, Update update)
        {
            Chat chat = null;

            var message = update.Message;
            if (message == null)
            {
                return;
            }

            IReplyMarkup keyboard = GetStartKeyboard();
            var user = message.From;
            chat = message.Chat;

            Log.Logger.Information($"Пользователь {user?.Username} ({user?.Id}) прислал сообщение в чат {chat.Id}: {message.Text?.Substring(0, Math.Min(message.Text.Length, 100))}");

            switch (message.Text)
            {
                case "/start":
                    await client.SendMessage(chat.Id, "Выберите команду на клавиатуре", replyMarkup: keyboard);
                    break;

                case "Найти места":
                    await client.SendMessage(
                        chat.Id,
                        $"Начинаю поиск...",
                        replyMarkup: keyboard);

                    // непосредственно действия
                    var command = new GetPlacesInfoCommand(repository, recognizer);
                    var info = command.Execute();

                    // результат
                    await client.SendMessage(
                        chat.Id,
                        $"На парковке {info.EmptyPlaces.Count} свободных мест и {info.InQuestionPlaces.Count} под вопросом",
                        replyMarkup: keyboard);

                    List<IAlbumInputMedia> album = new List<IAlbumInputMedia>();
                    List<StreamReader> readers = new List<StreamReader>();
                    try
                    {
                        foreach (var imagePath in info.ImagePaths)
                        {
                            StreamReader sr = new StreamReader(imagePath);
                            readers.Add(sr);

                            album.Add(new InputMediaPhoto(InputFile.FromStream(sr.BaseStream, imagePath)));
                        }

                        // TODO: отсеивать камеры, на которых нет свободных мест
                        int portion = GetPhotoSendProtion(album.Count);

                        for (int i = 0; i < album.Count; i += portion)
                        {
                            await client.SendMediaGroup(chat.Id, album.Skip(i).Take(portion));
                        }
                    }
                    finally
                    {
                        readers.ForEach(x => x.Dispose());
                    }

                    break;
            }

            Log.Logger.Information($"Пользователь {user?.Username} ({user?.Id}), чат {chat.Id}: закончили обработку сообщения");
        }

        /// <summary>
        /// Возвращает, по сколько фоток в сообщении посылать (больше 10 не позволяет ТГ)
        /// </summary>
        /// <param name="count">Сколько всего фоток надо отправить</param>
        /// <returns></returns>
        private int GetPhotoSendProtion(int count)
        {
            int max = 10;

            if (count <= max)
                return count;

            // Например: count = 13, messages = 2, 13 / 2 = 7
            int messages = (int)float.Ceiling((float)count / (float)max); // сколько сообщений придется отправить
            return (int)float.Ceiling((float)count / (float)messages);    // поровну в сообщении
        }

        private IReplyMarkup GetStartKeyboard()
        {
            var buttonList = new List<KeyboardButton[]>()
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Найти места")
                }
            };

            var replyKeyboard = new ReplyKeyboardMarkup(buttonList)
            {
                ResizeKeyboard = true,
                IsPersistent = true
            };

            return replyKeyboard;
        }
    }
}
