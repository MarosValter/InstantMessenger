using InstantMessenger.Client.Base;
using InstantMessenger.Common.DM;

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

        protected override bool Validate()
        {
            return _grid.SelectedIndex != -1;
        }
    }
}
