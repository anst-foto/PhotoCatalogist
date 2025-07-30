using System.IO;

namespace PhotoCatalogistApp.CoreLib;

public static class ExtendedMethod
{
    /// <summary>
    /// Метод переименования файла
    /// </summary>
    /// <param name="fileInfo">Файл</param>
    /// <param name="newName">Новое имя</param>
    public static void Rename(this FileInfo fileInfo, string newName)
    {
        var path = $@"{fileInfo.Directory.FullName}\{newName}";
        fileInfo.MoveTo(destFileName: path, overwrite: true);
    }
}
