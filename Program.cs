using BizFlowRESTapiRunner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    private static string _sessionKey;
    private static ILogger _logger;
    private static IConfigurationRoot _config;
    private static string _defaultLoginId;
    private static string _defaultPassword;
    private static ApiRequestHandler _apiRequestHandler;
    private static string _requestMode = "sync";

    static void Main()
    {
        // Fix for IDE0090: 'new' expression can be simplified
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        
        _config = builder.Build();

        _defaultLoginId = _config["Authentication:LoginId"];
        _defaultPassword = _config["Authentication:Password"];

        //configure logging
        var loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(_config.GetSection("Logging"));           
            loggingBuilder.AddDebug();
            string showLogOnConsole = _config["General:ShowLogOnConsole"];
            if (string.Equals(showLogOnConsole, "true", StringComparison.OrdinalIgnoreCase))
            {
                loggingBuilder.AddConsole();
            }
        });

        _logger = loggerFactory.CreateLogger<Program>();

        // Get the API key and endpoint from the configuration file
        string apiKey = _config["Network:ServerKey"];
        string endpoint = _config["Network:Endpoint"];

        _apiRequestHandler = new ApiRequestHandler(_logger, apiKey, endpoint);

        // Show the animation and main menu
        ShowAnimation();
        ShowMainMenu();
    }
    static void ShowAnimation()
    {
        Console.Clear();
        string[] lines = new string[]
        {
        "BizFlow REST API Runner",
        "Version 1.0.0",
        "Developed by Taeho Lee (Feb 2025)",
        "https://github.com/taeho-share/BizFlowRESTApiRunner",
        "A console-based interface for interacting with BizFlow REST APIs"

        };

        int top = (Console.WindowHeight - lines.Length) / 2;

        foreach (string line in lines)
        {
            int left = (Console.WindowWidth - line.Length) / 2;
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i < line.Length; i++)
            {
                Console.SetCursorPosition(left + i, top);
                Console.Write(line[i]);
                Thread.Sleep(1); // Adjust the speed of the animation here
            }
            top++;
            Thread.Sleep(1); // Pause between lines
        }
        /*
        for (int j = 0; j < 3; j++)
        {
            Console.ForegroundColor = j % 2 == 0 ? ConsoleColor.Yellow : ConsoleColor.Green;
            string loadingText = "Loading...";
            int left = (Console.WindowWidth - loadingText.Length) / 2;
            Console.SetCursorPosition(left, top + 2);
            Console.Write(loadingText);
            Thread.Sleep(5);
            Console.SetCursorPosition(left, top + 2);
            Console.Write(new string(' ', loadingText.Length));
            Thread.Sleep(5);
        }
        */
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition((Console.WindowWidth - "Press Enter to continue...".Length) / 2, top + 4);
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
        Console.Clear();
    }

    static void ShowMainMenu()
    {
        bool exit = false;
        while (!exit)
        {
            //Console.Clear();
            Console.WriteLine("=============================================");
            Console.WriteLine("========== BizFlow REST API Runner ==========");
            Console.WriteLine("==========        MAIN MENU        ==========");
            Console.WriteLine("=============================================");
            Console.WriteLine("");
            Console.WriteLine("[ 1] Activity            [ 2] Attachment");
            Console.WriteLine("[ 3] Authentication      [ 4] BizCove");
            Console.WriteLine("[ 5] Comment             [ 6] File");
            Console.WriteLine("[ 7] Folder              [ 8] Member");
            Console.WriteLine("[ 9] Member_Category     [10] Member_Profile");
            Console.WriteLine("[11] Map                 [12] Process");
            Console.WriteLine("[13] Process Definition  [14] Process Variable");
            Console.WriteLine("[15] Response            [16] Workitem");
            Console.WriteLine("[17] Workitem_Category   [99] Switch Request Mode [Sync|Async]");
            Console.WriteLine("[ 0] Exit");
            Console.WriteLine("");
            Console.Write("Select an option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                HandleMenuSelection(choice, ref exit);
            }
        }
    }

    static void HandleMenuSelection(int choice, ref bool exit)
    {
        switch (choice)
        {
            case 1: ActivityComplete(); break;
            case 2: ShowSubmenu("ATTACHMENT", new Action[] { AttachmentList, AttachmentDownload, AttachmentPut, AttachmentDelete }); break;
            case 3: ShowSubmenu("AUTHENTICATION", new Action[] { AuthenticationLogin, AuthenticationLogout }); break;
            case 4: ShowSubmenu("BIZCOVE", new Action[] { BizcoveContents }); break;
            case 5: ShowSubmenu("COMMENT", new Action[] { CommentList, CommentPut, CommentDelete }); break;
            case 6: ShowSubmenu("FILE", new Action[] { FilePut }); break;
            case 7: ShowSubmenu("FOLDER", new Action[] { FolderGet, FolderList }); break;
            case 8: ShowSubmenu("MEMBER", new Action[] { MemberGet, MemberList }); break;
            case 9: ShowSubmenu("MEMBER_CATEGORY", new Action[] { MemberCategoryList, MemberCategoryPut }); break;
            case 10: ShowSubmenu("MEMBER_PROFILE", new Action[] { MemberProfilePut }); break;
            case 11: ShowSubmenu("MAP", new Action[] { MapView }); break;
            case 12: ShowSubmenu("PROCESS", new Action[] { ProcessActivities, ProcessGet, ProcessHistory, ProcessWorklist, ProcessChangeState, ProcessStart }); break;
            case 13: ShowSubmenu("PROCESS DEFINITION", new Action[] { ProcessDefinitionGet, ProcessDefinitionList, ProcessDefinitionInitiate }); break;
            case 14: ShowSubmenu("PROCESS VARIABLE", new Action[] { ProcessVariableList, ProcessVariablePut }); break;
            case 15: ShowSubmenu("RESPONSE", new Action[] { ResponseList }); break;
            case 16: ShowSubmenu("WORKITEM", new Action[] { WorkitemCheckIn, WorkitemCheckOut, WorkitemGet, WorkitemComplete, WorkitemReject }); break;
            case 17: ShowSubmenu("WORKITEM_CATEGORY", new Action[] { WorkitemCategoryGet, WorkitemCategoryPut }); break;
            case 18: ShowSubmenu("REFERENCE", new Action[] { ErrorCodes }); break;
            case 99: ShowSubmenu("REFERENCE", new Action[] { SetRequestModeToSync, SetRequestModeToAsync }); break;
            case 0: exit = true; break;
            default: Console.WriteLine("Invalid option. Press Enter to try again."); Console.ReadLine(); break;
        }
    }

    static async Task ShowSubmenu(string category, Action[] actions)
    {
        /*
        if (actions.Length == 1)
        {
            actions[0].Invoke();
            return;
        }
        */
        bool backToMain = false;
        while (!backToMain)
        {
            //Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"=== {category} ===");
            for (int i = 0; i < actions.Length; i++)
            {
                //Console.WriteLine($"{i + 1}. {actions[i].Method.Name}");
                //Console.WriteLine($"[{(i + 1):D2}] {actions[i].Method.Name}");
                Console.WriteLine($"[{(i + 1).ToString().PadLeft(2, ' ')}] {actions[i].Method.Name}");
            }
            Console.WriteLine("[ 0] Back to Main Menu");
            Console.WriteLine("");
            Console.Write("Select an option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == 0)
                {
                    return; // backToMain = true;
                }
                else if (choice > 0 && choice <= actions.Length)
                {
                    actions[choice - 1].Invoke();
                }
                else
                {
                    Console.WriteLine("Invalid selection. Press Enter to try again.");
                    Console.ReadLine();
                }
            }
        }
    }

    /* 
        API Request Methods (Submenu Action Handler Methods) 
        These methods receive user input, call the API, and display the response.
        They can be synchronous or asynchronous based on the request mode (submenu 99, default is sync). 
     */
    static async void ActivityComplete() { Console.WriteLine("Executing Activity Complete..."); Console.ReadLine(); }
    static async void AttachmentList() { Console.WriteLine("Executing Attachment List..."); Console.ReadLine(); }
    static async void AttachmentDownload() { Console.WriteLine("Executing Attachment Download..."); Console.ReadLine(); }
    static async void AttachmentPut() { Console.WriteLine("Executing Attachment Put..."); Console.ReadLine(); }
    static async void AttachmentDelete() { Console.WriteLine("Executing Attachment Delete..."); Console.ReadLine(); }
    static async void AuthenticationLogin()
    {
        CommonUtils.WriteColoredLine($"Executing AuthenticationLogin-{_requestMode}", ConsoleColor.Blue);

        Console.Write($"Enter login id (default: {_defaultLoginId})$");
        string loginId = Console.ReadLine();
        loginId = string.IsNullOrEmpty(loginId) ? _defaultLoginId : loginId;

        Console.Write($"Enter Password (default: {_defaultPassword})$");
        string password = CommonUtils.ReadPassword();
        password = string.IsNullOrEmpty(password) ? _defaultPassword : password;

        AuthenticationAPI authenticationAPI = new AuthenticationAPI(_logger, _config, _apiRequestHandler);

        string response = "{}";
        if (_requestMode != "async") {
            response = authenticationAPI.Login(loginId, password);
        } else {
            response = await authenticationAPI.LoginAsync(loginId, password);
        }

        //Get the session key from the response, and store it for future API requests
        _sessionKey = CommonUtils.ParseResponseForSessionKey(response);
        _apiRequestHandler.SetSessionKey(_sessionKey);

        _logger.LogInformation("Response: {Response}", response);
    }
    static async void AuthenticationLogout() {
        CommonUtils.WriteColoredLine($"Executing AuthenticationLogout-{_requestMode}", ConsoleColor.Blue);

        AuthenticationAPI authenticationAPI = new AuthenticationAPI(_logger, _config, _apiRequestHandler);

        string response = "{}";
        if (_requestMode != "async")
        {
            response = authenticationAPI.Logout();
        }
        else
        {
            response = await authenticationAPI.LogoutAsync();
        }

        _logger.LogInformation("Response: {Response}", response);
    }
    static async void BizcoveContents() { Console.WriteLine("Executing Bizcove Contents..."); Console.ReadLine(); }
    static async void CommentList() {

        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        CommonUtils.WriteColoredLine($"Executing CommentList-{_requestMode}", ConsoleColor.Blue);

        CommentAPI commentAPI = new CommentAPI(_logger, _config, _apiRequestHandler);

        Console.Write($"Enter process id:$");
        string processIdInput = CommonUtils.ConsoleReadLineEx();
        int processId = int.Parse(processIdInput);

        string response = "{}";
        if (_requestMode != "async")
        {
            response = commentAPI.CommentList(processId);
        }
        else
        {
            response = await commentAPI.CommentListAsync(processId);
        }

        _logger.LogInformation("Response: {Response}", response);
    }
    static async void CommentPut() {

        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        CommonUtils.WriteColoredLine($"Executing CommentPut-{_requestMode}", ConsoleColor.Blue);

        //request input values from the user
        Console.Write($"Enter process id:$");
        string processIdInput = CommonUtils.ConsoleReadLineEx();
        int processId = int.Parse(processIdInput);

        Console.Write($"Enter workitem sequence:$");
        string workitemIdInput = CommonUtils.ConsoleReadLineEx();
        int workitemId = int.Parse(workitemIdInput);

        Console.Write($"Enter comment id (set 0 to add a new comment, default 0):$");
        string commentIdInput = Console.ReadLine();
        if (string.IsNullOrEmpty(commentIdInput))
        {
            commentIdInput = "0";
        }
        int commentId = int.Parse(commentIdInput);

        Console.Write($"Enter comment text:$");
        string commentText = CommonUtils.ConsoleReadLineEx();

        string response = "{}";

        //call the API
        CommentAPI commentAPI = new CommentAPI(_logger, _config, _apiRequestHandler);
        if (_requestMode != "async")
        {
            response = commentAPI.CommentPut(processId, workitemId, commentId, commentText);
        }
        else
        {
            response = await commentAPI.CommentPutAsync(processId, workitemId, commentId, commentText);
        }

        _logger.LogInformation("Response: {Response}", response);

    }
    static async void CommentDelete() {
        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        CommonUtils.WriteColoredLine($"Executing CommentDelete-{_requestMode}", ConsoleColor.Blue);

        //request input values from the user
        Console.Write($"Enter process id:$");
        string processIdInput = CommonUtils.ConsoleReadLineEx();
        int processId = int.Parse(processIdInput);

        Console.Write($"Enter comment id:$");
        string commentIdInput = CommonUtils.ConsoleReadLineEx();
        int commentId = int.Parse(commentIdInput);

        string response = "{}";

        //call the API
        CommentAPI commentAPI = new CommentAPI(_logger, _config, _apiRequestHandler);
        if (_requestMode != "async")
        {
            response = commentAPI.CommentDelete(processId, commentId);
        }
        else
        {
            response = await commentAPI.CommentDeleteAsync(processId, commentId);
        }

        _logger.LogInformation("Response: {Response}", response);
    }
    static async void FilePut() {

        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        FileUploadHandler fileUploader = new FileUploadHandler(_logger);
        string fileUploadEndpoint = _config["Network:EndpointFileUpload"];
        string defaultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "hello.txt");

        Console.Write($"Enter file full path (default {defaultFilePath})$");
        
        string filePathInput = Console.ReadLine();
        string filePath = string.IsNullOrEmpty(filePathInput) ? defaultFilePath : filePathInput;

        string response = "{}";
        if (_requestMode != "async")
        {
            FileUploadResponse fileInfoData = fileUploader.UploadFile(fileUploadEndpoint, _sessionKey, filePath);
            CommonUtils.WriteColoredLine(fileInfoData.ApiResponse, ConsoleColor.Green);
            FileInfoData fileInfo = fileInfoData.Data[0];
        }
        else
        {
            FileUploadResponse fileInfoData = await fileUploader.UploadFileAsync(fileUploadEndpoint, _sessionKey, filePath);
            CommonUtils.WriteColoredLine(fileInfoData.ApiResponse, ConsoleColor.Green);
            FileInfoData fileInfo = fileInfoData.Data[0];
        }


        //return fileInfo;

    }
    static async void FolderGet() {
        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        CommonUtils.WriteColoredLine($"Executing FolderGet-{_requestMode}", ConsoleColor.Blue);

        FolderAPI folderAPI = new FolderAPI(_logger, _config, _apiRequestHandler);

        Console.Write($"Enter folder id:$");
        string folderIdInput = CommonUtils.ConsoleReadLineEx();
        int folderId = int.Parse(folderIdInput);

        string response = "{}";
        if (_requestMode != "async")
        {
            response = folderAPI.FolderGet(folderId);
        }
        else
        {
            response = await folderAPI.FolderGetAsync(folderId);
        }

        _logger.LogInformation("Response: {Response}", response);

    }
    static async void FolderList() {
        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        CommonUtils.WriteColoredLine($"Executing FolderList-{_requestMode}", ConsoleColor.Blue);

        FolderAPI folderAPI = new FolderAPI(_logger, _config, _apiRequestHandler);
        bool exit = false;
        string folderCategory = "all";

        while (!exit)
        {
            Console.WriteLine("Folder Category ----");
            Console.WriteLine("[1] all (default)");
            Console.WriteLine("[2] application");
            Console.WriteLine("[3] archive");
            Console.WriteLine("[4] definition");
            Console.WriteLine("[5] globalvariable");
            Console.WriteLine("[6] instance");
            Console.WriteLine("");
            Console.Write($"Enter folder category:$");
            
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice) 
                {
                    case 1: folderCategory = "all"; exit = true; break;
                    case 2: folderCategory = "application"; exit = true; break;
                    case 3: folderCategory = "archive"; exit = true; break;
                    case 4: folderCategory = "definition"; exit = true; break;
                    case 5: folderCategory = "globalvariable"; exit = true; break;
                    case 6: folderCategory = "instance"; exit = true; break;
                    default: Console.WriteLine("Invalid option. Press Enter to try again."); exit = false; Console.ReadLine(); break;
                }
            }
        }
        Console.WriteLine($"Selected folder category: {folderCategory}");

        Console.Write($"Enter project folder id. it is not common to use this folder (default null):$");
        string projectFolderIdInput = Console.ReadLine();
        int projectFolderId = 0;
        if (!string.IsNullOrEmpty(projectFolderIdInput))
        {
            projectFolderId = int.Parse(projectFolderIdInput);
        }

        string response = "{}";
        if (_requestMode != "async")
        {
            response = folderAPI.FolderList(folderCategory, projectFolderId);
        }
        else
        {
            response = await folderAPI.FolderListAsync(folderCategory, projectFolderId);
        }

        _logger.LogInformation("Response: {Response}", response);
    }
    static async void MemberGet() { Console.WriteLine("Executing Member Get..."); Console.ReadLine(); }
    static async void MemberList() { Console.WriteLine("Executing Member List..."); Console.ReadLine(); }
    static async void MemberCategoryList() { Console.WriteLine("Executing Member Category List..."); Console.ReadLine(); }
    static async void MemberCategoryPut() { Console.WriteLine("Executing Member Category Put..."); Console.ReadLine(); }
    static async void MemberProfilePut() { Console.WriteLine("Executing Member Profile Put..."); Console.ReadLine(); }
    static async void MapView() { Console.WriteLine("Executing Map View..."); Console.ReadLine(); }
    static async void ProcessActivities() { Console.WriteLine("Executing Process Activities..."); Console.ReadLine(); }
    static async void ProcessGet() { Console.WriteLine("Executing Process Get..."); Console.ReadLine(); }
    static async void ProcessHistory() { Console.WriteLine("Executing Process History..."); Console.ReadLine(); }
    static async void ProcessWorklist() { Console.WriteLine("Executing Process Worklist..."); Console.ReadLine(); }
    static async void ProcessChangeState() { Console.WriteLine("Executing Process Change State..."); Console.ReadLine(); }
    static async void ProcessStart() { Console.WriteLine("Executing Process Start..."); Console.ReadLine(); }
    static async void ProcessDefinitionGet() {

        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        CommonUtils.WriteColoredLine($"Executing ProcessDefinitionGet-{_requestMode}", ConsoleColor.Blue);

        ProcessDefinitionAPI processDefinitionAPI = new ProcessDefinitionAPI(_logger, _config, _apiRequestHandler);

        Console.Write($"Enter process definition id:$");
        string procDefIdInput = CommonUtils.ConsoleReadLineEx();
        int procDefId = int.Parse(procDefIdInput);

        string response = "{}";
        if (_requestMode != "async")
        {
            response = processDefinitionAPI.ProcessDefinitionGet(procDefId);
        }
        else
        {
            response = await processDefinitionAPI.ProcessDefinitionGetAsync(procDefId);
        }

        _logger.LogInformation("Response: {Response}", response);

    }
    static async void ProcessDefinitionList() {

        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        CommonUtils.WriteColoredLine($"Executing ProcessDefinitionList-{_requestMode}", ConsoleColor.Blue);

        ProcessDefinitionAPI processDefinitionAPI = new ProcessDefinitionAPI(_logger, _config, _apiRequestHandler);

        Console.Write($"Enter process definition folder id:$");
        string procDefFolderIdInput = CommonUtils.ConsoleReadLineEx();
        int procDefFolderId = int.Parse(procDefFolderIdInput);

        string response = "{}";
        if (_requestMode != "async")
        {
            response = processDefinitionAPI.ProcessDefinitionList(procDefFolderId);
        }
        else
        {
            response = await processDefinitionAPI.ProcessDefinitionListAsync(procDefFolderId);
        }

        _logger.LogInformation("Response: {Response}", response);

    }
    static async void ProcessDefinitionInitiate()
    {

        if (_sessionKey == null)
        {
            Console.WriteLine("Please login first.");
            AuthenticationLogin();
        }

        CommonUtils.WriteColoredLine($"Executing ProcessDefinitionInitiate-{_requestMode}", ConsoleColor.Blue);

        ProcessDefinitionAPI processDefinitionAPI = new ProcessDefinitionAPI(_logger, _config, _apiRequestHandler);

        //request input values from the user --------------------------------
        Console.Write($"Enter process definition id:$");
        string procDefIdInput = CommonUtils.ConsoleReadLineEx();
        int procDefId = int.Parse(procDefIdInput);

        Console.Write($"Include work item information in the response? (y/n, default y):$");
        string returnWorkitemInfoInput = Console.ReadLine();
        if (string.IsNullOrEmpty(returnWorkitemInfoInput))
        {
            returnWorkitemInfoInput = "y";
        }
        bool returnWorkitemInfo = returnWorkitemInfoInput.Equals("y", StringComparison.OrdinalIgnoreCase);
 

        Console.Write($"Enter description:$");
        string procDefDescInput = CommonUtils.ConsoleReadLineEx();

        Console.Write($"Enter start activity name (leave blank to use the default start activity):$");
        string startActivityName = Console.ReadLine().Trim();

        Console.Write($"Do you want to set process variable (y or n, default n):$");
        string setProcessVariables = Console.ReadLine().Trim();

        Object[] processVariables = null;
        if (!string.IsNullOrEmpty(setProcessVariables) && setProcessVariables == "y")
        {
            Dictionary<string, string> processVariableInputs = CommonUtils.ConsoleReadProcessVariables();
            processVariables = CommonUtils.ConvertToProcessVariable(processVariableInputs);
        }

        List<FileInfoData> attachments = CommonUtils.ConsoleReadFileUploads(_logger, _config, _sessionKey);

        //call the API ------------------------------------------------------
        string response = "{}";
        if (_requestMode != "async")
        {
            response = processDefinitionAPI.ProcessDefinitionInitiate(procDefId, returnWorkitemInfo, startActivityName, procDefDescInput, processVariables, attachments);
        }
        else
        {
            response = await processDefinitionAPI.ProcessDefinitionInitiateAsync(procDefId, returnWorkitemInfo, startActivityName, procDefDescInput, processVariables, attachments);
        }

        _logger.LogInformation("Response: {Response}", response);

    }
    static async void ProcessVariableList() { Console.WriteLine("Executing Process Variable List..."); Console.ReadLine(); }
    static async void ProcessVariablePut() { Console.WriteLine("Executing Process Variable Put..."); Console.ReadLine(); }
    static async void ResponseList() { Console.WriteLine("Executing Response List..."); Console.ReadLine(); }
    static async void WorkitemCheckIn() { Console.WriteLine("Executing Workitem Check In..."); Console.ReadLine(); }
    static async void WorkitemCheckOut() { Console.WriteLine("Executing Workitem Check Out..."); Console.ReadLine(); }
    static async void WorkitemGet() { Console.WriteLine("Executing Workitem Get..."); Console.ReadLine(); }
    static async void WorkitemComplete() { Console.WriteLine("Executing Workitem Complete..."); Console.ReadLine(); }
    static async void WorkitemReject() { Console.WriteLine("Executing Workitem Reject..."); Console.ReadLine(); }
    static async void WorkitemCategoryGet() { Console.WriteLine("Executing Workitem Category Get..."); Console.ReadLine(); }
    static async void WorkitemCategoryPut() { Console.WriteLine("Executing Workitem Category Put..."); Console.ReadLine(); }
    static async void ErrorCodes() { Console.WriteLine("Displaying Error Codes..."); Console.ReadLine(); }

    static void SetRequestModeToSync() { _requestMode = "sync"; }
    static void SetRequestModeToAsync() { _requestMode = "async"; }
}
