BizFlow REST API Runner



Overview

BizFlow REST API Runner is a robust, console-based application designed to facilitate seamless interactions with BizFlow REST APIs. It provides an intuitive command-line interface for performing essential operations such as authentication, file management, process handling, and more. This tool streamlines API requests, making it easier to integrate BizFlow capabilities into your workflow.

Key Features

Authentication: Secure login and logout functionality.

File Management: Upload, download, and organize files efficiently.

Process Handling: Initiate, retrieve, and manage processes.

Comment Management: Add, list, and delete comments.

Folder Management: Retrieve folder details and structure.

Member Management: Fetch and list user details.

Map View: Access and display map-related data.

Workitem Management: Perform check-in, check-out, completion, and rejection of workitems.

Error Code Reference: Easily access a structured list of error codes.

Installation

Prerequisites

Ensure you have the following installed before proceeding:

.NET 9 (or latest compatible version)

Visual Studio (or any compatible IDE)

Windows 10/11 (Recommended OS)

Steps to Install

Clone the repository:

git clone https://github.com/yourusername/BizFlowRESTApiRunner.git

Navigate to the project directory:

cd BizFlowRESTApiRunner

Build the project:

dotnet build

Configuration

Create an appsettings.json file in the project root directory with the following structure:

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

Usage

Running the Application

To start the application, execute:

dotnet run

Follow the on-screen instructions to navigate through the main menu and execute various operations.

Main Menu Options

Activity: Complete an activity.

Attachment: Manage attachments (list, download, upload, delete).

Authentication: Perform login and logout.

BizCove: View BizCove contents.

Comment: Add, list, and delete comments.

File: Upload and manage files.

Folder: Retrieve folder details.

Member: Fetch and list user details.

Map: Display map data.

Process: Initiate, manage, and retrieve process details.

Process Definition: Get, list, and start process definitions.

Process Variable: List and update process variables.

Response: Retrieve API responses.

Workitem: Perform workitem operations (check-in, check-out, complete, reject).

Reference: Access error codes.

Toggle API Call Mode: Switch between synchronous and asynchronous API calls.

Exit: Close the application.

Example Code

Authentication Example

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

Development Environment

Framework: .NET 9

IDE: Visual Studio

OS: Windows 10/11

Future Enhancements

This is a beta version. Future updates will include:

Enhanced error handling and logging

Additional API examples

Performance optimizations

Contributing

Contributions are welcome! To contribute:

Fork the repository.

Create a feature branch.

Submit a pull request with your changes.

For major changes, please open an issue first to discuss your proposed updates.

License

This project is licensed under the MIT License. See the LICENSE file for more details.

Acknowledgments

Developed by Taeho Lee (Feb 2025).

