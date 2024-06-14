using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using NotePad.Views;
using System.Windows;

using ReactiveUI;
using System.Threading.Tasks;
using NotePad.Crypt;
using NotePad.Model;
using System.Reflection;
using NotePad.Helper;

namespace NotePad.ViewModels
{
	public class LoginFormViewModel : ViewModelBase
    {

        private const string UsersFilePath = "users.bin";
        private const string EncryptionKey = "myStrongPassword!123"; 



        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }

        public event Action LoginSuccessful;
        public event Action<string> LoginFailed;
        public LoginFormViewModel()
        {
            Logger.SaveInfo(LoggerLevel.INFO, "Приложение запущено.");
            Logger.SaveInfo(LoggerLevel.DEBUG, "Отладочное сообщение.");
            LoginCommand = ReactiveCommand.CreateFromTask(OnLoginAsync);
            EnsureUsersFileExists();

        }


        private void EnsureUsersFileExists()
        {
            if (!File.Exists(UsersFilePath))
            {
                using (File.Create(UsersFilePath)) { }
            }
        }

        private async Task OnLoginAsync()
        {
            try
            {
                var users = await ReadUsersFromFileAsync();
                var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

                if (user != null)
                {
                    Logger.SaveInfo(LoggerLevel.WARN, $"Успешный вход {user.Username}");
                    LoginSuccessful?.Invoke();
                }
                else
                {
                    Logger.SaveInfo(LoggerLevel.WARN, "Неверный логин или пароль");
                    LoginFailed?.Invoke("Неверный логин или пароль");

                }
            }
            catch (Exception ex)
            {
                LoginFailed?.Invoke($"Ошибка входа: {ex.Message}");
            }
        }


        private async Task<List<User>> ReadUsersFromFileAsync()
        {
            try
            {
                if (!File.Exists(UsersFilePath))
                {
                    return new List<User>();
                }

                await using var fileStream = new FileStream(UsersFilePath, FileMode.Open, FileAccess.Read);
                using var reader = new StreamReader(fileStream);
                var encryptedData = await reader.ReadToEndAsync();
                var decryptedData = CryptServices.Decrypt(encryptedData);

                if (string.IsNullOrWhiteSpace(decryptedData)) return new List<User>();

                var users = decryptedData
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line =>
                    {
                        var parts = line.Split('|');
                        return new User { Username = parts[0], Password = parts[1] };
                    })
                    .ToList();

                return users;
            }
            catch (Exception ex)
            {
                Logger.SaveInfo(LoggerLevel.WARN, $"Error reading users from file: {ex.Message}");
                return new List<User>();
            }
        }



    }
}