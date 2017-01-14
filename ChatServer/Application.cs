using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ChatServer
{
    internal abstract class Application
    {
        private Boolean _Ready;

        protected ServiceHost _Host;

        protected Uri _Address;

        public virtual Uri Address
        {
            get
            {
                return _Address;
            }
        }

        public Application(IPAddress address, Int32 port)
        {
            _Ready = false;

            _Host = null;

            String uriStr = String.Empty;
            uriStr += Uri.UriSchemeNetTcp + Uri.SchemeDelimiter;
            uriStr += address.ToString() + ":" + port + "/";

            Uri.TryCreate(uriStr, UriKind.Absolute, out _Address);
        }

        public virtual void Open()
        {
            if (_Ready)
            {
                throw new InvalidOperationException("The application can't be re-initialised because it was initialised before.");
            }

            if (_Host == null)
            {
                throw new ArgumentNullException("Host property field was not initialized in ");
            }

            ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
            _Host.Description.Behaviors.Add(metadataBehavior);

            ServiceDebugBehavior debugBehavior = _Host.Description.Behaviors.Find<ServiceDebugBehavior>();
            debugBehavior.IncludeExceptionDetailInFaults = true;

            _Host.AddServiceEndpoint(
                typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexTcpBinding(),
                "mex");

            _Ready = !_Ready;
        }

        public bool IsReady()
        {
            return _Ready;
        }

        public void Run()
        {
            if (!_Ready)
            {
                throw new InvalidOperationException("The application can't be executed because was not initialised.");
            }

            try
            {
                _Host.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw new ApplicationException("Failed to start host service.", ex);
            }
        }

        public void Close()
        {
            if (!_Ready)
            {
                throw new InvalidOperationException("The application can't be terminated because was never initialised.");
            }

            _Ready = !_Ready;

            _Host.Close();
        }
    }
}
