using SQLite_Header_Analyser.Common.Models;
using System.IO;

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
            int BytesRead = 0;
            SQLiteHeaderModel SHDC = new();

            try
            {
                using FileStream fs = new(dbPath, FileMode.Open, FileAccess.Read);
                BytesRead = fs.Read(SHDC.RawData, 0, SHDC.RawData.Length); // SDHC.RawData is 100 bytes
                fs.Close();
            }
            catch (Exception ex)
            {
                SHDC.ErrorOccurred = true;
                SHDC.ErrorMessage = ex.Message;
                return SHDC;
            }

            if (BytesRead != SHDC.RawData.Length)
            {
                SHDC.ErrorOccurred = true;
                SHDC.ErrorMessage = "Could not read the header of the file given";
                return SHDC;
            }

            byte[] signature = SHDC.RawData.Skip(0).Take(16).ToArray();
            byte[] pageSize = SHDC.RawData.Skip(16).Take(2).ToArray();            
            byte writeVer = SHDC.RawData[18];
            byte readVer = SHDC.RawData[19];
            byte resBytes = SHDC.RawData[20];
            byte maxEmbedPayloadFract = SHDC.RawData[21];
            byte minEmbedPayloadFract = SHDC.RawData[22];
            byte leafPayloadFract = SHDC.RawData[23];
            byte[] FileChangeCount = SHDC.RawData.Skip(24).Take(4).ToArray();
            byte[] pageCounter = SHDC.RawData.Skip(28).Take(4).ToArray();
            byte[] Trunk = SHDC.RawData.Skip(32).Take(4).ToArray();
            byte[] TotalFreelistPages = SHDC.RawData.Skip(36).Take(4).ToArray();
            byte[] SchemaCookie = SHDC.RawData.Skip(40).Take(4).ToArray();
            byte[] SchemaFormatNo = SHDC.RawData.Skip(44).Take(4).ToArray();
            byte[] DefaultPageCacheSize = SHDC.RawData.Skip(48).Take(4).ToArray();
            byte[] LargestRootBTree = SHDC.RawData.Skip(52).Take(4).ToArray();
            byte[] Encoding = SHDC.RawData.Skip(56).Take(4).ToArray();
            byte[] UserVersion = SHDC.RawData.Skip(60).Take(4).ToArray();
            byte[] IncrementalMode = SHDC.RawData.Skip(64).Take(4).ToArray();
            byte[] AppID = SHDC.RawData.Skip(68).Take(4).ToArray();
            byte[] ReservedExpansion = SHDC.RawData.Skip(72).Take(20).ToArray();
            byte[] VerValidFor = SHDC.RawData.Skip(92).Take(4).ToArray();
            byte[] VersionNo = SHDC.RawData.Skip(96).Take(4).ToArray();

            if (BitConverter.IsLittleEndian is true)
            {
                Array.Reverse(pageSize);
                Array.Reverse(FileChangeCount);
                Array.Reverse(pageCounter);
                Array.Reverse(Trunk);
                Array.Reverse(TotalFreelistPages);
                Array.Reverse(SchemaCookie);
                Array.Reverse(SchemaFormatNo);
                Array.Reverse(DefaultPageCacheSize);
                Array.Reverse(LargestRootBTree);
                Array.Reverse(Encoding);
                Array.Reverse(UserVersion);
                Array.Reverse(IncrementalMode);
                Array.Reverse(AppID);
                Array.Reverse(ReservedExpansion);
                Array.Reverse(VerValidFor);
                Array.Reverse(VersionNo);
            }
                  
            string sig = System.Text.Encoding.UTF8.GetString(signature) ?? String.Empty;

            if (sig is null || !sig.Contains("SQLite")) // Check valid SQLite file
            {
                SHDC.ErrorOccurred = true;
                SHDC.ErrorMessage = "This is not a valid SQLite database";
                return SHDC;
            }


            SHDC.SignatureReturn = sig.Replace("\0", string.Empty) ?? "Not Available";


            Int16 pageSizeInt = BitConverter.ToInt16(pageSize, 0);

            if (pageSizeInt == 1)
            {
                SHDC.PageSizeReturn = 65536;
            }
            else
            {
                SHDC.PageSizeReturn = pageSizeInt;
            }

            int wrVer = writeVer;
            if (writeVer == 1)
            {
                SHDC.WriteVerReturn = "Rollback (-Journal)";
            }
            else if (wrVer == 2)
            {
                SHDC.WriteVerReturn = "Write-Ahead Log (-WAL)";
            }
            else
            {
                SHDC.WriteVerReturn = "Unknown Journaling Method";
            }

            int rdVer = readVer;
            if (rdVer == 1)
            {
                SHDC.ReadVerReturn = "Rollback (-Journal)";
            }
            else if (rdVer == 2)
            {
                SHDC.ReadVerReturn = "Write-Ahead Log (-WAL)";
            }
            else
            {
                SHDC.ErrorOccurred = true;
                SHDC.ErrorMessage = "Unknown Journaling Method";
                SHDC.ReadVerReturn = "Unknown Journaling Method";
            }


            SHDC.ReservedBytesReturn = resBytes.ToString() ?? "Not Available";
            SHDC.MaxEmbedPayloadFract = maxEmbedPayloadFract.ToString() ?? "Not Available";
            SHDC.MinEmbedPayloadFract = minEmbedPayloadFract.ToString() ?? "Not Available";
            SHDC.LeafPayloadFract = leafPayloadFract.ToString() ?? "Not Available";
            SHDC.FileChangeReturn = BitConverter.ToInt32(FileChangeCount, 0).ToString() + " changes" ?? "Not Available";
            SHDC.PageCountReturn = BitConverter.ToInt32(pageCounter, 0).ToString() + " pages" ?? "Not Available";


            int TrunkRootPageINT = BitConverter.ToInt32(Trunk, 0);
            if (TrunkRootPageINT == 0)
            {
                SHDC.TrunkRootPageReturn = "No Freelist pages present";
            }
            else
            {
                SHDC.TrunkRootPageReturn = "Page " + TrunkRootPageINT ?? "Not Available";
            }


            SHDC.TotalNoFreelistPages = BitConverter.ToInt32(TotalFreelistPages, 0).ToString() + " pages" ?? "Not Available";
            SHDC.SchemaCookie = BitConverter.ToInt32(SchemaCookie, 0).ToString() ?? "Not Available";
            SHDC.SchemaFormatNo = BitConverter.ToInt32(SchemaFormatNo, 0).ToString() ?? "Not Available";
            SHDC.DefaultPageCacheSize = BitConverter.ToInt32(DefaultPageCacheSize, 0).ToString() + " bytes" ?? "Not Available";


            int DetectAutoVac = BitConverter.ToInt32(LargestRootBTree, 0);
            if (DetectAutoVac == 0)
            {
                SHDC.VacuumReturn = "Auto-Vacuum was not detected";
            }
            else
            {
                byte[] autoVacType = SHDC.RawData.Skip(64).Take(4).ToArray();

                if (BitConverter.IsLittleEndian is true)
                {
                    Array.Reverse(autoVacType);
                }

                int ConvertIncFlag = BitConverter.ToInt32(autoVacType, 0);

                if (ConvertIncFlag == 0)
                {
                    SHDC.VacuumReturn = "Auto-vacuum is enabled in FULL mode";
                }
                else if (ConvertIncFlag == 1)
                {
                    SHDC.VacuumReturn = "Auto-vacuum is enabled in INCREMENTAL mode";
                }
                else
                {
                    SHDC.VacuumReturn = DetectAutoVac + $"Auto-vacuum is enabled, however, the mode is unidentifiable (value = {ConvertIncFlag}))" ?? "Not Available";
                }
            }


            int enc = BitConverter.ToInt32(Encoding, 0);

            if (enc == 1)
            {
                SHDC.EncodingValueReturn = "UTF-8";
            }
            else if (enc == 2)
            {
                SHDC.EncodingValueReturn = "UTF-16 (le)";
            }
            else if (enc == 3)
            {
                SHDC.EncodingValueReturn = "UTF-16 (be)";
            }
            else
            {
                SHDC.EncodingValueReturn = "Unknown Encoding Type";
            }

            SHDC.UserVersion = BitConverter.ToInt32(UserVersion, 0).ToString() ?? "Not Available";
            SHDC.AppID = BitConverter.ToInt32(AppID, 0).ToString() ?? "Not Available";
            SHDC.ExpansionReserved = BitConverter.ToInt32(ReservedExpansion, 0).ToString() ?? "Not Available";
            SHDC.VersionValidFor = BitConverter.ToInt32(VerValidFor, 0).ToString() ?? "Not Available";
            SHDC.VersionNoReturn = BitConverter.ToInt32(VersionNo, 0).ToString() ?? "Not Available";
            return SHDC;
        }
    }
}
