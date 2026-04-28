using SQLite_Header_Analyser.Common.Models;
using SQLite_Header_Analyser.Common.Services;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace SQLite_Header_Analyser.Services.SQLiteCarving
{
    public class SQLiteHeaderParser
    {
        /// <summary>
        /// Takes a filename to an SQLite DB and reads the header (100 bytes from 0x00)
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>SQLiteHeaderModel containing results. Note: The bool 'ErrorOccurred' is a check you should use when the results are returned prior to continuing</returns>
        public SQLiteHeaderModel HeaderParser(string dbPath)
        {
            SQLiteHeaderModel SHDC = new();

            try
            {
                using FileStream fs = new(dbPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                int bytesRead = fs.Read(SHDC.RawData, 0, 100);

                if (bytesRead < 100)
                {
                    SHDC.ErrorOccurred = true;
                    SHDC.ErrorMessage = "File is too small to contain a valid SQLite header.";
                    return SHDC;
                }
            }
            catch (Exception ex)
            {
                SHDC.ErrorOccurred = true;
                SHDC.ErrorMessage = $"IO Error: {ex.Message}";
                return SHDC;
            }

            ReadOnlySpan<byte> headerSpan = SHDC.RawData;

            string sig = Encoding.UTF8.GetString(headerSpan.Slice(0, 16)).Replace("\0", string.Empty);
            if (!sig.Contains("SQLite format 3"))
            {
                SHDC.ErrorOccurred = true;
                SHDC.ErrorMessage = "Invalid Signature: Not a valid SQLite 3 database.";
                return SHDC;
            }
            SHDC.Signature = sig;

            ushort rawPageSize = BinaryPrimitives.ReadUInt16BigEndian(headerSpan.Slice(16, 2));
            SHDC.PageSize = (rawPageSize == 1) ? 65536u : rawPageSize;
            SHDC.WriteVersion = headerSpan[18];
            SHDC.ReadVersion = headerSpan[19];
            SHDC.ReservedBytes = headerSpan[20];
            SHDC.MaxEmbedPayload = headerSpan[21];
            SHDC.MinEmbedPayload = headerSpan[22];
            SHDC.LeafPayload = headerSpan[23];
            SHDC.FileChangeCounter = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(24, 4));
            SHDC.DatabasePageCount = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(28, 4));
            SHDC.TrunkRootPage = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(32, 4));
            SHDC.TotalFreelistPages = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(36, 4));
            SHDC.SchemaCookieValue = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(40, 4));
            SHDC.SchemaFormat = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(44, 4));
            SHDC.DefaultCacheSize = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(48, 4));
            SHDC.LargestRootBTree = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(52, 4));
            SHDC.IncrementalVacuumValue = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(64, 4));
            SHDC.EncodingType = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(56, 4));
            SHDC.UserVersionValue = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(60, 4));
            SHDC.ApplicationIDValue = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(68, 4));
            SHDC.VersionValidForValue = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(92, 4));
            SHDC.SqliteVersionNumber = BinaryPrimitives.ReadUInt32BigEndian(headerSpan.Slice(96, 4));

            FileInfoGetter FIG = new();
            SHDC.ActualFileSize = FIG.GetFileSize(dbPath);
            return SHDC;
        }
    }
}
