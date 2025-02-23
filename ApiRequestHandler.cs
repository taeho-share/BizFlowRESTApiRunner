using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BizFlowRESTapiRunner
{
    /// <summary>
    /// Handles API requests to the BizFlow REST API.
    /// </summary>
    class ApiRequestHandler
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpointUrl; // BizFlow API endpoint URL
        private readonly string _apiKey; // API key for encryption
        private string _sessionKey; // BizFlow session key
        public string JsessionId { get; set; } // J2EE session ID
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="apiKey">The API key for encryption.</param>
        /// <param name="endpointUrl">The BizFlow API endpoint URL.</param>
        public ApiRequestHandler(ILogger logger, string apiKey, string endpointUrl)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
            _logger = logger;
            _apiKey = apiKey;
            _endpointUrl = endpointUrl;
        }

        /// <summary>
        /// Sets the session key for the API requests.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        public void SetSessionKey(string sessionKey)
        {
            _sessionKey = sessionKey;
        }

        /// <summary>
        /// Sends a synchronous API request with the given payload.
        /// </summary>
        /// <param name="payload">The payload to send in the request.</param>
        /// <returns>The response content as a string.</returns>
        public string SendApiRequest(object payload)
        {
            try
            {
                // Serialize and beautify the payload for logging
                string beautifiedPayload = CommonUtils.BeautifyJsonString(JsonSerializer.Serialize(payload));
                string jsonPayload = JsonSerializer.Serialize(payload);
                _logger.LogInformation("payload: {JsonPayload}", beautifiedPayload);

                // Encrypt the JSON payload
                var encryptedJsonPayload = AesEncryptionHelper.EncryptPayload(jsonPayload, _apiKey);
                _logger.LogInformation("encrypted-payload: {EncryptedJsonPayload}", encryptedJsonPayload);

                // Prepare the request content
                var requestContent = new StringContent(encryptedJsonPayload, Encoding.UTF8);
                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

                // Create the HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
                {
                    Content = requestContent
                };
                request.Headers.Add("User-Agent", "ApiTester/1.0");
                request.Headers.Add("x-bizflow-session-key", _sessionKey);
                CommonUtils.WriteColoredLine($"x-bizflow-session-key: {_sessionKey}", ConsoleColor.DarkMagenta);

                // Add JSESSIONID to the request header if available
                if (JsessionId != null)
                {
                    _logger.LogInformation("request-JSESSIONID: {JSESSIONID}", JsessionId);
                    CommonUtils.WriteColoredLine($"Set-Cookie JSESSIONID: {JsessionId}", ConsoleColor.DarkMagenta);
                    request.Headers.Add("Cookie", $"JSESSIONID={JsessionId}; Path=/bizflow; HttpOnly");
                }
                CommonUtils.WriteColoredLine(jsonPayload, ConsoleColor.Magenta);

                // Send the request and get the response
                var response = _httpClient.Send(request);
                string responseContent = response.Content.ReadAsStringAsync().Result;

                // Extract JSESSIONID from the response if available
                string jsessionId = GetJSESSIONID(response);
                if (jsessionId != null)
                {
                    JsessionId = jsessionId;
                    _logger.LogInformation("response-JSESSIONID: {JSESSIONID}", jsessionId);
                }

                CommonUtils.WriteColoredLine(responseContent, ConsoleColor.Green);
                _logger.LogInformation("\r\n {Response}", responseContent);

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error calling API: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Sends an asynchronous API request with the given payload.
        /// </summary>
        /// <param name="payload">The payload to send in the request.</param>
        /// <returns>The response content as a string.</returns>
        public async Task<string> SendApiRequestAsync(object payload)
        {
            try
            {
                // Serialize and beautify the payload for logging
                string beautifiedPayload = CommonUtils.BeautifyJsonString(JsonSerializer.Serialize(payload));
                string jsonPayload = JsonSerializer.Serialize(payload);
                _logger.LogInformation("payload: \n\r{JsonPayload}", beautifiedPayload);

                // Encrypt the JSON payload
                var encryptedJsonPayload = AesEncryptionHelper.EncryptPayload(jsonPayload, _apiKey);
                _logger.LogInformation("encryptedPayload: \n\r{EncryptedJsonPayload}", encryptedJsonPayload);

                // Prepare the request content
                var requestContent = new StringContent(encryptedJsonPayload, Encoding.UTF8);
                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

                // Create the HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
                {
                    Content = requestContent
                };
                request.Headers.Add("User-Agent", "ApiTester/1.0");
                request.Headers.Add("x-bizflow-session-key", _sessionKey);

                // Add JSESSIONID to the request header if available
                if (JsessionId != null)
                {
                    _logger.LogInformation("request-JSESSIONID: {JSESSIONID}", JsessionId);
                    CommonUtils.WriteColoredLine("Set-Cookie JSESSIONID: " + JsessionId, ConsoleColor.Magenta);
                    request.Headers.Add("Cookie", $"JSESSIONID={JsessionId}; Path=/bizflow; HttpOnly");
                }
                CommonUtils.WriteColoredLine(jsonPayload, ConsoleColor.Magenta);

                // Send the request and get the response
                var response = await _httpClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();

                // Extract JSESSIONID from the response if available
                string jsessionId = GetJSESSIONID(response);
                if (jsessionId != null)
                {
                    JsessionId = jsessionId;
                    _logger.LogInformation("response-JSESSIONID: {JSESSIONID}", jsessionId);
                }

                CommonUtils.WriteColoredLine(responseContent, ConsoleColor.Green);
                _logger.LogInformation("response: \n\r{Response}", responseContent);

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error calling API: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Extracts the JSESSIONID from the response headers.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        /// <returns>The JSESSIONID if found; otherwise, null.</returns>
        public static string GetJSESSIONID(HttpResponseMessage response)
        {
            // Example: Header: Set-Cookie = JSESSIONID=33A0573202265208B349F597C90AFDE5; Path=/bizflow; HttpOnly
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    if (cookie.Contains("JSESSIONID") && cookie.Contains("Path=/bizflow"))
                    {
                        var jsessionId = cookie.Split(';')
                                               .FirstOrDefault(part => part.Trim().StartsWith("JSESSIONID"))
                                               ?.Split('=')[1];
                        return jsessionId;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Clears all cookies from the HttpClientHandler.
        /// </summary>
        public void ClearAllCookies()
        {
            var handler = _httpClient.DefaultRequestHeaders;
            handler.Clear();
            _logger.LogInformation("All cookies have been cleared.");
        }
    }
}
