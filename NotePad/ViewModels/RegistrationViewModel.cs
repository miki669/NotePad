using NotePad.Model;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using NotePad.Crypt;
using System.Linq;
using NotePad.Helper;

namespace NotePad.ViewModels;

public class RegistrationViewModel : ViewModelBase
{
    private const string UsersFilePath = "users.bin";
    private const string EncryptionKey = "+nksGiPcFxwvGFcirC/UPTH2M19ACbzA5zeHj8sXlPdl5GgzKcE6SuwiDei5JtX2"; 

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

    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

    public event Action RegistrationSuccessful;
    public event Action<string> RegistrationFailed;

    public RegistrationViewModel()
    {
        
        RegisterCommand = ReactiveCommand.CreateFromTask(OnRegisterAsync);
    }

    private async Task OnRegisterAsync()
    {
        try
        {
            var users = await ReadUsersFromFileAsync();
            var userExists = users.Any(u => u.Username == Username);

            if (userExists)
            {
                Logger.SaveInfo(LoggerLevel.WARN, $"Пользователь с таким логином `{Username}` уже существует");

                RegistrationFailed?.Invoke("Пользователь с таким логином уже существует");
            }
            else
            {
                users.Add(new User { Username = Username, Password = Password });
                await WriteUsersToFileAsync(users);
                RegistrationSuccessful?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Logger.SaveInfo(LoggerLevel.WARN, $"Ошибка регистрации: {ex.Message}");
            RegistrationFailed?.Invoke($"Ошибка регистрации: {ex.Message}");
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

            var users = new List<User>();
            if (string.IsNullOrWhiteSpace(decryptedData)) return users;
            var lines = decryptedData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            users.AddRange(from line in lines select line.Split('|') into parts where parts.Length == 2 select new User { Username = parts[0], Password = parts[1] });
            return users;
        }
        catch
        {
            return new List<User>();
        }
    }


    private async Task WriteUsersToFileAsync(List<User> users)
    {
        try
        {
            await using var fileStream = new FileStream(UsersFilePath, FileMode.Create, FileAccess.Write);
            await using var writer = new StreamWriter(fileStream);
            var data = new StringBuilder();
            foreach (var user in users)
            {
                data.AppendLine($"{user.Username}|{user.Password}");
            }

            var encryptedData = CryptServices.Encrypt(data.ToString());
            await writer.WriteAsync(encryptedData);
            Logger.SaveInfo(LoggerLevel.INFO, $"Пользователь успешно добавлен");

        }
        catch (Exception ex)
        {
            Logger.SaveInfo(LoggerLevel.WARN, $"Error writing users to file: {ex.Message}");
        }
    }



}