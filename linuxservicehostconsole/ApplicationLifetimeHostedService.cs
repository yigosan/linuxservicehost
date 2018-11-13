using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace linuxservicehostconsole
{
    public class ApplicationLifetimeHostedService : IHostedService
    {
        IApplicationLifetime appLifetime;
        ILogger<ApplicationLifetimeHostedService> logger;
        IHostingEnvironment environment;
        IConfiguration configuration;

        public ApplicationLifetimeHostedService(
            IConfiguration configuration,
            IHostingEnvironment environment,
            ILogger<ApplicationLifetimeHostedService> logger,
            IApplicationLifetime appLifetime)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.appLifetime = appLifetime;
            this.environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("StartAsync method called.");

            this.appLifetime.ApplicationStarted.Register(OnStarted);
            this.appLifetime.ApplicationStopping.Register(OnStopping);
            this.appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;

        }

        private void OnStarted()
        {
            this.logger.LogInformation("OnStarted method called.");
            CryptoTest crypto = new CryptoTest();

            string text = "yigit";
            

            String encrypted = crypto.Encrypt(text);
            string base64orig = Convert.ToBase64String(Encoding.UTF8.GetBytes(encrypted));
            this.logger.LogInformation("encrypted -> " + base64orig);

            byte[] data = Convert.FromBase64String(base64orig);
            String base64decrypt = Encoding.UTF8.GetString(data, 0, data.Length);
            String decrypted = crypto.Decrypt(base64decrypt);
            this.logger.LogInformation("decrypted -> " + decrypted);

            String detectedLiveIpV4 = GetLiveIpAddressV4().ToString();
            String detectedLiveIpV6 = GetLiveIpAddressV6().ToString();


            this.logger.LogInformation("Live IPv4 -> " + detectedLiveIpV4);
            this.logger.LogInformation("Live IPv6 -> " + detectedLiveIpV6);

            // Post-startup code goes here  
        }

        private void OnStopping()
        {
            this.logger.LogInformation("OnStopping method called.");

            // On-stopping code goes here  
        }

        private void OnStopped()
        {
            this.logger.LogInformation("OnStopped method called.");

            // Post-stopped code goes here  
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("StopAsync method called.");

            return Task.CompletedTask;
        }

        public static IPAddress GetLiveIpAddressV4()
        {
            // Just make a call to a dummy address. Address need not be existing.
            IPAddress localIP = null;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                try
                {
                    socket.Connect("10.0.4.4", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address;
                }
                catch (Exception) { }
            }
            return localIP;
        }

        public static IPAddress GetLiveIpAddressV6()
        {
            // Just make a call to a dummy address. Address need not be existing.
            IPAddress localIP = null;
            using (Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, 0))
            {
                try
                {
                    socket.Connect("dddd:dddd:140:11e1:ffff:ffff::1", 65530); // dummy ip address
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address;
                }
                catch (Exception) { }
            }
            return localIP;
        }

    }
}
