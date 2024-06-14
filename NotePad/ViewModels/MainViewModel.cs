using Avalonia.Controls;
using NotePad.Helper;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace NotePad.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    private string _text;
    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    private string _filePath;
    public string FilePath
    {
        get => _filePath;
        set => this.RaiseAndSetIfChanged(ref _filePath, value);
    }

    private string _windowTitle;
    public string WindowTitle
    {
        get => _windowTitle;
        set => this.RaiseAndSetIfChanged(ref _windowTitle, value);
    }

    public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateReportCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveFileCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveAsFileCommand { get; }

    public MainViewModel()
    {
        OpenFileCommand = ReactiveCommand.CreateFromTask(OpenFile);
        SaveFileCommand = ReactiveCommand.CreateFromTask(SaveFile);
        SaveAsFileCommand = ReactiveCommand.CreateFromTask(SaveAsFile);
        CreateReportCommand = ReactiveCommand.CreateFromTask(CreateReportAsync);
        WindowTitle = "Блокнот";
    }

    private async Task OpenFile()
    {
        var fileDialog = new OpenFileDialog();
        fileDialog.Title = "Select image file(s)...";
        fileDialog.AllowMultiple = false;
        var success = await fileDialog.ShowAsync(new Window());
        if (success != null)
        {
            Text = await File.ReadAllTextAsync(success.FirstOrDefault());
            FilePath = success.FirstOrDefault();
            WindowTitle = $"{Path.GetFileName(FilePath)} - Блокнот";
            Logger.SaveInfo(LoggerLevel.INFO, $"Открыт файл {Path.GetFileName(FilePath)}");
        }
    }

    private async Task SaveFile()
    {
        if (!string.IsNullOrEmpty(_text))
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                await File.WriteAllTextAsync(FilePath, Text);
            }
            else
            {
                await SaveAsFile();
                Logger.SaveInfo(LoggerLevel.INFO, $"Сохранен файл {Path.GetFileName(FilePath)}, по пути {FilePath}");
            }
        }
    }

    private async Task SaveAsFile()
    {
        var dialog = new SaveFileDialog
        {
            DefaultExtension = ".txt",
        };
        dialog.Filters.Add(new FileDialogFilter() { Name = "Text files", Extensions = { "txt" } });
        var result = await dialog.ShowAsync(new Window());
        if (result != null)
        {
            await File.WriteAllTextAsync(result, Text);
            FilePath = result;
            Logger.SaveInfo(LoggerLevel.INFO, $"Сохранен файл {Path.GetFileName(FilePath)}, по пути {FilePath}");
        }
    }

    private async Task CreateReportAsync()
    {

        try
        {
            // Проверяем, существует ли файл
            if (File.Exists("log.txt"))
            {
                // Читаем файл асинхронно
                Text = await File.ReadAllTextAsync("log.txt");

                // Выводим содержимое файла в Text (здесь можно заменить на нужный вывод)
                Console.WriteLine("Содержимое файла log.txt:");
            }
            else
            {
                Console.WriteLine("Файл log.txt не существует.");
            }
        }
        catch (Exception ex)
        {
            // Обработка ошибок чтения файла
            Console.Error.WriteLine($"Ошибка чтения файла: {ex.Message}");
        }
    }


}

