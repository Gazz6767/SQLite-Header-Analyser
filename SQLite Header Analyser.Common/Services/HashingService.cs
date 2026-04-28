using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SQLite_Header_Analyser.Common.Services
{
    public class HashingService
    {
        /// <summary>
        /// Calculates the SHA256 hash of a file with retry logic.
        /// </summary>
        /// <param name="filePath">The absolute path to the target file.</param>
        /// <param name="maxRetries">Number of attempts before failing.</param>
        /// <returns>A hexadecimal string representation of the hash.</returns>
        public async Task<string> ComputeFileHashAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            }

            int attempts = 0;
            int maxRetries = 3;

            while (true)
            {
                try
                {
                    using (SHA256 sha256 = SHA256.Create())
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                    {
                        byte[] hashBytes = await sha256.ComputeHashAsync(stream);

                        StringBuilder sb = new StringBuilder();
                        foreach (byte b in hashBytes)
                        {
                            sb.Append(b.ToString("x2"));
                        }
                        return sb.ToString();
                    }
                }
                catch (IOException ex) when (attempts < maxRetries)
                {
                    attempts++;
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Critical error hashing file {filePath}: {ex.Message}", ex);
                }
            }
        }
    }
}
