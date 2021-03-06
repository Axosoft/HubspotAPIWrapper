﻿using System;
using System.IO;
using System.Net;

namespace HubspotAPIWrapper
{
    internal class WindowsWebClient : IWebClient
    {
        public string UploadString(string uri, string method = "GET", string contentType = "application/text",
                                   string data = "")
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = method;
            request.ContentType = contentType;

            if (data.Length > 0)
            {
                var writer = new StreamWriter(request.GetRequestStream());
                writer.Write(data);
                writer.Close();
            }


            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        var streamReader = new StreamReader(responseStream);
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                using (var response = (HttpWebResponse) e.Response)
                {
                    if (response == null)
                    {
                        Axosoft.Common.ExceptionManagement.ExceptionManager.Publish(e, new System.Collections.Generic.Dictionary<string, string> {
                            { "uri", uri },
                            { "response", "null" }
                        });
                    }
                    else
                    {
                        Console.WriteLine("Error code: {0}", response.StatusCode);
                        string responseText = string.Empty;
                        Stream responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            var reader = new StreamReader(responseStream);
                            responseText = reader.ReadToEnd();
                            Console.WriteLine(responseText);
                        }
                        Axosoft.Common.ExceptionManagement.ExceptionManager.Publish(e, new System.Collections.Generic.Dictionary<string, string> {
                            { "uri", uri },
                            { "response", responseText }
                        });
                    }
                }
            }

            return String.Empty;
        }
    }
}
