using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BizFlowRESTapiRunner
{
    /// <summary>
    /// Handles authentication-related API requests to the BizFlow REST API.
    /// </summary>
    class AuthenticationAPI
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly ApiRequestHandler _apiRequestHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationAPI"/> class with dependencies.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="config">The configuration instance.</param>
        /// <param name="apiRequestHandler">The API request handler instance.</param>
        public AuthenticationAPI(ILogger logger, IConfigurationRoot config, ApiRequestHandler apiRequestHandler)
        {
            this._logger = logger;
            this._config = config;
            this._apiRequestHandler = apiRequestHandler;
        }

        /// <summary>
        /// Logs in to the BizFlow API synchronously.
        /// </summary>
        /// <param name="loginId">The login ID.</param>
        /// <param name="password">The password.</param>
        /// <returns>The response from the API as a string.</returns>
        public string Login(string loginId, string password)
        {
            _logger.LogInformation("API [Authentication-Login]");

            _apiRequestHandler.SetSessionKey(null); // Clear the session key to start a new BizFlow session
            _apiRequestHandler.ClearAllCookies(); // Clear all cookies to start a new session

            string response = null;

            try
            {
                // Create the payload for the login request
                var payload = new
                {
                    @object = "authentication",
                    action = "login",
                    query = new
                    {
                        id = loginId,
                        pwd = password,
                    }
                };

                // Send the API request and get the response
                response = _apiRequestHandler.SendApiRequest(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication.");
            }

            return response;
        }

        /// <summary>
        /// Logs in to the BizFlow API asynchronously.
        /// </summary>
        /// <param name="loginId">The login ID.</param>
        /// <param name="password">The password.</param>
        /// <returns>The response from the API as a string.</returns>
        /// 

        public async Task<string> LoginAsync(string loginId, string password)
        {
            _logger.LogInformation("API [Authentication-LoginAsync]");

            _apiRequestHandler.SetSessionKey(null); // Clear the session key to start a new BizFlow session
            _apiRequestHandler.ClearAllCookies();

            string response = null;

            try
            {
                // Create the payload for the login request
                var payload = new
                {
                    @object = "authentication",
                    action = "login",
                    query = new
                    {
                        id = loginId,
                        pwd = password,
                    }
                };

                // Send the API request asynchronously and get the response
                response = await _apiRequestHandler.SendApiRequestAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication.");
            }

            return response;
        }

        public string Logout()
        {
            _logger.LogInformation("API [Authentication-Logout]");

            string response = null;

            try
            {
                // Create the payload for the login request
                var payload = new
                {
                    @object = "authentication",
                    action = "logout",
                    query = new
                    {
                        allSessions = false //If this value is true, the current login user will log out of all sessions and all devices.
                    }
                };

                // Send the API request and get the response
                response = _apiRequestHandler.SendApiRequest(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication.");
            }

            return response;
        }

        public async Task<string> LogoutAsync()
        {
            _logger.LogInformation("API [Authentication-LogoutAsync]");

            string response = null;

            try
            {
                // Create the payload for the login request
                var payload = new
                {
                    @object = "authentication",
                    action = "logout",
                    query = new
                    {
                        allSessions = false //If this value is true, the current login user will log out of all sessions and all devices.
                    }
                };

                // Send the API request asynchronously and get the response
                response = await _apiRequestHandler.SendApiRequestAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication.");
            }

            return response;
        }


    }





}
