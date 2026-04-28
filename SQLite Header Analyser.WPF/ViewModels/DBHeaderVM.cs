using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SQLite_Header_Analyser.Common.Models;
using SQLite_Header_Analyser.Common.Services;
using SQLite_Header_Analyser.Services.SQLiteCarving;
using SQLite_Header_Analyser.WPF.Services;
using System.Windows;

namespace SQLite_Header_Analyser.WPF.ViewModels
{
    public partial class DBHeaderVM : ObservableObject
    {
        [RelayCommand]
        internal async Task OpenDBAsync()
        {
            string path = DialogService.GetFilePath("Select an SQLite database file");

            if (string.IsNullOrWhiteSpace(path))
            {
                // error message box shown now
                return;
            }

            SQLiteHeaderParser SHP = new();
            HeaderStructure = SHP.HeaderParser(path);

            if (HeaderStructure.ErrorOccurred)
            {
                // Show the returned error message
                return;
            }

            HashingService HS = new();
            Sha256Hash = await HS.ComputeFileHashAsync(path);

            WeakReferenceMessenger.Default.Send(HeaderStructure.RawData);
            AdditionalStatsVisibility = Visibility.Visible;
            FileName = path;
        }

        [ObservableProperty]
        SQLiteHeaderModel headerStructure = new();

        [ObservableProperty]
        Visibility additionalStatsVisibility = Visibility.Collapsed;

        [ObservableProperty]
        string fileName = string.Empty;

        [ObservableProperty]
        string sha256Hash = string.Empty;
    }
}
