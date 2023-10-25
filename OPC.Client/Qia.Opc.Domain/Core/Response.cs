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
	public class Response<T> where T : class
	{
		private Response(T value, int status = 200)
		{
			Ok = true;
			Status = status;
			Value = value;
		}

		private Response(Error error, int status = 500)
		{
			Ok = false;
			Status = status;
			Error = error;
		}

		public T Value { get; set; }
		public Error Error { get; set; }
		public bool Ok { get; set; }
		public int Status { get; set; }
		public string Message { get; }

		public static Response<T> Success(T value) => new(value);
		public static Response<Error> Fail(Error error)
			=> new(error);
	}

	public class Error
	{
		public Error(string stackTrace = null, string[] internalErrors = null)
		{
			StackTrace = stackTrace ?? Environment.StackTrace;
			InternalErrors = internalErrors;
		}

		public string StackTrace { get; }
		public string[] InternalErrors { get; }

		internal static string GetDefaultMessageForStatusCode(int statusCode)
		{
			return statusCode switch
			{
				400 => "A bad request, you have made",
				401 => "Authorized, you are not",
				404 => "Resource found, it was not",
				500 => "Errors are the path to the dark side. Errors lead to anger.  Anger leads to hate.  Hate leads to career change",
				_ => ((HttpStatusCode)statusCode).ToString()

			};
		}
	}
}
