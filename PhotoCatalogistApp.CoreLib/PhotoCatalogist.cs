using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using MetadataExtractor;


namespace PhotoCatalogistApp.CoreLib;

/// <summary>
/// PhotoCatalogist - класс для работы с фотографиями
/// </summary>
public static class PhotoCatalogist
{
    /// <summary>
    /// Метод для получения даты из файла
    /// </summary>
    /// <param name="file">Файл</param>
    /// <returns>Дата создания фотографии или null, если даты не существует</returns>
    public static DateTime? GetExifDate(FileInfo file)
    {
        var exif = ImageMetadataReader.ReadMetadata(file.FullName);
        foreach (var directory in exif)
            foreach (var tag in directory.Tags)
                if (tag.Name is "Date/Time Original")
                    return DateTime.ParseExact(tag.Description, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);

        return null;
    }

    /// <summary>
    /// Метод для переименования файлов
    /// </summary>
    /// <param name="directory">Директория</param>
    /// <param name="progress">Прогресс</param>
    /// <param name="token">Токен отмены</param>
    public static async Task RenameFiles(DirectoryInfo directory, IProgress<int> progress, CancellationToken token = default)
    {
        var files = directory.GetFiles();
        await Task.Run(() =>
        {
            for (var i = 0; i < files.Length; i++)
            {
                token.ThrowIfCancellationRequested();

                var file = files[i];
                var date = GetExifDate(file);
                var newName = $"{date:yyyy-MM-dd_HH-mm-ss}{file.Extension}";
                file.Rename(newName);

                progress.Report(i * 100 / files.Length);
            }
        }, token);
    }
}
