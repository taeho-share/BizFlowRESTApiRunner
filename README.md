# BizFlow REST API Runner

by **Taeho Lee** (Feb 21st 2025).

## Overview

**BizFlow REST API Runner** is a powerful, console-based application designed to streamline interactions with BizFlow REST APIs. It offers a user-friendly command-line interface for essential operations, including authentication, file management, process handling, and more. This tool simplifies API communication, enhancing workflow efficiency.

## Features

- **Authentication**: Secure login and logout functionality.
- **File Management**: Upload, download, and organize files.
- **Process Handling**: Initiate, retrieve, and manage processes.
- **Comment Management**: Add, list, and delete comments.
- **Folder Management**: Retrieve and manage folder details.
- **Member Management**: Fetch and list user details.
- **Map View**: Access and display map-related data.
- **Workitem Management**: Check-in, check-out, complete, and reject workitems.
- **Error Code Reference**: Access structured lists of error codes.

## Installation

### Prerequisites
Ensure the following dependencies are installed:
- **.NET 9**
- **Visual Studio** (or any compatible IDE)
- **Windows (Recommended OS)

### Installation Steps

1. **Clone the repository:**
   ```sh
   git clone https://github.com/yourusername/BizFlowRESTApiRunner.git
   ```
2. **Navigate to the project directory:**
   ```sh
   cd BizFlowRESTApiRunner
   ```
3. **Build the project:**
   ```sh
   dotnet build
   ```

## Configuration

Create an `appsettings.json` file in the root directory with the following structure:

```json
{
  "Authentication": {
    "LoginId": "your_login_id",
    "Password": "your_password"
  },
  "Network": {
    "ServerKey": "your_server_key",
    "Endpoint": "your_api_endpoint",
    "EndpointFileUpload": "your_file_upload_endpoint"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "General": {
    "ShowLogOnConsole": "true"
  }
}
```

## Usage

### Running the Application
To start the application, execute:
```sh
dotnet run
```
Follow the on-screen instructions to navigate through the main menu and execute various operations.

### Available Commands
- **Activity**: Complete an activity.
- **Attachment**: Manage attachments (list, download, upload, delete).
- **Authentication**: Perform login and logout operations.
- **BizCove**: Access BizCove contents.
- **Comment**: Add, list, and delete comments.
- **File**: Upload and manage files.
- **Folder**: Retrieve folder details.
- **Member**: Fetch and list user details.
- **Map**: Display map data.
- **Process**: Initiate, manage, and retrieve process details.
- **Process Definition**: Get, list, and start process definitions.
- **Process Variable**: List and update process variables.
- **Response**: Retrieve API responses.
- **Workitem**: Manage workitems (check-in, check-out, complete, reject).
- **Reference**: Access error codes.
- **Toggle API Call Mode**: Switch between synchronous and asynchronous API calls.
- **Exit**: Close the application.

## Example Code

### Authentication Example

```csharp
/// <summary>
/// Logs in to the BizFlow API synchronously.
/// </summary>
/// <param name="loginId">The login ID.</param>
/// <param name="password">The password.</param>
/// <returns>The response from the API as a string.</returns>
public string Login(string loginId, string password)
{
    _logger.LogInformation("API [Authentication-Login]");

    _apiRequestHandler.SetSessionKey(null); // Reset session key
    _apiRequestHandler.ClearAllCookies(); // Clear cookies for a new session

    string response = null;
    try
    {
        var payload = new
        {
            @object = "authentication",
            action = "login",
            query = new
            {
                id = loginId,
                pwd = password
            }
        };

        response = _apiRequestHandler.SendApiRequest(payload);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred during authentication.");
    }

    return response;
}
```

## Development Environment

- **Framework**: .NET 9
- **IDE**: Visual Studio 2022
- **OS**: Windows 10/11

## Roadmap & Future Enhancements
This is a beta release. Upcoming updates will include:
- Improved error handling and logging.
- Additional API usage examples.
- Performance optimizations.

## Contributing

We welcome contributions! To contribute:
1. Fork the repository.
2. Create a feature branch.
3. Submit a pull request with your changes.

For significant modifications, please open an issue to discuss your proposed updates before submitting a pull request.

## License

This software is licensed under the Mozilla Public License (MPL) 2.0.
Modification and distribution are allowed, but commercial use and resale are prohibited.


