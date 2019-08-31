using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Net.Abstractions
{

    public interface IHttpClient
    {

        int Timeout { get; set; }

        Task<HttpClientResponse> GetAsync(Uri url);

        Task<HttpClientResponse> GetAsync(Uri url, IDictionary<string, string> parameters);

        Task<HttpClientResponse> PostAsync(Uri url);

        Task<HttpClientResponse> PostAsync(Uri url, IDictionary<string, string> parameters);

        Task<HttpClientResponse> RequestAsync(HttpMethod method, Uri url, IDictionary<string, string> parameters);

        Task<HttpClientResponse> RequestAsync(HttpMethod method, Uri url, IDictionary<string, string> parameters, string contentType);

    }

    public class HttpClientResponse
    {
        public string Response { get; set; }
        
        public bool Succeeded { get; set; }

        public string Error { get; set; }

    }

    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete
    }
    
}
