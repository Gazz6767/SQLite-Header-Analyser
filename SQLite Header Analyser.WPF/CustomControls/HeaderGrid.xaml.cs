using System.Windows.Controls;

namespace SQLite_Header_Analyser.WPF.CustomControls
{
    /// <summary>
    /// Interaction logic for Header.xaml
    /// </summary>
    public partial class HeaderGrid : UserControl
    {
        public HeaderGrid() 
        {         
            InitializeComponent();
            DataContext = new ViewModels.SQLiteHeaderCTRL();
        }
    }
}
