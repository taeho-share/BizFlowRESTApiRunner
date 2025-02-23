using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BizFlowRESTapiRunner
{
    class FolderAPI
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly ApiRequestHandler _apiRequestHandler;

        public FolderAPI(ILogger logger, IConfigurationRoot config, ApiRequestHandler apiRequestHandler)
        {
            this._logger = logger;
            this._config = config;
            this._apiRequestHandler = apiRequestHandler;
        }

        public string FolderGet(int folderId)
        {
            _logger.LogInformation("API [FolderGet]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "folder",
                    action = "get",
                    query = new
                    {
                        id = folderId
                    }
                };

                // Send the API request and get the response
                response = _apiRequestHandler.SendApiRequest(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during folder get.");
            }

            return response;
        }

        public async Task<string> FolderGetAsync(int folderId)
        {
            _logger.LogInformation("API [FolderGet-Async]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "folder",
                    action = "get",
                    query = new
                    {
                        id = folderId
                    }
                };

                // Send the API request asynchronously and get the response
                response = await _apiRequestHandler.SendApiRequestAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during folder get.");
            }

            return response;
        }

        // Note: Not commonly used in real-world scenarios. It is just for internal BizFlow product development.
        // Especially, the prjId parameter is no use to develop a real-world application.
        public string FolderList(string category, int projectId)
        {
            _logger.LogInformation("API [FolderList]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "folder",
                    action = "list",
                    query = new
                    {
                        category = category,
                        prjId = (projectId > 0) ? (int?)projectId : null
                    }
                };

                /*
                    "all" : All folders
                    "application" : Application folder
                    "archive" : Archive folder (not applicable for project)
                    "definition" : Process definition folder
                    "globalvariable" : Global variable folder (not applicable for project)
                    "instance" : Process instance folder (not applicable for project)
                 */

                // Send the API request and get the response
                response = _apiRequestHandler.SendApiRequest(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during folder list.");
            }

            return response;
        }

        public async Task<string> FolderListAsync(string category, int projectId)
        {
            _logger.LogInformation("API [FolderList-Async]");
            string response = null;

            try
            {
                var payload = new
                {
                    @object = "folder",
                    action = "list",
                    query = new
                    {
                        category = category,
                        prjId = (projectId > 0) ? (int?)projectId : null
                    }
                };

                /*
                    "all" : All folders
                    "application" : Application folder
                    "archive" : Archive folder (not applicable for project)
                    "definition" : Process definition folder
                    "globalvariable" : Global variable folder (not applicable for project)
                    "instance" : Process instance folder (not applicable for project)
                 */

                // Send the API request and get the response
                response = await _apiRequestHandler.SendApiRequestAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during folder list.");
            }

            return response;
        }
    }
}
