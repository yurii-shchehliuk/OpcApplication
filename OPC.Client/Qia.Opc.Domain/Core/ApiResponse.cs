using Newtonsoft.Json;
using System.Net;

namespace Qia.Opc.Domain.Core
{
	public class ApiResponse<T>
	{
		private ApiResponse(T value, HttpStatusCode status)
		{
			IsSuccess = true;
			Status = status;
			Value = value;
		}

		private ApiResponse(T value, HttpStatusCode status, string error)
		{
			IsSuccess = false;
			Status = status;
			Error = error;
			Value = value;
		}

		public T Value { get; }
		public string Error { get; }
		public bool IsSuccess { get; }
		public HttpStatusCode Status { get; }

		public static ApiResponse<T> Success(T value = default, HttpStatusCode status = HttpStatusCode.OK)
		{
			//LoggerManager.Logger.Information(status + ": " + value.GetType().Name);
			return new ApiResponse<T>(value, status);
		}

		public static ApiResponse<T> Failure(HttpStatusCode status, string error = "", T value = default)
		{
			if (string.IsNullOrEmpty(error))
				error = GetDefaultMessageForStatusCode((int)status);

			LoggerManager.Logger.Error(status + "\n" + error + "\n" + JsonConvert.SerializeObject(value, new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,

			}));
			return new ApiResponse<T>(value, status, error);
		}

		public static ApiResponse<T> HandledFailure(HttpStatusCode status, string message, T value = default)
		{
			LoggerManager.Logger.Information(status + " " + message + " " + JsonConvert.SerializeObject(value, new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,

			}));
			return new ApiResponse<T>(value, status, message);
		}

		internal static string GetDefaultMessageForStatusCode(int statusCode)
		{
			return statusCode switch
			{
				400 => "A bad request, you have made",
				401 => "Authorized, you are not",
				404 => "Resource found, it was not",
				502 => "Session is not connected",
				503 => "Cannot connect to session. Server is not available",
				_ => ((HttpStatusCode)statusCode).ToString()

			};
		}
	}
}
