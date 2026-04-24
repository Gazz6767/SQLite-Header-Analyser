using System;
using System.Collections.Generic;
using System.Text;

namespace SQLite_Header_Analyser.Common.Models
{
    public class SQLiteHeaderModel
    {
        public byte[] RawData { get; set; } = new byte[100];
        public string SignatureReturn { get; set; } = String.Empty;
        public int PageSizeReturn { get; set; }
        public string WriteVerReturn { get; set; } = String.Empty;
        public string ReadVerReturn { get; set; } = String.Empty;
        public string ReservedBytesReturn { get; set; } = String.Empty;
        public string MaxEmbedPayloadFract { get; set; } = String.Empty;
        public string MinEmbedPayloadFract { get; set; } = String.Empty;
        public string LeafPayloadFract { get; set; } = String.Empty;
        public string FileChangeReturn { get; set; } = String.Empty;
        public string PageCountReturn { get; set; } = String.Empty;
        public string TrunkRootPageReturn { get; set; } = String.Empty;
        public string TotalNoFreelistPages { get; set; } = String.Empty;
        public string SchemaCookie { get; set; } = String.Empty;
        public string SchemaFormatNo { get; set; } = String.Empty;
        public string DefaultPageCacheSize { get; set; } = String.Empty;
        public string LargestRootBTreePage { get; set; } = String.Empty;
        public string EncodingValueReturn { get; set; } = String.Empty;
        public string UserVersion { get; set; } = String.Empty;
        public string VacuumReturn { get; set; } = String.Empty;
        public string AppID { get; set; } = String.Empty;
        public string ExpansionReserved { get; set; } = String.Empty;
        public string VersionValidFor { get; set; } = String.Empty;
        public string VersionNoReturn { get; set; } = String.Empty;
        public bool ErrorOccurred { get; set; } = false;
        public string ErrorMessage { get; set; } = String.Empty;
    }
}
