using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Networking;

namespace RestClient.Core
{
	public class RestWebClient : Singleton<RestWebClient>
	{
		private const string defaultContentType = "application/json";
		public IEnumerator HttpGet(string url, System.Action<Response> callback)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
			{
				yield return webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					if (PulseCommunicator.Pulse)
					{
						//InterfaceController.Instance.LogWarning(webRequest.error);
						UnityEngine.Debug.LogWarning(webRequest.error);
					}

					// callback(new Response
					// {
					// 	StatusCode = webRequest.responseCode,
					// 	Error = webRequest.error,
					// });
				}

				if (webRequest.isDone)
				{
					string data = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
					// data = data.Replace("\\u0027", "'");
					// Debug.Log("Data: " + data);
					callback(new Response
					{
						StatusCode = webRequest.responseCode,
						Error = webRequest.error,
						Data = data
					});
				}
			}
		}

		// public IEnumerator HttpDelete(string url, System.Action<Response> callback)
		// {
		// 	using (UnityWebRequest webRequest = UnityWebRequest.Delete(url))
		// 	{
		// 		yield return webRequest.SendWebRequest();

		// 		if (webRequest.result == UnityWebRequest.Result.ConnectionError)
		// 		{
		// 			callback(new Response
		// 			{
		// 				StatusCode = webRequest.responseCode,
		// 				Error = webRequest.error
		// 			});
		// 		}

		// 		if (webRequest.isDone)
		// 		{
		// 			callback(new Response
		// 			{
		// 				StatusCode = webRequest.responseCode
		// 			});
		// 		}
		// 	}
		// }

		public IEnumerator HttpPost(string url, string body, System.Action<Response> callback, IEnumerable<RequestHeader> headers = null)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Post(url, body))
			{
				if (headers == null)
				{
					headers = new List<RequestHeader>()
					{
						new RequestHeader
						{
							Key = "Content-Type",
							Value = "application/json"
						}
					};
				}

				if (headers != null)
				{
					foreach (RequestHeader header in headers)
					{
						webRequest.SetRequestHeader(header.Key, header.Value);
					}
				}

				webRequest.uploadHandler.contentType = defaultContentType;
				webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));

				yield return webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					if (PulseCommunicator.Pulse)
					{
						//InterfaceController.Instance.LogWarning(webRequest.error);
						UnityEngine.Debug.LogWarning(webRequest.error);
					}

					// callback(new Response
					// {
					// 	StatusCode = webRequest.responseCode,
					// 	Error = webRequest.error
					// });
				}
				else if (webRequest.isDone)
				{
					string data = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
					callback(new Response
					{
						StatusCode = webRequest.responseCode,
						Error = webRequest.error,
						Data = data
					});
				}
			}
		}

		public IEnumerator HttpPut(string url, string body, System.Action<Response> callback, IEnumerable<RequestHeader> headers = null)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Put(url, body))
			{
				if (headers == null)
				{
					headers = new List<RequestHeader>()
					{
						new RequestHeader
						{
							Key = "Content-Type",
							Value = "application/json"
						}
					};
				}

				if (headers != null)
				{
					foreach (RequestHeader header in headers)
					{
						webRequest.SetRequestHeader(header.Key, header.Value);
					}
				}

				webRequest.uploadHandler.contentType = defaultContentType;
				webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));

				yield return webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					if (PulseCommunicator.Pulse)
					{
						//InterfaceController.Instance.LogWarning(webRequest.error);
						UnityEngine.Debug.LogWarning(webRequest.error);
					}
					// callback(new Response
					// {
					// 	StatusCode = webRequest.responseCode,
					// 	Error = webRequest.error
					// });
				}
				else if (webRequest.isDone)
				{
					string data = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
					callback(new Response
					{
						StatusCode = webRequest.responseCode,
						Error = webRequest.error,
						Data = data
					});
				}
			}
		}

		// public IEnumerator HttpPut(string url, string body, System.Action<Response> callback, IEnumerable<RequestHeader> headers = null)
		// {
		// 	using (UnityWebRequest webRequest = UnityWebRequest.Put(url, body))
		// 	{
		// 		if (headers != null)
		// 		{
		// 			foreach (RequestHeader header in headers)
		// 			{
		// 				webRequest.SetRequestHeader(header.Key, header.Value);
		// 			}
		// 		}

		// 		webRequest.uploadHandler.contentType = defaultContentType;
		// 		webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));

		// 		yield return webRequest.SendWebRequest();

		// 		if (webRequest.result == UnityWebRequest.Result.ConnectionError)
		// 		{
		// 			callback(new Response
		// 			{
		// 				StatusCode = webRequest.responseCode,
		// 				Error = webRequest.error,
		// 			});
		// 		}

		// 		if (webRequest.isDone)
		// 		{
		// 			callback(new Response
		// 			{
		// 				StatusCode = webRequest.responseCode,
		// 			});
		// 		}
		// 	}
		// }

		public IEnumerator HttpHead(string url, System.Action<Response> callback)
		{
			using (UnityWebRequest webRequest = UnityWebRequest.Head(url))
			{
				yield return webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				{
					callback(new Response
					{
						StatusCode = webRequest.responseCode,
						Error = webRequest.error,
					});
				}

				if (webRequest.isDone)
				{
					var responseHeaders = webRequest.GetResponseHeaders();
					callback(new Response
					{
						StatusCode = webRequest.responseCode,
						Error = webRequest.error,
						Headers = responseHeaders
					});
				}
			}
		}
	}
}