using System.Threading;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Platform.Storage;


namespace PhotoCatalogistApp.Desktop.Services;

public class FilesService : IFilesService
{
    private readonly Window _target;

    public FilesService(Window target)
    {
        _target = target;
    }

    public async Task<IStorageFolder?> OpenFolderAsync(CancellationToken token = default)
    {
        var folders = await _target.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Открыть директорию",
                AllowMultiple = false
            });

        return folders.Count >= 1 ? folders[0] : null;
    }
}
