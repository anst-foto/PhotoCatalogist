using System.Threading;
using System.Threading.Tasks;

using Avalonia.Platform.Storage;


namespace PhotoCatalogistApp.Desktop.Services;

public interface IFilesService
{
    public Task<IStorageFolder?> OpenFolderAsync(CancellationToken token = default);
}
