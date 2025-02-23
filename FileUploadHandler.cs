using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BizFlowRESTapiRunner
{
    public class FileUploadHandler
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        
        private string _sessionKey; // BizFlow session key
        public string JsessionId { get; set; } // J2EE session ID

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger to use for logging information and errors.</param>
        public FileUploadHandler(ILogger logger)
        {
            _logger = logger;

            // Create an HttpClientHandler to handle server certificate validation
            var handler = new HttpClientHandler
            {
                // Bypass server certificate validation (not recommended for production)
                // This is needed to allow the HttpClient to communicate with servers that have self-signed certificates or other certificate issues.
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            // Initialize the HttpClient with the handler
            // HttpClient is used to send HTTP requests and receive HTTP responses from a resource identified by a URI.
            _httpClient = new HttpClient(handler);
        }

        public void SetSessionKey(string sessionKey)
        {
            _sessionKey = sessionKey;
        }

        public FileUploadResponse UploadFile(string apiEndpoint, string sessionKey, string filePath)
        {
            _logger.LogInformation("API [UploadFile]");

            try
            {
                // Load the file bytes from the specified file path
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Create a boundary string for the multipart form-data
                string boundary = "Boundary-" + DateTime.Now.Ticks.ToString("x");

                // Prepare the multipart content
                var content = new MultipartFormDataContent(boundary);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file", // Name of the form-data parameter
                    FileName = Path.GetFileName(filePath) // Name of the file being uploaded
                };

                // Get the MIME type of the file
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out string mimeType))
                {
                    mimeType = "application/octet-stream"; // Default MIME type if unknown
                }
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                content.Add(fileContent);

                // Create the HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint)
                {
                    Content = content // Set the content of the request
                };

                // Add the x-bizflow-session-key header to the request
                request.Headers.Add("x-bizflow-session-key", sessionKey);

                // Send the HTTP request and get the response
                var response = _httpClient.Send(request);
                response.EnsureSuccessStatusCode(); // Throw an exception if the response indicates an error

                // Read the response content as a string
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                Console.WriteLine($"API Response: {responseBody}");

                // Deserialize the response content into a FileInfoData object
                FileUploadResponse fileInfoData = JsonSerializer.Deserialize<FileUploadResponse>(responseBody);
                fileInfoData.ApiResponse = responseBody;

                // Return the FileInfoData object
                return fileInfoData;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError("Error uploading file: {Message}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Uploads a file to the specified API endpoint asynchronously.
        /// </summary>
        /// <param name="apiEndpoint">The API endpoint to upload the file to.</param>
        /// <param name="sessionKey">The session key to include in the request headers.</param>
        /// <param name="filePath">The path to the file to be uploaded.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a FileInfoData object with the response data, or null if an error occurs.</returns>
        public async Task<FileUploadResponse> UploadFileAsync(string apiEndpoint, string sessionKey, string filePath)
        {
            _logger.LogInformation("API [UploadFileAsync]");
            try
            {
                // Load the file bytes from the specified file path
                // This is needed to read the file content into a byte array so it can be sent in the HTTP request.
                byte[] fileBytes = await LoadFileAsync(filePath);

                // Create a boundary string for the multipart form-data
                // The boundary is used to separate different parts of the multipart content.
                string boundary = "Boundary-" + DateTime.Now.Ticks.ToString("x");

                // Prepare the multipart content
                // MultipartFormDataContent is used to send a form with a file upload.
                var content = new MultipartFormDataContent(boundary);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file", // Name of the form-data parameter
                    FileName = Path.GetFileName(filePath) // Name of the file being uploaded
                };

                // Get the MIME type of the file
                // This is needed to set the correct Content-Type header for the file being uploaded.
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out string mimeType))
                {
                    mimeType = "application/octet-stream"; // Default MIME type if unknown
                }
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                content.Add(fileContent);

                // Create the HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint)
                {
                    Content = content // Set the content of the request
                };

                // Add the x-bizflow-session-key header to the request
                // This is needed to authenticate the request with the session key.
                request.Headers.Add("x-bizflow-session-key", sessionKey);

                // Send the HTTP request and get the response
                // This sends the request to the server and waits for the response.
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Throw an exception if the response indicates an error

                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response: {responseBody}");

                // Deserialize the response content into a FileInfoData object
                FileUploadResponse fileInfoData = JsonSerializer.Deserialize<FileUploadResponse>(responseBody);
                fileInfoData.ApiResponse = responseBody;
                // Return the FileInfoData object
                return fileInfoData;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                // This is needed to log any errors that occur during the file upload process.
                _logger.LogError("Error uploading file: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Loads the file bytes asynchronously from the specified file path.
        /// </summary>
        /// <param name="filePath">The path to the file to be loaded.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the file bytes.</returns>
        private async Task<byte[]> LoadFileAsync(string filePath)
        {
            try
            {
                // Log the file loading operation
                // This is needed to provide information about the file loading process.
                _logger.LogInformation("Loading file from local disk: {FilePath}", filePath);
                // Read and return the file bytes
                // This reads the file content into a byte array.
                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                // This is needed to log any errors that occur during the file loading process.
                _logger.LogError("Error loading file: {Message}", ex.Message);
                throw;
            }
        }
    }

    public class FileUploadResponse
    {
        public string ApiResponse { get; set; }

        [JsonPropertyName("data")]
        public List<FileInfoData> Data { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonConstructor]
        public FileUploadResponse(string apiResponse, List<FileInfoData> data, bool success)
        {
            ApiResponse = apiResponse;
            Data = data;
            Success = success;
        }

        public override string ToString()
        {
            return $"ApiResponse: {ApiResponse}, Success: {Success}, Data: [{string.Join(", ", Data)}]";
        }
    }

    public class FileInfoData
    {
        [JsonPropertyName("fileId")]
        public string FileId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("description")]
        public int Description { get; set; }

        public override string ToString()
        {
            return $"FileId: {FileId}, Name: {Name}, Size: {Size}, Description: {Description}";
        }
    }

}
