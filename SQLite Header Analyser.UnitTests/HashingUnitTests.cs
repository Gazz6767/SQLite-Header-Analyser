using SQLite_Header_Analyser.Common.Services;
using System.Security.Cryptography;
using System.Text;

public class HashServiceTests : IDisposable
{
    private readonly string _testFilePath;
    
    public HashServiceTests()
    {
        _testFilePath = Path.GetTempFileName();
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
    }

    [Fact]
    public async Task ComputeFileHashAsync_ShouldReturnCorrectHash_WhenFileIsValid()
    {
        byte[] testData = Encoding.UTF8.GetBytes("ForensicDataTest");
        await File.WriteAllBytesAsync(_testFilePath, testData);

        string expectedHash;
        using (SHA256 sha256 = SHA256.Create())
        {
            expectedHash = Convert.ToHexString(sha256.ComputeHash(testData)).ToLower();
        }

        HashingService HS = new();
        string result = await HS.ComputeFileHashAsync(_testFilePath);
        Assert.Equal(expectedHash, result);
    }

    [Fact]
    public async Task ComputeFileHashAsync_ShouldThrowFileNotFound_WhenPathIsInvalid()
    {
        HashingService HS = new();
        await Assert.ThrowsAsync<FileNotFoundException>(() => HS.ComputeFileHashAsync("non_existent_file.db"));
    }

    [Fact]
    public async Task ComputeFileHashAsync_ShouldSucceed_WhenLockedFileBecomesAvailable()
    {
        await File.WriteAllTextAsync(_testFilePath, "RetriableData");

        // This is a complex test: simulate a lock by opening the file exclusively,
        // then delaying the release to allow the retry logic to trigger.
        using (FileStream fs = new FileStream(_testFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            // Start the hash task (it will hit the catch(IOException) and retry)
            HashingService HS = new();
            var hashTask = HS.ComputeFileHashAsync(_testFilePath);

            await Task.Delay(600); // Wait for first attempt to fail
            fs.Close(); // Release lock

            string result = await hashTask;
            Assert.NotNull(result);
        }
    }
}