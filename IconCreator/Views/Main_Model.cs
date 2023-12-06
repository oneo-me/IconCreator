using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using IconCreator.Encoders;
using IconCreator.Services;
using OpenView;
using OpenView.Commands;
using OpenView.Models;
using OpenView.Services;
using SixLabors.ImageSharp.PixelFormats;

namespace IconCreator.Views;

public class Main_Model : WindowModel<Main>
{
    readonly ConfigService _configService = Service.Get<ConfigService>();

    public Main_Model()
    {
        using var iconStream = AssetLoader.Open(new("avares://IconCreator/Assets/Icon.ico"));

        Icon = new(iconStream);
        Title = nameof(IconCreator);
        Width = 420;
        Height = 420;
        CanResize = false;

        Menu.Add(new NativeMenuItem("File")
        {
            Menu = [new NativeMenuItem("Import Image") { Command = ImportFileAsyncCommand }, new NativeMenuItemSeparator(), new NativeMenuItem("Export Icon") { Command = ExportIconAsyncCommand }, new NativeMenuItem("Export Icns") { Command = ExportIcnsAsyncCommand }]
        });
    }

    Bitmap? _image;

    public Bitmap? Image
    {
        get => _image;
        set => SetValue(ref _image, value);
    }

    string _path = string.Empty;

    public string Path
    {
        get => _path;
        set
        {
            if (SetValue(ref _path, value))
                OnPathChanged(value);
        }
    }

    protected override void OnInitialized(Main view)
    {
        base.OnInitialized(view);
        Path = _configService.Config.Path;
    }

    void OnPathChanged(string file)
    {
        if (File.Exists(file) is false)
            return;

        Image?.Dispose();
        Image = new(file);

        _configService.Config.Path = file;
    }

    public AsyncCommand ImportFileAsyncCommand => GetCommand(async () =>
    {
        var topLevel = AppView.GetTopLevel();
        var storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(new()
        {
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Image Files") { Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp"] }
            }
        }).ConfigureAwait(false);
        if (storageFiles.Count <= 0)
            return;

        Path = storageFiles[0].Path.LocalPath;
    });

    public AsyncCommand ExportIconAsyncCommand => GetCommand(async () =>
    {
        var topLevel = AppView.GetTopLevel();
        var storageFile = await topLevel.StorageProvider.SaveFilePickerAsync(new()
        {
            SuggestedFileName = "Icon.ico",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Icon File") { Patterns = ["*.ico"] }
            }
        }).ConfigureAwait(false);

        if (storageFile is null)
            return;

        var sourceFile = storageFile.Path.LocalPath;
        using var source = SixLabors.ImageSharp.Image.Load<Rgba32>(Path);
        await using var fileStream = File.Create(sourceFile);
        await source.SaveAsync(fileStream, new IconEncoder()).ConfigureAwait(false);
    });

    public AsyncCommand ExportIcnsAsyncCommand => GetCommand(async () =>
    {
        var topLevel = AppView.GetTopLevel();
        var storageFile = await topLevel.StorageProvider.SaveFilePickerAsync(new()
        {
            SuggestedFileName = "applet.ico",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Icns File") { Patterns = ["*.icns"] }
            }
        }).ConfigureAwait(false);

        if (storageFile is null)
            return;

        var sourceFile = storageFile.Path.LocalPath;
        using var source = SixLabors.ImageSharp.Image.Load<Rgba32>(Path);
        await using var fileStream = File.Create(sourceFile);
        await source.SaveAsync(fileStream, new IconEncoder()).ConfigureAwait(false);
    });
}
