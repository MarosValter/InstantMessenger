using System;
using System.Collections.Specialized;
using System.Windows.Controls;
using InstantMessenger.Client.Base;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.TabItem
{
    /// <summary>
    /// Interaction logic for Tab.xaml
    /// </summary>
    public partial class Body : PanelBase
    {
        #region Attributes

        protected new ConversationModel Model { get { return (ConversationModel) base.Model; } }

        #endregion

        #region Constructor

        public Body(long conversationOid)
        {
            InitializeComponent();
            Init(new ConversationModel(conversationOid));

        }

        #endregion

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset < 1)
            {
                RequestCommand.Execute((Action<TransportObject>)Model.LoadNextMessages);
            }
        }
    }
}
