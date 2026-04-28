using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace SQLite_Header_Analyser.WPF.ViewModels
{
    public partial class SQLiteHeaderCTRL : ObservableObject
    {
        public SQLiteHeaderCTRL() 
        {
            WeakReferenceMessenger.Default.Register<byte[]>(this, (recipient, message) =>
            {
                Data = null;
                Data = message;
            });
        }

        [ObservableProperty]
        byte[]? data = new byte[100];
    }
}
