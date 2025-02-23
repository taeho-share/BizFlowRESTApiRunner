using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BizFlowRESTapiRunner
{
    class ProcessDefinitionAPI
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly ApiRequestHandler _apiRequestHandler;

        public ProcessDefinitionAPI(ILogger logger, IConfigurationRoot config, ApiRequestHandler apiRequestHandler)
        {
            this._logger = logger;
            this._config = config;
            this._apiRequestHandler = apiRequestHandler;
        }

        public string ProcessDefinitionGet(int processDefinitionId)
        {
            _logger.LogInformation("API [ProcessDefinition-Get]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "process_definition",
                    action = "get",
                    query = new
                    {
                        id = processDefinitionId
                    }
                };

                // Send the API request and get the response
                response = _apiRequestHandler.SendApiRequest(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during ProcessDefinitionGet.");
            }

            return response;
        }

        public async Task<string> ProcessDefinitionGetAsync(int processDefinitionId)
        {
            _logger.LogInformation("API [ProcessDefinition-Get-Async]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "process_definition",
                    action = "get",
                    query = new
                    {
                        id = processDefinitionId
                    }
                };

                // Send the API request asynchronously and get the response
                response = await _apiRequestHandler.SendApiRequestAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during ProcessDefinitionGetAsync.");
            }

            return response;
        }

        public string ProcessDefinitionList(int processDefinitionFolderId)
        {
            _logger.LogInformation("API [ProcessDefinition-List]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "process_definition",
                    action = "list",
                    query = new
                    {
                        id = processDefinitionFolderId
                    }
                };

                // Send the API request and get the response
                response = _apiRequestHandler.SendApiRequest(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during ProcessDefinitionList.");
            }

            return response;
        }

        public async Task<string> ProcessDefinitionListAsync(int processDefinitionFolderId)
        {
            _logger.LogInformation("API [ProcessDefinition-List-Async]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "process_definition",
                    action = "list",
                    query = new
                    {
                        id = processDefinitionFolderId
                    }
                };

                // Send the API request asynchronously and get the response
                response = await _apiRequestHandler.SendApiRequestAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during ProcessDefinitionListAsync.");
            }

            return response;
        }

        public string ProcessDefinitionInitiate(int processDefinitionId, 
                                                    bool returnWorkitemId, 
                                                    string startActivityName,
                                                    string description,
                                                    Object[] variables,
                                                    List<FileInfoData> attachments)
        {
            _logger.LogInformation("API [ProcessDefinition-Initiate]");
            string response = null;

            //convert attachments to array of objects
            var attachmentsArray = attachments.Select(attachment => new
            {
                fileId = attachment.FileId,
                name = attachment.Name,
                description = ""
            }).ToArray();

            try
            {
                var payload = new
                {
                    @object = "process_definition",
                    action = "initiate",
                    query = new
                    {
                        id = processDefinitionId,
                        returnWorkitemInfo = returnWorkitemId,
                        startActivityName = String.IsNullOrEmpty(startActivityName) ? null : startActivityName,
                        description = String.IsNullOrEmpty(description) ? null : description,
                        variables = (variables != null && variables.Length > 0) ? variables : null,
                        attachments = (attachmentsArray != null && attachmentsArray.Length > 0) ? attachmentsArray : null
                    }
                };

                // Send the API request and get the response
                response = _apiRequestHandler.SendApiRequest(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during ProcessDefinitionInitiate.");
            }

            return response;
        }

        public async Task<string> ProcessDefinitionInitiateAsync(int processDefinitionId,
                                                                    bool returnWorkitemId,
                                                                    string startActivityName,
                                                                    string description,
                                                                    Object[] variables,
                                                                    List<FileInfoData> attachments)
        {
            _logger.LogInformation("API [ProcessDefinition-Initiate-Async]");
            string response = null;

            //convert attachments to array of objects
            var attachmentsArray = attachments.Select(attachment => new
            {
                fileId = attachment.FileId,
                name = attachment.Name,
                description = ""
            }).ToArray();

            try
            {
                var payload = new
                {
                    @object = "process_definition",
                    action = "initiate",
                    query = new
                    {
                        id = processDefinitionId,
                        returnWorkitemInfo = returnWorkitemId,
                        startActivityName = String.IsNullOrEmpty(startActivityName) ? null : startActivityName,
                        description = String.IsNullOrEmpty(description) ? null : description,
                        variables = (variables != null && variables.Length > 0) ? variables : null,
                        attachments = (attachmentsArray != null && attachmentsArray.Length > 0) ? attachmentsArray : null
                    }
                };

                // Send the API request and get the response
                response = await _apiRequestHandler.SendApiRequestAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during ProcessDefinitionInitiate.");
            }

            return response;
        }
    }
}
