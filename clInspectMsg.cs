using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DemoBehavior
{
	class DemoMsgInspector : IClientMessageInspector
	{
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			Utils.LogToFile("AfterReceiveReply" + reply.ToString());
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			Utils.LogToFile("BeforeSendRequest " + request.ToString());
			return null;    //throw new NotImplementedException();
		}
	}

	// Endpoint behavior
	public class DemoLogger : IEndpointBehavior, IDispatchMessageInspector
	{
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			//string messageFileName = Path.Combine(this.messageLogFolder, string.Format("Log{0:000}_Incoming.txt", Interlocked.Increment(ref messageLogFileIndex)));
			Utils.LogToFile("AfterReceiveRequest. " + request.ToString());
			return null;
		}
		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			//string messageFileName = Path.Combine(this.messageLogFolder, string.Format("Log{0:000}_Outgoing.txt", Interlocked.Increment(ref messageLogFileIndex)));
			Utils.LogToFile("BeforeSendReply. " + reply.ToString());
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
			// No implementation necessary
			//Utils.LogToFile("AddBindingParameters. ");
		}
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			//Utils.LogToFile("ApplyClientBehavior. ");
			clientRuntime.MessageInspectors.Add(new DemoMsgInspector());
		}
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			Utils.LogToFile("ApplyDispatchBehavior. ");
			// No implementation necessary
		}
		public void Validate(ServiceEndpoint endpoint)
		{
			Utils.LogToFile("Validate. endpoint.Address = " + endpoint.Address);
			Utils.LogToFile("endpoint.Contract = " + endpoint.Contract);
			// No implementation necessary
		}
	}

	// Configuration element 
	public class DemoLoggerBehaviorExtension : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof(DemoLogger); }
		}

		protected override object CreateBehavior()
		{
			// Create the  endpoint behavior that will insert the message
			// inspector into the client runtime
			return new DemoLogger();
		}
	}

}
