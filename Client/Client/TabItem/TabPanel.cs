using System.Windows;
using InstantMessenger.Common.Flats;

namespace InstantMessenger.Client.TabItem
{
    public class TabPanel : System.Windows.Controls.TabItem
    {
        public TabPanel(ConversationFlat conversation)
        {
            var header = new CloseHeader(conversation.Name);
            var content = new Body(conversation.OID);

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
