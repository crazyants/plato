using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Client.Services
{
    
    public class StopForumSpamClient : HttpClient, IStopForumSpamClient
    {
        
        private readonly string _format;

        public StopForumSpamClientOptions Options { get; private set; } = new StopForumSpamClientOptions();

        public StopForumSpamClient() 
        {
            this._format = "json";
        }

        public void Configure(Action<StopForumSpamClientOptions> configure)
        {
            configure(this.Options);
        }

        public async Task<Response> CheckEmailAddressAsync(string emailAddress)
        {
            return await this.CheckAsync(new {email = emailAddress});
        }

        public async Task<Response> CheckUsernameAsync(string username)
        {
            return await this.CheckAsync(new {username = username});
        }

        public async Task<Response> CheckIpAddressAsync(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out var address))
            {
                throw new ArgumentException("The ipAddress argument is not a valid IP address.");
            }

            return await this.CheckAsync(new {ip = address.ToString()});
        }

        public async Task<Response> CheckIpAddressAsync(IPAddress ipAddress)
        {
            return await this.CheckAsync(new {ip = ipAddress});
        }

        public async Task<Response> CheckAsync(string username, string emailAddress, string ipAddress)
        {
            IPAddress address = null;
            if (!string.IsNullOrWhiteSpace(ipAddress) && !IPAddress.TryParse(ipAddress, out address))
            {
                throw new ArgumentException("The ipAddress argument (" + ipAddress + ") is not a valid IP address.");
            }

            return await this.CheckAsync(username, emailAddress, address);
        }

        public async Task<Response> CheckAsync(string username, string emailAddress, IPAddress ipAddress)
        {

            var parameters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(username))
            {
                parameters.Add("username", username);
            }

            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                parameters.Add("email", emailAddress);
            }

            if (ipAddress != null)
            {
                parameters.Add("ip", ipAddress.ToString());
            }

            return await this.CheckAsync(parameters);
        }

        private async Task<Response> CheckAsync(object values)
        {
            var parameters = new Dictionary<string, string>();
            if (values != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(values))
                {
                    parameters.Add(property.Name, property.GetValue(values)?.ToString());
                }
            }

            return await this.CheckAsync(parameters);
        }

        private async Task<Response> CheckAsync(Dictionary<string, string> parameters)
        {
            parameters.Add("f", this._format);
            return Response.Parse( await this.GetAsync(new Uri("http://www.stopforumspam.com/api"), parameters), this._format);
        }

        public async Task<Response> AddSpammerAsync(string username, string emailAddress, string ipAddress)
        {
            if (IPAddress.TryParse(ipAddress, out var address))
            {
                return await this.AddSpammerAsync(username, emailAddress, address);
            }

            throw new ArgumentException("The ipAddress argument is not a valid IP address.");
        }

        public async Task<Response> AddSpammerAsync(string username, string emailAddress, IPAddress ipAddress)
        {

            if (this.Options == null)
            {
                throw new ArgumentNullException(nameof(this.Options));
            }

            if (string.IsNullOrWhiteSpace(this.Options.ApiKey))
            {
                throw new ArgumentNullException(nameof(this.Options.ApiKey));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            var parameters = new Dictionary<string, string>
            {
                {"username", username},
                {"ip_addr", ipAddress.ToString()},
                {"email", emailAddress},
                {"api_key", this.Options.ApiKey}
            };

            return Response.Parse(await this.PostAsync(new Uri("http://www.stopforumspam.com/add.php"), parameters), this._format);

        }

    }

}
