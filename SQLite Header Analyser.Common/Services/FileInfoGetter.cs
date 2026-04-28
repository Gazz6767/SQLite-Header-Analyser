using System;
using System.Collections.Generic;
using System.Text;

namespace SQLite_Header_Analyser.Common.Services
{
    public class FileInfoGetter
    {
        public long GetFileSize(string filePath)
        {
            try
            {
                FileInfo fileInfo = new System.IO.FileInfo(filePath);
                long sizeInBytes = fileInfo.Length;
                return sizeInBytes;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
