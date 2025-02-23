using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BizFlowRESTapiRunner
{
    class CommonUtils
    {

        /// <summary>
        /// Reads a password from the console input, masking the input with asterisks.
        /// </summary>
        /// <returns>The password entered by the user.</returns>
        public static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        /// <summary>
        /// Writes a line of text to the console in the specified color.
        /// </summary>
        /// <param name="text">The text to write to the console.</param>
        /// <param name="color">The color to use for the text.</param>
        public static void WriteColoredLine(string text, ConsoleColor color)
        {
            try
            {
                // Parse and format JSON using System.Text.Json
                using (JsonDocument doc = JsonDocument.Parse(text))
                {
                    text = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
                    {
                        WriteIndented = true // Enables pretty-printing
                    });
                }
            }
            catch (JsonException)
            {
                // Not a valid JSON, use original text
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Formats a JSON string to be more readable.
        /// </summary>
        /// <param name="text">The JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        public static string BeautifyJsonString(string text)
        {
            string formattedText = text;
            try
            {
                // Parse and format JSON using System.Text.Json
                using (JsonDocument doc = JsonDocument.Parse(text))
                {
                    formattedText = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
                    {
                        WriteIndented = true // Enables pretty-printing
                    });
                }
            }
            catch (JsonException)
            {
                // Not a valid JSON, use original text
            }
            catch (Exception ex)
            {
                // Handle other exceptions if necessary
            }

            return formattedText;
        }

        public static string ParseResponseForSessionKey(string responseContent)
        {
            using (JsonDocument doc = JsonDocument.Parse(responseContent))
            {
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean())
                {
                    if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.TryGetProperty("sessionKey", out JsonElement sessionKeyElement))
                    {
                        return sessionKeyElement.GetString();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Reads console input and ensures a valid input is provided.
        /// </summary>
        /// <returns>The valid input entered by the user.</returns>
        public static string ConsoleReadLineEx()
        {
            string input;
            do
            {
                input = Console.ReadLine()?.Trim();
            } while (string.IsNullOrEmpty(input));

            return input;
        }

        public static Dictionary<string, string> ConsoleReadProcessVariables()
        {
            var processVariables = new Dictionary<string, string>();
            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Write("Enter Process Variable Name: ");
                string name = ConsoleReadLineEx();

                Console.Write("Enter Process Variable Value: ");
                string value = Console.ReadLine()?.Trim();

                if (processVariables.ContainsKey(name))
                {
                    Console.WriteLine($"Process Variable '{name}' already exists. Replacing the value.");
                    processVariables[name] = value;
                }
                else
                {
                    processVariables.Add(name, value);
                }

                Console.WriteLine("Press ESC to stop entering process variables, or any other key to continue.");
                keyInfo = Console.ReadKey(true);
            } while (keyInfo.Key != ConsoleKey.Escape);

            return processVariables;
        }

        public static object[] ConvertToProcessVariable(Dictionary<string, string> processVariables)
        {
            return processVariables.Select(kvp => new { name = kvp.Key, value = kvp.Value }).ToArray();
        }

        public static List<FileInfoData> ConsoleReadFileUploads(ILogger logger, IConfigurationRoot config, string sessionKey)
        {
            List<FileInfoData> attachments = new List<FileInfoData>();

            FileUploadHandler fileUploader = new FileUploadHandler(logger);
            string fileUploadEndpoint = config["Network:EndpointFileUpload"];

            while (true)
            {
                Console.Write("Enter file full path to attach (or press Enter to finish): ");
                string filePath = Console.ReadLine();
                if (string.IsNullOrEmpty(filePath))
                {
                    break;
                }
                FileUploadResponse fileInfoData = fileUploader.UploadFile(fileUploadEndpoint, sessionKey, filePath);
                attachments.AddRange(fileInfoData.Data);
            }

            return attachments;
        }



    }
}
