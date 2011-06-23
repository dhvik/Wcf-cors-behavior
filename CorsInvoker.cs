using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace Cors {
	/// <summary>
	/// Handles invocations for the Cors behaviour
	/// </summary>
	/// <remarks>
	/// 2011-06-23 dan: Created
	/// </remarks>
	public class CorsInvoker : IOperationInvoker {
		/* *******************************************************************
         *  Properties 
         * *******************************************************************/
		#region public bool IsSynchronous
		/// <summary>
		/// Gets a value that specifies whether the <see cref="M:System.ServiceModel.Dispatcher.IOperationInvoker.Invoke(System.Object,System.Object[],System.Object[]@)"/> or <see cref="M:System.ServiceModel.Dispatcher.IOperationInvoker.InvokeBegin(System.Object,System.Object[],System.AsyncCallback,System.Object)"/> method is called by the dispatcher.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// true if the dispatcher invokes the synchronous operation; otherwise, false.
		/// </returns>
		public bool IsSynchronous {
			get { return true; }
		}
		#endregion
		private readonly IOperationInvoker _originalInvoker;
		/* *******************************************************************
         *  Constructors 
         * *******************************************************************/
		#region public CorsInvoker()
		/// <summary>
		/// Initializes a new instance of the <b>CorsInvoker</b> class.
		/// </summary>
		/// <param name="invoker"></param>
		public CorsInvoker(IOperationInvoker invoker) {
			if (!invoker.IsSynchronous) {
				throw new NotSupportedException("This implementation only supports syncronous invokers.");
			}
			_originalInvoker = invoker;
		}
		#endregion
		/* *******************************************************************
         *  Methods 
         * *******************************************************************/
		#region public object[] AllocateInputs()
		/// <summary>
		/// Returns an <see cref="T:System.Array"/> of parameter objects.
		/// </summary>
		/// <returns>
		/// The parameters that are to be used as arguments to the operation.
		/// </returns>
		public object[] AllocateInputs() {
			return _originalInvoker.AllocateInputs();
		}
		#endregion
		#region public object Invoke(object instance, object[] inputs, out object[] outputs)
		/// <summary>
		/// Returns an object and a set of output objects from an instance and set of input objects.  
		/// </summary>
		/// <param name="instance">The object to be invoked.</param>
		/// <param name="inputs">The inputs to the method.</param>
		/// <param name="outputs">The outputs from the method.</param>
		/// <returns>
		/// The return value.
		/// </returns>
		public object Invoke(object instance, object[] inputs, out object[] outputs) {
			if (OperationContext.Current.IncomingMessageProperties.ContainsKey(CorsDispatchMessageInspector.CrossOriginResourceSharingPropertyName)) {
				var state = OperationContext.Current.IncomingMessageProperties[CorsDispatchMessageInspector.CrossOriginResourceSharingPropertyName] as CorsState;
				if (state != null && state.Message != null) {
					outputs = null;
					return null;
				}
			}
			return _originalInvoker.Invoke(instance, inputs, out outputs);
		}
		#endregion
		#region public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
		/// <summary>
		/// An asynchronous implementation of the <see cref="M:System.ServiceModel.Dispatcher.IOperationInvoker.Invoke(System.Object,System.Object[],System.Object[]@)"/> method.
		/// </summary>
		/// <param name="instance">The object to be invoked.</param>
		/// <param name="inputs">The inputs to the method.</param>
		/// <param name="callback">The asynchronous callback object.</param>
		/// <param name="state">Associated state data.</param>
		/// <returns>
		/// A <see cref="T:System.IAsyncResult"/> used to complete the asynchronous call.
		/// </returns>
		/// <exception cref="NotSupportedException"></exception>
		public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state) {
			throw new NotSupportedException();
		}
		#endregion
		#region public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
		/// <summary>
		/// The asynchronous end method.
		/// </summary>
		/// <param name="instance">The object invoked.</param>
		/// <param name="outputs">The outputs from the method.</param>
		/// <param name="result">The <see cref="T:System.IAsyncResult"/> object.</param>
		/// <returns>
		/// The return value.
		/// </returns>
		/// <exception cref="NotSupportedException"></exception>
		public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result) {
			throw new NotSupportedException();
		}
		#endregion
	}
}