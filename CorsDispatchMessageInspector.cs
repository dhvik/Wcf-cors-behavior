using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace Cors {
	/// <summary>
	/// Inspects the incoming message and looks for cors requests
	/// </summary>
	/// <remarks>
	/// 2011-06-23 dan: Created
	/// </remarks>
	public class CorsDispatchMessageInspector : IDispatchMessageInspector {
		/* *******************************************************************
         *  Properties 
         * *******************************************************************/
		private readonly ServiceEndpoint _serviceEndpoint;
		private readonly CorsBehaviorAttribute _behavior;
		internal const string CrossOriginResourceSharingPropertyName = "CrossOriginResourcSharingState";
		/* *******************************************************************
         *  Constructors 
         * *******************************************************************/
		#region public CorsDispatchMessageInspector()
		/// <summary>
		/// Initializes a new instance of the <b>CorsDispatchMessageInspector</b> class.
		/// </summary>
		public CorsDispatchMessageInspector(ServiceEndpoint serviceEndpoint, CorsBehaviorAttribute behavior) {
			_serviceEndpoint = serviceEndpoint;
			_behavior = behavior;
		}
		#endregion
		/* *******************************************************************
         *  Methods 
         * *******************************************************************/
		#region public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		/// <summary>
		/// Called after an inbound message has been received but before the message is dispatched to the intended operation.
		/// </summary>
		/// <param name="request">The request message.</param>
		/// <param name="channel">The incoming channel.</param>
		/// <param name="instanceContext">The current service instance.</param>
		/// <returns>
		/// The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)"/> method.
		/// </returns>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext) {
			CorsState state = null;
			HttpRequestMessageProperty reqProp = null;
			if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name)) {
				reqProp = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
			}

			if (reqProp != null) {

				//Handle cors requests
				var origin = reqProp.Headers["Origin"];
				if (!string.IsNullOrEmpty(origin)) {
					state = new CorsState();
					//if a cors options request (preflight) is detected, we create our own reply message and don't invoke any operation at all.
					if (reqProp.Method == "OPTIONS") {
						state.Message = Message.CreateMessage(request.Version, FindReplyAction(request.Headers.Action), new EmptyBodyWriter());
					}
					request.Properties.Add(CrossOriginResourceSharingPropertyName, state);
				}
			}
			return state;
		}
		#endregion
		#region private string FindReplyAction(string requestAction)
		/// <summary>
		/// Finds the reply action based on the supplied request action
		/// </summary>
		/// <param name="requestAction">The request action for witch the reply action should be found.</param>
		/// <returns></returns>
		private string FindReplyAction(string requestAction) {
			foreach (var operation in _serviceEndpoint.Contract.Operations) {
				if (operation.Messages[0].Action == requestAction) {
					return operation.Messages[1].Action;
				}
			}
			return null;
		}
		#endregion

		/// <summary>
		/// A simple body writer that writes nothing
		/// </summary>
		class EmptyBodyWriter : BodyWriter {
			#region public EmptyBodyWriter()
			/// <summary>
			/// Initializes a new instance of the <b>EmptyBodyWriter</b> class.
			/// </summary>
			public EmptyBodyWriter() : base(true) { }
			#endregion
			#region protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
			/// <summary>
			/// When implemented, provides an extensibility point when the body contents are written.
			/// </summary>
			/// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter"/> used to write out the message body.</param>
			protected override void OnWriteBodyContents(XmlDictionaryWriter writer) { }
			#endregion
		}

		#region public void BeforeSendReply(ref Message reply, object correlationState)
		/// <summary>
		/// Called after the operation has returned but before the reply message is sent.
		/// </summary>
		/// <param name="reply">The reply message. This value is null if the operation is one way.</param>
		/// <param name="correlationState">The correlation object returned from the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(System.ServiceModel.Channels.Message@,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)"/> method.</param>
		public void BeforeSendReply(ref Message reply, object correlationState) {
			var state = correlationState as CorsState;
			if (state != null) {
				if (state.Message != null) {
					reply = state.Message;
				}
				HttpResponseMessageProperty reqProp = null;
				if (reply.Properties.ContainsKey(HttpResponseMessageProperty.Name)) {
					reqProp = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
				}
				if (reqProp == null) {
					reqProp = new HttpResponseMessageProperty();
					reply.Properties.Add(HttpResponseMessageProperty.Name, reqProp);
				}
				//Acao should be added for all cors responses
				reqProp.Headers.Add("Access-Control-Allow-Origin", _behavior.AllowOrigin);
				if (state.Message != null) {
					//the following headers should only be added for OPTIONS requests
					reqProp.Headers.Add("Access-Control-Allow-Methods", _behavior.AllowMethods);
					reqProp.Headers.Add("Access-Control-Allow-Headers", _behavior.AllowHeaders);
				}

			}
		}
		#endregion

	}
	class CorsState {
		public Message Message;
	}
}