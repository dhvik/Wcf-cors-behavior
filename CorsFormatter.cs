using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Cors {
	/// <summary>
	/// Handles request/response formatting for the Cors behavior
	/// </summary>
	/// <remarks>
	/// 2011-06-23 dan: Created
	/// </remarks>
	public class CorsFormatter : IDispatchMessageFormatter {
		/* *******************************************************************
         *  Properties 
         * *******************************************************************/
		private readonly IDispatchMessageFormatter _originalFormatter;
		/* *******************************************************************
         *  Constructors 
         * *******************************************************************/
		#region public CorsFormatter(IDispatchMessageFormatter formatter)
		/// <summary>
		/// Initializes a new instance of the <b>CorsFormatter</b> class.
		/// </summary>
		/// <param name="formatter"></param>
		public CorsFormatter(IDispatchMessageFormatter formatter) {
			_originalFormatter = formatter;
		}
		#endregion
		/* *******************************************************************
         *  Methods 
         * *******************************************************************/
		/// <summary>
		/// Deserializes a message into an array of parameters.
		/// </summary>
		/// <param name="message">The incoming message.</param><param name="parameters">The objects that are passed to the operation as parameters.</param>
		public void DeserializeRequest(Message message, object[] parameters) {
		if (message.Properties.ContainsKey(CorsDispatchMessageInspector.CrossOriginResourceSharingPropertyName)) {
				var state = message.Properties[CorsDispatchMessageInspector.CrossOriginResourceSharingPropertyName] as CorsState;
				if (state != null) {
					//if we have a message ready, skip normal deserialization
					if (state.Message != null) {
						OperationContext.Current.OutgoingMessageProperties.Add(CorsDispatchMessageInspector.CrossOriginResourceSharingPropertyName, state);
						return;
					}
				}
			}
			_originalFormatter.DeserializeRequest(message, parameters);
		}
		/// <summary>
		/// Serializes a reply message from a specified message version, array of parameters, and a return value.
		/// </summary>
		/// <returns>
		/// The serialized reply message.
		/// </returns>
		/// <param name="messageVersion">The SOAP message version.</param><param name="parameters">The out parameters.</param><param name="result">The return value.</param>
		public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result) {

			//see if we have a cors state with a predefined message.
			//in that case where we can ignore the whole serialization process
			if (OperationContext.Current.OutgoingMessageProperties.ContainsKey(CorsDispatchMessageInspector.CrossOriginResourceSharingPropertyName)) {
				var state = OperationContext.Current.OutgoingMessageProperties[CorsDispatchMessageInspector.CrossOriginResourceSharingPropertyName] as CorsState;
				if (state != null && state.Message != null) {
					return state.Message;
				}
			}
			return _originalFormatter.SerializeReply(messageVersion, parameters, result);
		}
	}
}