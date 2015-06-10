using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InstantMessenger.Client.Base;
using InstantMessenger.Client.TabItem;
using InstantMessenger.Common;
using InstantMessenger.Common.Flats;

namespace InstantMessenger.Client.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        #region Attributes
        protected new MainModel Model { get { return (MainModel) base.Model; } }
        private readonly IDictionary<UserFlat, TabPanel> _openTabs;

        #endregion

        #region Constructor

        public MainWindow()
        {
            _openTabs = new Dictionary<UserFlat, TabPanel>();
            InitializeComponent();
            Init(new MainModel());
        }

        #endregion

        public new bool? ShowDialog()
        {
            Model.GetInitData();
            return base.ShowDialog();
        }

        #region Event handlers

        private void _btnFriends_Click(object sender, RoutedEventArgs e)
        {
            var sc = new FindScreen.FindScreen();
            sc.ShowDialog();
        }

        private void _btnRequests_Click(object sender, RoutedEventArgs e)
        {
            var sc = new RequestScreen.RequestScreen();
            sc.ShowDialog();
        }

        private void OnlineFriends_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var user = (sender as ListBox).SelectedItem as UserFlat;
            if (user == null)
            {
                return;
            }

            if (ActiveConversation(user))
            {
                _openTabs[user].Focus();
                return;
            }

            var tab = new TabPanel(user);
            _openTabs[user] = tab;
            Conversations.Items.Add(tab);
            tab.Focus();

        }

        #endregion

        #region Other methods

        private bool ActiveConversation(UserFlat user)
        {
            return _openTabs.ContainsKey(user);
        }

        #endregion
    }
}
