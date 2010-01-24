using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;

namespace WpadProxy
{
	public class WpadProxy : IHttpHandler
	{
		#region IHttpHandler Members
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			try
			{
				string requestUrl = ConfigurationManager.AppSettings.Get(context.Request.Url.AbsolutePath);
				if (requestUrl == null)
				{
					context.Response.StatusCode = 500;
					context.Response.StatusDescription = "Requested URL not found";
					context.Response.Write(context.Response.StatusDescription);
					return;
				}

				// Build HTTP request.
				HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(requestUrl);
				request.Proxy = null;
				request.Headers.Clear();
				request.Headers.Add(request.Headers);

				// Get Response.
				HttpWebResponse response = (HttpWebResponse) request.GetResponse();
				using (Stream stream = response.GetResponseStream())
				{
					context.Response.ClearHeaders();
					// Copy headers.
					foreach (string header in response.Headers.AllKeys)
					{
						context.Response.AddHeader(header, response.Headers.Get(header));
					}
					
					// Copy content.
					context.Response.ClearContent();
					byte[] buffer = new byte[2000];
					int bytesRead;
					while ((bytesRead = stream.Read(buffer, 0, 500)) > 0)
					{
						context.Response.OutputStream.Write(buffer, 0, bytesRead);
					}
				}
			}
			catch (WebException ex)
			{
				throw ex;
			}
		}
		#endregion
	}
}