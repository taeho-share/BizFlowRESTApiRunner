using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BizFlowRESTapiRunner
{
    class CommentAPI
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly ApiRequestHandler _apiRequestHandler;

        public CommentAPI(ILogger logger, IConfigurationRoot config, ApiRequestHandler apiRequestHandler)
        {
            this._logger = logger;
            this._config = config;
            this._apiRequestHandler = apiRequestHandler;
        }

        public string CommentList(int processId)
        {
            _logger.LogInformation("API [Comment-List]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "comment",
                    action = "list",
                    query = new
                    {
                        processId = processId
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

        public async Task<string> CommentListAsync(int processId)
        {
            _logger.LogInformation("API [Comment-List-Async]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "comment",
                    action = "list",
                    query = new
                    {
                        processId = processId
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


        public string CommentPut(int processId, int workitemSeq, int commentId, string comment)
        {
            _logger.LogInformation("API [Comment-Put]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "comment",
                    action = "put",
                    query = new
                    {
                        processId = processId,
                        workitemId = workitemSeq,
                        id = commentId,
                        comment = comment
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

        public async Task<string> CommentPutAsync(int processId, int workitemSeq, int commentId, string comment)
        {
            _logger.LogInformation("API [Comment-Put-Async]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "comment",
                    action = "put",
                    query = new
                    {
                        processId = processId,
                        workitemId = workitemSeq,
                        id = commentId,
                        comment = comment
                    }
                };

                // Send the API request and get the response
                response = await _apiRequestHandler.SendApiRequestAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication.");
            }

            return response;
        }


        public string CommentDelete(int processId, int commentId)
        {
            _logger.LogInformation("API [Comment-Delete]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "comment",
                    action = "delete",
                    query = new
                    {
                        processId = processId,
                        id = commentId
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

        public async Task<string> CommentDeleteAsync(int processId, int commentId)
        {
            _logger.LogInformation("API [Comment-Delete-Async]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "comment",
                    action = "delete",
                    query = new
                    {
                        processId = processId,
                        id = commentId
                    }
                };

                // Send the API request and get the response
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
