using System;
using System.Threading;

class Program
{
    static void Main()
    {
        ShowAnimation();
        ShowMainMenu();
    }
    static void ShowAnimation()
    {
        Console.Clear();
        string[] lines = new string[]
        {
        "BizFlow REST API Runner",
        "Developed by Taeho Lee (Feb 2025)",
        "Version 1.0.0",
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
                Thread.Sleep(50); // Adjust the speed of the animation here
            }
            top++;
            Thread.Sleep(500); // Pause between lines
        }

        for (int j = 0; j < 3; j++)
        {
            Console.ForegroundColor = j % 2 == 0 ? ConsoleColor.Yellow : ConsoleColor.Green;
            string loadingText = "Loading...";
            int left = (Console.WindowWidth - loadingText.Length) / 2;
            Console.SetCursorPosition(left, top + 2);
            Console.Write(loadingText);
            Thread.Sleep(500);
            Console.SetCursorPosition(left, top + 2);
            Console.Write(new string(' ', loadingText.Length));
            Thread.Sleep(500);
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition((Console.WindowWidth - "Press Enter to continue...".Length) / 2, top + 4);
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    static void ShowMainMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("BizFlow REST API Runner");
            Console.WriteLine("=== MAIN MENU ===");
            Console.WriteLine("1. ACTIVITY");
            Console.WriteLine("2. ATTACHMENT");
            Console.WriteLine("3. AUTHENTICATION");
            Console.WriteLine("4. BIZCOVE");
            Console.WriteLine("5. COMMENT");
            Console.WriteLine("6. FILE");
            Console.WriteLine("7. FOLDER");
            Console.WriteLine("8. MEMBER");
            Console.WriteLine("9. MEMBER_CATEGORY");
            Console.WriteLine("10. MEMBER_PROFILE");
            Console.WriteLine("11. MAP");
            Console.WriteLine("12. PROCESS");
            Console.WriteLine("13. PROCESS DEFINITION");
            Console.WriteLine("14. PROCESS VARIABLE");
            Console.WriteLine("15. RESPONSE");
            Console.WriteLine("16. WORKITEM");
            Console.WriteLine("17. WORKITEM_CATEGORY");
            Console.WriteLine("18. REFERENCE: ERROR CODES");
            Console.WriteLine("99. Exit");
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
            case 99: exit = true; break;
            default: Console.WriteLine("Invalid option. Press Enter to try again."); Console.ReadLine(); break;
        }
    }

    static void ShowSubmenu(string category, Action[] actions)
    {
        if (actions.Length == 1)
        {
            actions[0].Invoke();
            return;
        }

        bool backToMain = false;
        while (!backToMain)
        {
            Console.Clear();
            Console.WriteLine($"=== {category} ===");
            for (int i = 0; i < actions.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {actions[i].Method.Name}");
            }
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("Select an option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == 0)
                {
                    backToMain = true;
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

    static void ActivityComplete() { Console.WriteLine("Executing Activity Complete..."); Console.ReadLine(); }
    static void AttachmentList() { Console.WriteLine("Executing Attachment List..."); Console.ReadLine(); }
    static void AttachmentDownload() { Console.WriteLine("Executing Attachment Download..."); Console.ReadLine(); }
    static void AttachmentPut() { Console.WriteLine("Executing Attachment Put..."); Console.ReadLine(); }
    static void AttachmentDelete() { Console.WriteLine("Executing Attachment Delete..."); Console.ReadLine(); }
    static void AuthenticationLogin() { Console.WriteLine("Executing Authentication Login..."); Console.ReadLine(); }
    static void AuthenticationLogout() { Console.WriteLine("Executing Authentication Logout..."); Console.ReadLine(); }
    static void BizcoveContents() { Console.WriteLine("Executing Bizcove Contents..."); Console.ReadLine(); }
    static void CommentList() { Console.WriteLine("Executing Comment List..."); Console.ReadLine(); }
    static void CommentPut() { Console.WriteLine("Executing Comment Put..."); Console.ReadLine(); }
    static void CommentDelete() { Console.WriteLine("Executing Comment Delete..."); Console.ReadLine(); }
    static void FilePut() { Console.WriteLine("Executing File Put..."); Console.ReadLine(); }
    static void FolderGet() { Console.WriteLine("Executing Folder Get..."); Console.ReadLine(); }
    static void FolderList() { Console.WriteLine("Executing Folder List..."); Console.ReadLine(); }
    static void MemberGet() { Console.WriteLine("Executing Member Get..."); Console.ReadLine(); }
    static void MemberList() { Console.WriteLine("Executing Member List..."); Console.ReadLine(); }
    static void MemberCategoryList() { Console.WriteLine("Executing Member Category List..."); Console.ReadLine(); }
    static void MemberCategoryPut() { Console.WriteLine("Executing Member Category Put..."); Console.ReadLine(); }
    static void MemberProfilePut() { Console.WriteLine("Executing Member Profile Put..."); Console.ReadLine(); }
    static void MapView() { Console.WriteLine("Executing Map View..."); Console.ReadLine(); }
    static void ProcessActivities() { Console.WriteLine("Executing Process Activities..."); Console.ReadLine(); }
    static void ProcessGet() { Console.WriteLine("Executing Process Get..."); Console.ReadLine(); }
    static void ProcessHistory() { Console.WriteLine("Executing Process History..."); Console.ReadLine(); }
    static void ProcessWorklist() { Console.WriteLine("Executing Process Worklist..."); Console.ReadLine(); }
    static void ProcessChangeState() { Console.WriteLine("Executing Process Change State..."); Console.ReadLine(); }
    static void ProcessStart() { Console.WriteLine("Executing Process Start..."); Console.ReadLine(); }
    static void ProcessDefinitionGet() { Console.WriteLine("Executing Process Definition Get..."); Console.ReadLine(); }
    static void ProcessDefinitionList() { Console.WriteLine("Executing Process Definition List..."); Console.ReadLine(); }
    static void ProcessDefinitionInitiate() { Console.WriteLine("Executing Process Definition Initiate..."); Console.ReadLine(); }
    static void ProcessVariableList() { Console.WriteLine("Executing Process Variable List..."); Console.ReadLine(); }
    static void ProcessVariablePut() { Console.WriteLine("Executing Process Variable Put..."); Console.ReadLine(); }
    static void ResponseList() { Console.WriteLine("Executing Response List..."); Console.ReadLine(); }
    static void WorkitemCheckIn() { Console.WriteLine("Executing Workitem Check In..."); Console.ReadLine(); }
    static void WorkitemCheckOut() { Console.WriteLine("Executing Workitem Check Out..."); Console.ReadLine(); }
    static void WorkitemGet() { Console.WriteLine("Executing Workitem Get..."); Console.ReadLine(); }
    static void WorkitemComplete() { Console.WriteLine("Executing Workitem Complete..."); Console.ReadLine(); }
    static void WorkitemReject() { Console.WriteLine("Executing Workitem Reject..."); Console.ReadLine(); }
    static void WorkitemCategoryGet() { Console.WriteLine("Executing Workitem Category Get..."); Console.ReadLine(); }
    static void WorkitemCategoryPut() { Console.WriteLine("Executing Workitem Category Put..."); Console.ReadLine(); }
    static void ErrorCodes() { Console.WriteLine("Displaying Error Codes..."); Console.ReadLine(); }
}
