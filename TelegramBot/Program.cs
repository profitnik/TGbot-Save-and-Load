using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;


namespace TelegramBot
{
    
    class Program
    {
        static string path_root = Environment.CurrentDirectory + "\\Save\\";
        static TelegramBotClient bot;
        static void Main(string[] args)
        {
            // Here you need a token
            string token = "989083912:AAGuZ7vtJOeBL0UjdX13EnEp**********";
            
            

            DirectoryInfo save = new(Environment.CurrentDirectory);
            bool direct = false;
            foreach (var item in save.GetDirectories())
            {
                if(item.Name == "Save")
                {
                    direct = true;
                    break;
                }
                
            }

            if(direct == false) Directory.CreateDirectory(path_root); 

            bot = new TelegramBotClient(token);

            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            Console.WriteLine();
            string text = $"{DateTime.Now.ToShortTimeString()} {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Type.ToString()} {e.Message.Text} ";
            Console.WriteLine(text);

            switch(e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Файл загружен на компьютер");
                    Console.WriteLine($"{e.Message.Document.FileName} Файл загружен на компьютер");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    GiveFile(e.Message.Text, e.Message.Chat.Id);
                    break;
                default: bot.SendTextMessageAsync(e.Message.Chat.Id, "Загрузите документ или запросите текстом название документа");
                    break;
            }

            

        }

        static async void DownLoad(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);

            string fileName = $"{path_root}{path}";
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            FileStream fs = new FileStream(fileName, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }
        static async void GiveFile(string name, long chatId)
        {
            if (name.Length >= 3)
            {
                string path = path_root; //@"C:\Users\vgbyf\Desktop\C#\Практика\TelegramBot\TelegramBot\bin\Debug\net5.0\Save";

                DirectoryInfo dir = new(path_root);
                bool find = false;
                foreach (var item in dir.GetFiles())
                {
                    string nameFile = item.Name;
                    Regex regex = new Regex($@"\w*{name}\w*", RegexOptions.IgnoreCase);

                    if (regex.IsMatch(nameFile) == true)
                    {
                        //Console.WriteLine("Файл найден");
                        path = path + item.Name;
                        using FileStream fs = File.OpenRead(path);
                        InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, item.Name);
                        Thread.Sleep(100);
                        await bot.SendDocumentAsync(chatId, inputOnlineFile);
                        Console.WriteLine($"{item.Name} Файл загружен в ответ");
                        fs.Close();
                        fs.Dispose();
                        find = true;
                    }
                    

                }

                if (find == false)
                {
                    bot.SendTextMessageAsync(chatId, "Файл не найден");
                }
                else find = false;
            }
            else bot.SendTextMessageAsync(chatId, "Название должны быть не менее 5 символов");



        }
    }
}
