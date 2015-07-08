using System;
using System.Windows;
using System.Windows.Input;
using InstantMessenger.Common;
using InstantMessenger.Communication;

namespace InstantMessenger.Client.Base
{
    public class WindowBase : Window//, IDisposable
    {
        #region Attributes

        //private bool _disposed ;
        private LoadingScreen _loadingScreen;

        public ModelBase<IDataManager> Model { get; set; }

        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand RequestCommand { get; private set; }

        #endregion

        #region Constructor

        public WindowBase(bool acceptEnter = true)
        {
            var okInputs = new InputGestureCollection();
            if (acceptEnter)
            {
                okInputs.Add(new KeyGesture(Key.Enter));
            }
            OKCommand = new RoutedUICommand("OK", "OK", typeof(WindowBase), okInputs);
            CancelCommand = new RoutedUICommand("Cancel", "Cancel", typeof(WindowBase), new InputGestureCollection
            {
                new KeyGesture(Key.Escape),
            });
            RequestCommand = new RoutedUICommand("Request", "Request", typeof(WindowBase));

            CreateCommandBindings();           
            Client.Reconnecting += ClientOnReconnecting;
            Client.Reconnected += ClientOnReconnected;
            Client.Connected += ClientOnConnected;
            Client.Disconnected += ClientOnDisconnected;

            Closed += OnClosed;
        }

        protected virtual void OnClosed(object sender, EventArgs eventArgs)
        {
            Client.Reconnecting -= ClientOnReconnecting;
            Client.Reconnected -= ClientOnReconnected;
            Client.Connected -= ClientOnConnected;
            Client.Disconnected -= ClientOnDisconnected;
        }

        #endregion

        protected void Init(ModelBase<IDataManager> model)
        {
            Model = model;
            DataContext = model;
            Model.ErrorReceived += ModelOnErrorReceived;
        }

        #region Public methods

        public void RefreshData()
        {
            Model.GetInitData();
        }

        #endregion

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
            Model.OKAction();
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

        private void ClientOnReconnecting(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (IsActive && IsEnabled)
                {
                    IsEnabled = false;
                    _loadingScreen = new LoadingScreen(this, "Reconnecting..");
                    _loadingScreen.Show();
                }
            }));          
        }

        private void ClientOnReconnected(object sender, EventArgs eventArgs)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                IsEnabled = true;
                if (_loadingScreen != null)
                {
                    _loadingScreen.Close();
                    _loadingScreen = null;
                }
            }));

            Model.GetInitData();
        }

        private void ClientOnDisconnected(object sender, EventArgs eventArgs)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                IsEnabled = true;
                if (_loadingScreen != null)
                {
                    _loadingScreen.Close();
                    _loadingScreen = null;
                }
            }));
        }

        private void ClientOnConnected(object sender, EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        #endregion

        //#region IDisposable

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_disposed)
        //    {
        //        if (disposing)
        //        {
        //            //dispose managed resources
        //            if (_loadingScreen != null)
        //                _loadingScreen.Close();
        //        }
        //    }
        //    //dispose unmanaged resources
        //    _disposed = true;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //#endregion
    }
}
