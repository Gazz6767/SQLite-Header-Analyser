using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLite_Header_Analyser.WPF.Services
{
    public static class DialogService
    {
        /// <summary>
        /// Opens a file dialog window. Folder title is the title of the dialog window
        /// </summary>
        /// <param name="FolderTitle"></param>
        /// <returns>Either string.Empty if false, or the file path if true</returns>
        public static string GetFilePath(string FolderTitle)
        {
            OpenFileDialog dlg = new()
            {
                Title = FolderTitle,
                Multiselect = false,
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
