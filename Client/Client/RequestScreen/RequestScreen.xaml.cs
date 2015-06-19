using System.Windows;
using System.Windows.Controls;
using InstantMessenger.Client.Base;
using InstantMessenger.Client.RegisterScreen;
using InstantMessenger.Common;

namespace InstantMessenger.Client.RequestScreen
{
    /// <summary>
    /// Interaction logic for FindScreen.xaml
    /// </summary>
    public partial class RequestScreen : WindowBase
    {
        protected new RequestModel Model { get { return (RequestModel)base.Model; } }
        public RequestScreen()
        {
            InitializeComponent();
            Init(new RequestModel());
        }
    }
}
