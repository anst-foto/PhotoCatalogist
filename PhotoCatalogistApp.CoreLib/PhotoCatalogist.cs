using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
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

    /// <summary>
    /// Метод для группировки файлов по годам и месяцам
    /// </summary>
    /// <param name="source">Директория с файлами</param>
    /// <returns>Словарь с группированными файлами по годам и месяцам</returns>
    public static Dictionary<int, Dictionary<int, List<FileInfo>>> GetGroupFilesByYearByMonth(DirectoryInfo source)
    {
        var files = source.EnumerateFiles();
        var grouped = files
            .GroupBy(file =>
                DateTime.ParseExact(file.Name, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture).Year)
            .OrderBy(yearGroup => yearGroup.Key)
            .ToDictionary(
                yearGroup => yearGroup.Key,
                yearGroup => yearGroup
                    .GroupBy(file =>
                        DateTime.ParseExact(file.Name, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture).Month)
                    .OrderBy(monthGroup => monthGroup.Key)
                    .ToDictionary(
                        monthGroup => monthGroup.Key,
                        monthGroup => monthGroup.ToList()
                    )
            );
        return grouped;
    }

    /// <summary>
    /// Метод для копирования файлов в указанную директорию
    /// </summary>
    /// <param name="destination">Директория для копирования</param>
    /// <param name="files">Словарь с группированными файлами по годам и месяцам</param>
    /// <param name="progress">Прогресс</param>
    /// <param name="token">Токен отмены</param>
    public static async Task CopyFiles(DirectoryInfo destination, Dictionary<int, Dictionary<int, List<FileInfo>>> files, IProgress<int> progress,
        CancellationToken token = default)
    {
        var currentDirectory = new DirectoryInfo(destination.FullName);
        await Task.Run(() =>
        {
            foreach (var (year, months) in files)
            {
                if (currentDirectory.EnumerateDirectories(year.ToString()).Any())
                {
                    currentDirectory.CreateSubdirectory(year.ToString());
                }

                currentDirectory = new DirectoryInfo(Path.Combine(currentDirectory.FullName, year.ToString()));
                foreach (var (month, filesList) in months)
                {
                    if (currentDirectory.EnumerateDirectories(month.ToString()).Any())
                    {
                        currentDirectory.CreateSubdirectory(month.ToString());
                    }

                    currentDirectory = new DirectoryInfo(Path.Combine(currentDirectory.FullName, month.ToString()));
                    foreach (var file in filesList)
                    {
                        file.CopyTo(Path.Combine(currentDirectory.FullName, file.Name));
                    }
                }
            }
        }, token);
    }
}
