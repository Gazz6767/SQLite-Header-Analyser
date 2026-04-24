using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SQLite_Header_Analyser.WPF.Services;

namespace SQLite_Header_Analyser.WPF.ViewModels
{
    public partial class SQLiteHeaderCTRL : ObservableObject
    {
        public SQLiteHeaderCTRL() 
        {
            WeakReferenceMessenger.Default.Register<byte[]>(this, (recipient, message) =>
            {
                data = message;
            });
        }

        [ObservableProperty]
        byte[]? data = new byte[100];
    }
}
