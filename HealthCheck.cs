using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    // Main class of the program
    // Class HealthCheck receives 5 arguments: exe run file, -url, url name, -configFile, configFile path
    // Checks if the arguments are valid, if not, exists
    // Otherwise, runs ServerAliveCheck program using the arguments it received.
    public class HealthCheck 
    {
        private static int CONFIG_FILE_ERROR = -1; // Indicates config file error
        private static readonly LogClass _log = LogClass.GetLogger(); // Get thr logger to write logs into Logs.txt file

        // The method receives a string and a regex and checks if the string matches the regex
        // Used to check if the url and configFile inputs are valid
        // receives the string to check, the regex string and an identifier: if it equals 0,
        // the string to check represents an url address, if it equals 1, the string to check represents 
        // configFile path. Returns false if the string to check doesn't match the regex
        public static bool RegexMatch(string StringToCheck, string RegexString, int identfier)
        {
            MatchCollection MatchCollection = Regex.Matches(StringToCheck, RegexString);

            // Identifer 0 for url regex, identifier 1 for config file regex
            // other identifer is invalid
            if ((identfier != 0) && (identfier != 1))
            {
                return false;
            }

            // If more than one regex recognized, the input is invalid
            if (MatchCollection.Count != 1)
            {
                switch (identfier)
                {
                    case 0:
                        _log.WriteLog(string.Format("regex URL format incorrect at: {0}", DateTime.Now));
                        break;
                    case 1:
                        _log.WriteLog(string.Format("regex config file format incorrect at: {0}", DateTime.Now));
                        break;
                    default:
                        return false;
                       // break;
                }
                return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            int NumOfArguments = 4; // Number of arguments the application accepts

            // Check that the number of arguments is valid
            if (args.Length != NumOfArguments)
            {
                Console.WriteLine("Number of arguments should be: {0} ", NumOfArguments);
                System.Environment.Exit(0);
            }

            // Gets the arguments
            String urlCommand="";
            String urlAddress="";
            String configFileCommand="";
            String configFileDesc="";

            try
            {
                urlCommand = args[0];
                urlAddress = args[1];
                configFileCommand = args[2];
                configFileDesc = args[3];
            }
            catch (Exception e)
            {
                _log.WriteLog(string.Format("Error reading arguments {0} at: {1} ",e, DateTime.Now));
                System.Environment.Exit(0);
            }
          
            // Checks if the arguments are valid
            CheckArguments(urlCommand, urlAddress, configFileCommand, configFileDesc);

            // Open config file - check that the returned tuple is valid, print logs
            Tuple<int, String> configFileData;
            configFileData = OpenFile(configFileDesc);

            // Check if config file data is valid
            if (configFileData.Item1 == CONFIG_FILE_ERROR) 
            {
                _log.WriteLog(string.Format("Parameters of config file are invalid at: {0}", DateTime.Now));
               // System.Environment.Exit(0);
                return;
            }

            int TimeInterval = configFileData.Item1;
            String ApllianceName = configFileData.Item2;

            // Append aplliance name to URL address
            urlAddress = urlAddress + ApllianceName; // assume that url address finishes with "/"

            ServerAliveCheck serverCheck = new ServerAliveCheck(urlAddress, TimeInterval);
            serverCheck.Start();
        }
        
        // Checks if the arguments values received by the application are valid
        private static void CheckArguments(String urlCommand, String urlAddress, String configFileCommand, String configFileDesc)
        {
            if ((urlCommand == null) || (urlAddress == null)
                    || (configFileCommand == null) || (configFileDesc == null))
            {
                return;
            }

            try
            {
                // Checks if the commands are valid
                if ((urlCommand != "-url") || (configFileCommand != "-configFile"))
                {
                    Console.WriteLine("Invalid commands, commands should be: {0} or {1}", "-url", "-configFile");
                    System.Environment.Exit(0);
                }

                CheckUrlAddress(urlAddress);
                CheckConfigFileDesc(configFileDesc);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading arguments {0} ", e);
            }
        }

        // Checks if the url argument has a valid format of URL address
        private static void CheckUrlAddress(String urlAddress)
        {
            if (urlAddress == null)
            {
                return;
            }

            String urlRegex = "https:\\/\\/[a-z,-]*.docusign*.net:[a-z,\\/, 0-9]*";

            bool isValidURL = RegexMatch(urlAddress, urlRegex, 0);
            if (!isValidURL)
            {
                System.Environment.Exit(0);
            }
        }
        
        // Checks if the file argument is a valid json file
        private static void CheckConfigFileDesc(String configFileDesc)
        {
            if (configFileDesc == null)
            {
                return;
            }

            String configRegex = ".*.json";

            bool isValidConfigFile = RegexMatch(configFileDesc, configRegex, 1);
            if (!isValidConfigFile)
            {
                System.Environment.Exit(0);
            }
        }

        // Opens the json file. If the read of the file is successfull, returns a tuple containing
        // the parameters that appear in the file, otherwise returns a tuple with error code -1
        private static Tuple<int, String> OpenFile(String configFileDesc)
        {
            Tuple<int, String> configFileDataNotFound =
                   new Tuple<int, String>(CONFIG_FILE_ERROR, "");

            try
            {
                JSONFile JsonFileObj = new JSONFile(configFileDesc);
                bool IsReadSuccessfull = JsonFileObj.ReadFile();

                // If the read of the json file is successfull, returns the parameters that appear in
                // the file: time interval in seconds and the name of appliance
                // Otherwise, returns a tuple with code error -1
                if (IsReadSuccessfull)
                {
                    JsonData data = JsonFileObj.GetData();

                    if (data == null)
                    {
                        return configFileDataNotFound;
                    }

                    Tuple<int, String> configFileData =
                        new Tuple<int, String>(data.TimeIntervalInSec, data.NameOfAppliance);

                    // Check validity of the json file parameters
                    // time in seconds is negative or 0
                    if (configFileData.Item1 <= 0)
                    {
                        return configFileDataNotFound;
                    }

                    return configFileData;
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(string.Format("Error openning json file at: {0}", DateTime.Now)); 
            }

            return configFileDataNotFound;
        }
    }
}
