using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InstantMessenger.Common;
using InstantMessenger.Common.TransportObject;

namespace InstantMessenger.Client.Base
{
    public class WindowBase : Window
    {
        #region Attributes

        public ModelBase Model { get; set; }

        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand RequestCommand { get; private set; }

        #endregion

        #region Constructor

        public WindowBase()
        {
            OKCommand = new RoutedUICommand("OK", "OK", typeof(WindowBase));
            CancelCommand = new RoutedUICommand("Cancel", "Cancel", typeof(WindowBase), new InputGestureCollection
            {
                new KeyGesture(Key.Escape),
            });
            RequestCommand = new RoutedUICommand("Request", "Request", typeof(WindowBase));

            CreateCommandBindings();           
            Client.Reconnecting += ClientOnReconnecting;
        }

        #endregion

        protected void Init(ModelBase model)
        {
            Model = model;
            DataContext = model;
            Model.ErrorReceived += ModelOnErrorReceived;
        }

        #region Commands methods

        private void CreateCommandBindings()
        {
            CommandBindings.Add(new CommandBinding(OKCommand, OKCommand_Executed, OKCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(CancelCommand, CancelCommand_Executed, CancelCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(RequestCommand, RequestCommand_Executed, RequestCommand_CanExecute));
        }

        #region OK

        /// <summary>
        /// Default OK button action. Calls <see cref="ModelBase.CreateRequest"/> on Model.
        /// When overriden, needs to call base implementation at the end.
        /// </summary>
        protected virtual void OKButtonAction()
        {
            Model.OKACtion();
        }

        protected virtual bool CanExecuteOK()
        {
            return true;
        }

        private void OKCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!Validate())
                return;

            OKButtonAction();
        }

        private void OKCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanExecuteOK();
        }

        #endregion

        #region Cancel

        /// <summary>
        /// Default Cancel button action. Closes the window.
        /// </summary>
        protected virtual void CancelButtonAction()
        {
            Close();
        }

        protected virtual bool CanExecuteCancel()
        {
            return true;
        }

        private void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CancelButtonAction();
        }

        private void CancelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanExecuteCancel();
        }

        #endregion

        #region Request
            
        private void RequestCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var action = e.Parameter as Action<TransportObject>;
            if (action == null)
            {
                throw new Exception("You must provide a method with TransportObject as a single parameter and no return value as command parameter.");
            }

            Model.CustomRequest(action);
        }

        private void RequestCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #endregion

        #region Validation

        /// <summary>
        /// Default controls validation.
        /// </summary>
        /// <returns></returns>
        protected virtual bool Validate()
        {
            return true;
        }

        #endregion

        #region Event handlers

        private void ModelOnErrorReceived(object sender, string s)
        {
            MessageBox.Show(s, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ClientOnReconnecting(object sender, int i)
        {
        }

        #endregion
    }
}
