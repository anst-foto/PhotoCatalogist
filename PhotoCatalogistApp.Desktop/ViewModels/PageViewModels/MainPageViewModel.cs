using System;
using System.IO;
using System.Threading;
using System.Reactive;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using PhotoCatalogistApp.CoreLib;
using PhotoCatalogistApp.Desktop.Services;


namespace PhotoCatalogistApp.Desktop.ViewModels;

public class MainPageViewModel : PageViewModelBase
{
    [Reactive] public string? DirectoryPath { get; set; }
    [Reactive] public int Progress { get; set; }
    [Reactive] public int MinProgress { get; set; } = 0;
    [Reactive] public int MaxProgress { get; set; } = 100;

    public ReactiveCommand<Unit, Unit> CommandSelectDirectory { get; }
    public ReactiveCommand<Unit, Unit> CommandRenameFiles { get; }

    public MainPageViewModel()
    {
        CommandSelectDirectory = ReactiveCommand.CreateFromTask(SelectDirectoryAsync);
        CommandRenameFiles = ReactiveCommand.CreateFromTask(RenameFilesAsync);
    }

    private async Task RenameFilesAsync(CancellationToken token = default)
    {
        var progress = new Progress<int>();
        progress.ProgressChanged += (sender, i) => Progress = i;

        var directory = new DirectoryInfo(DirectoryPath);
        try
        {
            await PhotoCatalogist.RenameFiles(directory, progress, token);
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }

    }

    private async Task SelectDirectoryAsync(CancellationToken token = default)
    {
        ErrorMessages?.Clear();
        try
        {
            var filesService = App.Current?.Services?.GetService<IFilesService>();
            if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

            var file = await filesService.OpenFolderAsync(token);
            if (file is null) return;

            DirectoryPath = file.Path.AbsolutePath;
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }
}
