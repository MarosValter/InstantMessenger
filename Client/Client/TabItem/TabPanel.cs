using System.Windows;
using InstantMessenger.Common.Flats;

namespace InstantMessenger.Client.TabItem
{
    public class TabPanel : System.Windows.Controls.TabItem
    {
        public TabPanel(UserFlat user)
        {
            var header = new CloseHeader(user.Username);
            var content = new Body();

            base.Header = header;
            base.Content = content;
            base.Padding = new Thickness(0);
            base.Margin = new Thickness(0);
            base.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            base.VerticalContentAlignment = VerticalAlignment.Stretch;
            base.ClipToBounds = true;        
        }
    }
}
