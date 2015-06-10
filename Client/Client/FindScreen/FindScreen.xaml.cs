using InstantMessenger.Client.Base;

namespace InstantMessenger.Client.FindScreen
{
    /// <summary>
    /// Interaction logic for RequestScreen.xaml
    /// </summary>
    public partial class FindScreen : WindowBase
    {
        #region Attributes

        protected new FindModel Model { get { return (FindModel) base.Model; } }

        #endregion
        public FindScreen()
        {
            InitializeComponent();
            Init(new FindModel());
        }
    }
}
