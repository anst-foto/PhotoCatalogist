using System;
using System.IO;


namespace PhotoCatalogistApp.CoreLib.Test;

public class PhotoCatalogist_Test
{
    [Fact]
    public void RenameFile_PositiveTest()
    {
        var file = new FileInfo("test.txt");
        file.Rename("test_new");

        Assert.True(file.Exists);
    }

    [Fact]
    public void GetExifDate_PositiveTest()
    {
        var file = new FileInfo("test.jpg");
        var actual = PhotoCatalogist.GetExifDate(file);

        var expected = new DateTime(
            year: 2020, month: 08, day: 19,
            hour: 13, minute: 58, second: 53);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetExifDate_NegativeTest()
    {
        var file = new FileInfo("bad_test.jpg");
        var actual = PhotoCatalogist.GetExifDate(file);

        Assert.Null(actual);
    }
}
