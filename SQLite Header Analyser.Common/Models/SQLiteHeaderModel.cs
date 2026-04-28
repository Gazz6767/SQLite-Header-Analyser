namespace SQLite_Header_Analyser.Common.Models
{
    public class SQLiteHeaderModel
    {
        public byte[] RawData { get; set; } = new byte[100];
        public string Signature { get; set; } = string.Empty;

        public uint PageSize { get; set; }
        public string PageSizeReturn => PageSize.ToString();

        public byte WriteVersion { get; set; }
        public string WriteVerReturn => WriteVersion == 2 ? "2 (Write-Ahead Log)" : "1 (Rollback Journal)";

        public byte ReadVersion { get; set; }
        public string ReadVerReturn => ReadVersion == 2 ? "2 (Write-Ahead Log)" : "1 (Rollback Journal)";

        public byte ReservedBytes { get; set; }
        public string ReservedBytesReturn => ReservedBytes.ToString();

        public byte MaxEmbedPayload { get; set; }
        public string MaxEmbedPayloadFract => MaxEmbedPayload.ToString();

        public byte MinEmbedPayload { get; set; }
        public string MinEmbedPayloadFract => MinEmbedPayload.ToString();

        public byte LeafPayload { get; set; }
        public string LeafPayloadFract => LeafPayload.ToString();

        public uint FileChangeCounter { get; set; }
        public string FileChangeReturn => FileChangeCounter.ToString();

        public uint DatabasePageCount { get; set; }
        public string PageCountReturn => DatabasePageCount.ToString();

        public uint TrunkRootPage { get; set; }
        public string TrunkRootPageReturn => TrunkRootPage.ToString();

        public long TrunkRootOffset => TrunkRootPage > 0 ? ((long)PageSize * TrunkRootPage) - PageSize : 0;

        public uint TotalFreelistPages { get; set; }
        public string TotalNoFreelistPages => TotalFreelistPages.ToString();

        public uint SchemaCookieValue { get; set; }
        public string SchemaCookie => SchemaCookieValue.ToString();

        public uint SchemaFormat { get; set; }
        public string SchemaFormatNo => SchemaFormat.ToString();

        public uint DefaultCacheSize { get; set; }
        public string DefaultPageCacheSize => DefaultCacheSize.ToString();

        public uint LargestRootBTree { get; set; }
        public string LargestRootBTreePage => LargestRootBTree.ToString();

        public uint IncrementalVacuumValue { get; set; } // Offset 64

        public string VacuumReturn => LargestRootBTree switch
        {
            0 => "0 (None / Disabled)",
            _ => IncrementalVacuumValue == 0
                ? $"{LargestRootBTree} (Full Auto-Vacuum)"
                : $"{LargestRootBTree} (Incremental Mode)"
        };

        public uint EncodingType { get; set; }
        public string EncodingValueReturn => EncodingType switch
        {
            1 => "1 (UTF-8)",
            2 => "2 (UTF-16le)",
            3 => "3 (UTF-16be)",
            _ => "Unknown"
        };

        public uint UserVersionValue { get; set; }
        public string UserVersion => UserVersionValue.ToString();

        public uint ApplicationIDValue { get; set; }
        public string AppID => $"0x{ApplicationIDValue:X8} ({ApplicationIDValue})";

        public uint VersionValidForValue { get; set; }
        public string VersionValidFor => VersionValidForValue.ToString();

        public uint SqliteVersionNumber { get; set; }
        public string VersionNoReturn => SqliteVersionNumber.ToString();

        public long ActualFileSize { get; set; }

        public string VerificationResult
        {
            get
            {
                long expectedSize = (long)PageSize * DatabasePageCount;

                if (expectedSize == ActualFileSize)
                {
                    return "The file size matches the expected size based on the header information.";
                }

                long difference = Math.Abs(ActualFileSize - expectedSize);
                string direction = ActualFileSize > expectedSize ? "Appended/Extra" : "Missing/Truncated";

                return $"The file size does NOT match the expected size. Expected: {expectedSize} bytes, Actual: {ActualFileSize} bytes.";
            }
        }

        public string FreelistOffset
        {
            get
            {
                if (TrunkRootPage == 0)
                {
                    return "No freelist pages";
                }
                else
                {
                    long offset = (TrunkRootOffset * PageSize) - PageSize;
                    return $"{offset} (decimal)  |  0x{offset:X} (hexadecimal)";
                }                
            }
        }

        // --- Error Handling ---
        public bool ErrorOccurred { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
