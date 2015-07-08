using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InstantMessenger.Common;
using InstantMessenger.Communication;

namespace InstantMessenger.Client.Base
{
    public class PanelBase : UserControl//, IDisposable
    {
        #region Attributes

        //private bool _disposed;
        private LoadingScreen _loadingScreen;

        public ModelBase<IDataManager> Model { get; set; }

        public ICommand OKCommand { get; private set; }
        public ICommand RequestCommand { get; private set; }

        #endregion

        #region Constructor

        public PanelBase()
        {
            OKCommand = new RoutedUICommand("OK", "OK", typeof(PanelBase));
            RequestCommand = new RoutedUICommand("Request", "Request", typeof(PanelBase));

            CreateCommandBindings();           
            Client.Reconnecting += ClientOnReconnecting;
            Client.Reconnected += ClientOnReconnected;
            Client.Connected += ClientOnConnected;
            Client.Disconnected += ClientOnDisconnected;
            
            Unloaded += OnUnloaded;
        }

        protected virtual void OnUnloaded(object sender, EventArgs eventArgs)
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
                if (IsEnabled)
                {
                    IsEnabled = false;
                    _loadingScreen = new LoadingScreen(Application.Current.MainWindow, "Reconnecting..");
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
        //            _loadingScreen.Dispose();
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
