using System.Windows;
using System.Windows.Controls;

namespace InstantMessenger.Client.TabItem
{
    /// <summary>
    /// Interaction logic for CloseHeader.xaml
    /// </summary>
    public partial class CloseHeader : UserControl
    {
        public CloseHeader(string title)
        {           
            InitializeComponent();
            Title.Content = title;
        }

        public CloseHeader()
            :this("Title")
        { }

        private void Title_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BtnClose.Margin = new Thickness(Title.ActualWidth + 5, 3, 2, 1);
        }
    }
}
