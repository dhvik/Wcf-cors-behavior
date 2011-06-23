using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Cors {
	/// <summary>
	/// This behavior should be added to both endpoint and operations to make it work.
	/// This will detect cors requests and reply on preflight request, plus supply any needed headers to make rest wcf calls work seamlessly.
	/// </summary>
	/// <remarks>
	/// 2011-06-23 dan: Created
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
	public class CorsBehaviorAttribute : Attribute, IEndpointBehavior, IOperationBehavior {
		/* *******************************************************************
         *  Properties 
         * *******************************************************************/
		#region public string AllowOrigin
		/// <summary>
		/// Get/Sets the AllowOrigin of the CrossOriginResourceSharingBehaviorAttribute
		/// </summary>
		/// <value></value>
		public string AllowOrigin {
			get { return _allowOrigin; }
			set { _allowOrigin = value; }
		}
		private string _allowOrigin = "*";
		#endregion
		#region public string AllowMethods
		/// <summary>
		/// Get/Sets the AllowMethods of the CrossOriginResourceSharingBehaviorAttribute
		/// </summary>
		/// <value></value>
		public string AllowMethods {
			get { return _allowMethods; }
			set { _allowMethods = value; }
		}
		private string _allowMethods = "POST, OPTIONS, GET";
		#endregion
		#region public string AllowHeaders
		/// <summary>
		/// Get/Sets the AllowHeaders of the CrossOriginResourceSharingBehaviorAttribute
		/// </summary>
		/// <value></value>
		public string AllowHeaders {
			get { return _allowHeaders; }
			set { _allowHeaders = value; }
		}
		private string _allowHeaders = "Content-Type, Accept, Authorization, x-requested-with";
		#endregion
		/* *******************************************************************
         *  Methods 
         * *******************************************************************/
		#region public void Validate(ServiceEndpoint endpoint)
		/// <summary>
		/// Implement to confirm that the endpoint meets some intended criteria.
		/// </summary>
		/// <param name="endpoint">The endpoint to validate.</param>
		public void Validate(ServiceEndpoint endpoint) { }
		#endregion
		#region public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		/// <summary>
		/// Implement to pass data at runtime to bindings to support custom behavior.
		/// </summary>
		/// <param name="endpoint">The endpoint to modify.</param>
		/// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
		#endregion
		#region public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		/// <summary>
		/// Implements a modification or extension of the service across an endpoint.
		/// </summary>
		/// <param name="endpoint">The endpoint that exposes the contract.</param>
		/// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) {
			//adds an inspector that detect cors requests and marks them so the operation invoker/formatter can detect it
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CorsDispatchMessageInspector(endpoint, this));
		}
		#endregion
		#region public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		/// <summary>
		/// Implements a modification or extension of the client across an endpoint.
		/// </summary>
		/// <param name="endpoint">The endpoint that is to be customized.</param>
		/// <param name="clientRuntime">The client runtime to be customized.</param>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }
		#endregion
		#region public void Validate(OperationDescription operationDescription)
		/// <summary>
		/// Implement to confirm that the operation meets some intended criteria.
		/// </summary>
		/// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
		public void Validate(OperationDescription operationDescription) { }
		#endregion
		#region public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		/// <summary>
		/// Implements a modification or extension of the service across an operation.
		/// </summary>
		/// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
		/// <param name="dispatchOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription"/>.</param>
		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation) {
			//For every opertation we inject a formatter and an invoker to detect Cors calls
			dispatchOperation.Formatter = new CorsFormatter(dispatchOperation.Formatter);
			dispatchOperation.Invoker = new CorsInvoker(dispatchOperation.Invoker);
		}
		#endregion
		#region public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		/// <summary>
		/// Implements a modification or extension of the client across an operation.
		/// </summary>
		/// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
		/// <param name="clientOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription"/>.</param>
		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) { }
		#endregion
		#region public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		/// <summary>
		/// Implement to pass data at runtime to bindings to support custom behavior.
		/// </summary>
		/// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
		/// <param name="bindingParameters">The collection of objects that binding elements require to support the behavior.</param>
		public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }
		#endregion
	}
}