using SQLite_Header_Analyser.Services.SQLiteCarving;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Xunit;

public class SQLiteHeaderTests : IDisposable
{
    private readonly string _testDbPath;

    public SQLiteHeaderTests()
    {
        // Creates a unique temp file path for each test run
        _testDbPath = Path.GetTempFileName();
    }

    // This runs after every test to keep your hard drive clean
    public void Dispose()
    {
        if (File.Exists(_testDbPath)) File.Delete(_testDbPath);
    }

    private byte[] CreateValidHeader()
    {
        byte[] header = new byte[100];
        // 1. Set Signature
        Encoding.UTF8.GetBytes("SQLite format 3\0").CopyTo(header, 0);

        // 2. Set default values (using Big Endian as SQLite expects)
        BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan(16, 2), 4096); // Page Size
        header[18] = 1; // Write version
        header[19] = 1; // Read version
        BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(28, 4), 527); // Page Count
        BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(68, 4), 0x12345678); // App ID

        return header;
    }

    [Fact]
    public void HeaderParser_ShouldParseCorrectly_WhenFileIsValid()
    {
        // Arrange
        SQLiteHeaderParser SHP = new();
        File.WriteAllBytes(_testDbPath, CreateValidHeader());

        // Act
        var result = SHP.HeaderParser(_testDbPath);

        // Assert
        Assert.False(result.ErrorOccurred);
        Assert.Equal("SQLite format 3", result.Signature);
        Assert.Equal(4096u, result.PageSize);
        Assert.Equal(527u, result.DatabasePageCount);
        Assert.Equal(0x12345678u, result.ApplicationIDValue);
        Assert.Equal(100, result.ActualFileSize);
    }

    [Fact]
    public void HeaderParser_ShouldHandleLargePageSize_WhenValueIsOne()
    {
        // Arrange
        SQLiteHeaderParser SHP = new();
        var header = CreateValidHeader();
        // Set page size bytes to 0x0001 (which represents 65536 in SQLite)
        BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan(16, 2), 1);
        File.WriteAllBytes(_testDbPath, header);

        // Act
        var result = SHP.HeaderParser(_testDbPath);

        // Assert
        Assert.Equal(65536u, result.PageSize);
    }

    [Fact]
    public void HeaderParser_ShouldReturnError_WhenFileIsTooSmall()
    {

        // Arrange
        SQLiteHeaderParser SHP = new();
        File.WriteAllBytes(_testDbPath, new byte[50]); // Only 50 bytes

        // Act
        var result = SHP.HeaderParser(_testDbPath);

        // Assert
        Assert.True(result.ErrorOccurred);
        Assert.Contains("too small", result.ErrorMessage);
    }

    [Fact]
    public void HeaderParser_ShouldReturnError_WhenSignatureIsInvalid()
    {
        // Arrange
        SQLiteHeaderParser SHP = new();
        var header = new byte[100];
        Encoding.UTF8.GetBytes("BAD SIGNATURE!!!").CopyTo(header, 0);
        File.WriteAllBytes(_testDbPath, header);

        // Act
        var result = SHP.HeaderParser(_testDbPath);

        // Assert
        Assert.True(result.ErrorOccurred);
        Assert.Contains("Invalid Signature", result.ErrorMessage);
    }
}