using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qia.Opc.Domain.Core
{
	public class OperationResponse<T> where T : class
	{
		private OperationResponse(T value, HttpStatusCode status, string message)
		{
			Success = true;
			Status = status;
			Value = value;
			Message = message;
		}

		private OperationResponse(IEnumerable<string> errors, HttpStatusCode status, string message)
		{
			Success = false;
			Status = status;
			Errors = errors ?? new List<string> { "An unexpected error occurred." };
			Message = string.IsNullOrEmpty(message) ? "Failed to process the request." : message;
		}

		public T Value { get; }
		public IEnumerable<string> Errors { get; }
		public bool Success { get; }
		public HttpStatusCode Status { get; }
		public string Message { get; }

		public static OperationResponse<T> CreateSuccess(
			T value,
			HttpStatusCode status = HttpStatusCode.OK,
			string message = "") => new OperationResponse<T>(value, status, message);

		public static OperationResponse<T> CreateFailure(
			IEnumerable<string> errors,
			HttpStatusCode status = HttpStatusCode.BadRequest,
			string message = "")
			=> new OperationResponse<T>(errors, status, message);
	}
}
