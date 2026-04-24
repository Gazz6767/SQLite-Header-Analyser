using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SQLite_Header_Analyser.Common.Models;
using SQLite_Header_Analyser.Services.SQLiteCarving;
using SQLite_Header_Analyser.WPF.Services;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SQLite_Header_Analyser.WPF.ViewModels
{
    public partial class DBHeaderVM : ObservableObject
    {
        [RelayCommand]
        internal void OpenDB()
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

            WeakReferenceMessenger.Default.Send(HeaderStructure.RawData);
        }

        [ObservableProperty]
        SQLiteHeaderModel headerStructure = new();

        [ObservableProperty]
        Visibility additionalStatsVisibility = Visibility.Collapsed;

        [ObservableProperty]
        bool isCheckChecked = false;

        //[ObservableProperty]
        //string? signatureFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.SignatureReturn;

        //[ObservableProperty]
        //int? pageSizeFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.PageSizeReturn;

        //[ObservableProperty]
        //string? writeVerFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.WriteVerReturn;

        //[ObservableProperty]
        //string? readVerFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.ReadVerReturn;

        //[ObservableProperty]
        //string? reservedBytesFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.ReservedBytesReturn;

        //[ObservableProperty]
        //string? maxEmbedPayload = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.MaxEmbedPayloadFract;

        //[ObservableProperty]
        //string? minEmbedPayload = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.MinEmbedPayloadFract;

        //[ObservableProperty]
        //string? leafPayload = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.LeafPayloadFract;

        //[ObservableProperty]
        //string? fileChangeFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.FileChangeReturn;

        //[ObservableProperty]
        //string? pageCountFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.PageCountReturn;

        //[ObservableProperty]
        //string? trunkRootFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.TrunkRootPageReturn;

        //[ObservableProperty]
        //string? totalFreelistPages = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.TotalNoFreelistPages;

        //[ObservableProperty]
        //string? schemaCookie = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.SchemaCookie;

        //[ObservableProperty]
        //string? schemaFormatNo = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.SchemaFormatNo;

        //[ObservableProperty]
        //string? defaultPageSizeCache = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.DefaultPageCacheSize;

        //[ObservableProperty]
        //string? largestBtreePage = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.LargestRootBTreePage;

        //[ObservableProperty]
        //string? encodingFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.EncodingValueReturn;

        //[ObservableProperty]
        //string? userVersion = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.UserVersion;

        //[ObservableProperty]
        //string? vacuumFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.VacuumReturn;

        //[ObservableProperty]
        //string? appID = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.AppID;

        //[ObservableProperty]
        //string? expansionReserved = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.ExpansionReserved;

        //[ObservableProperty]
        //string? versionValidFor = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.VersionValidFor;

        //[ObservableProperty]
        //string? versionFound = GlobalConfigsStatic.SQliteCarvedDataPack?.SQLiteHeader?.VersionNoReturn;

        partial void OnIsCheckCheckedChanged(bool value)
        {
            if (value)
            {
                AdditionalStatsVisibility = Visibility.Visible;
            }
            else
            {
                AdditionalStatsVisibility = Visibility.Collapsed;
            }
        }
    }
}
