using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.IO;
using System.Net;
using System.Web;

namespace ConsoleApp3
{
    /// Class ServerAliveCheck checks if a server is on, each X seconds
    /// An Instance of the class receives the server's name and amount of seconds to check that the server
    /// is on
    /// to stop the program, press q and enter
    class ServerAliveCheck
    {
        private System.Timers.Timer Timer;
        private readonly String _ServerUrl = "";
        private readonly int _seconds;
        private readonly LogClass _log;

        // Constructor for ServerAliveCheck class
        // Receives the server's name and seconds to check 
        public ServerAliveCheck(String URL, int seconds)
        {
            this._ServerUrl = URL;
            this._seconds = seconds;
            _log = LogClass.GetLogger();
        }

        // Starts the ServerAliveCheck 
        public void Start()
        {
            StartTimer(this._seconds); // Starting the timer

            Console.WriteLine("Application started at {0:HH:mm:ss.fff}. To stop press: q and Enter", DateTime.Now);
            Console.ReadLine();
            Timer.Stop();
            Timer.Dispose();
        }

        private void StartTimer(int time)
        {
            Timer = new System.Timers.Timer(time * 1000);
            Timer.Elapsed += OnTimedEvent;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Checks if the stop char was inserted by the user, if yes, stops the program
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo c = Console.ReadKey();
                if (c.Equals("q"))
                {
                    return;
                }
            }

            Console.WriteLine("Time elapsed at {0:HH:mm:ss.fff}",
                              e.SignalTime);
            // On elapsed time, check if the Server is alive
            CreateHttpRequest(this._ServerUrl);
        }

        // Private method that checks if the DSA Server is "alive", by creating a WebRequest,
        // using "GET" method
        private void CreateHttpRequest(String URL)
        {
            try
            {
                WebRequest HttpRequest = WebRequest.Create(URL);
                HttpRequest.Method = "GET";
                WebResponse response = HttpRequest.GetResponse();

                // Saving and printing the status of the request
                var ResponseStatus = ((HttpWebResponse)response).StatusCode;
              
                // Checks if the server's response has status 200 (OK)
                if (ResponseStatus == HttpStatusCode.OK)
                {
                    // prints a message to the user that the server is up
                    Console.WriteLine("The Server is OK"); 

                    // Writes successfull log to the log file
                    _log.WriteLog(string.Format("Server {0} is OK in date: {1}",this._ServerUrl, DateTime.Now));

                    // Get the response's stream
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        // Display the content (optional)
                       // Console.WriteLine(responseFromServer);
                    }

                    response.Close();
                }
            }
            catch (System.UriFormatException e) // Catch the exception of incorrect URL format
            {
                Console.WriteLine("Error - URL format incorrect");
                _log.WriteLog(string.Format("Error - URL {0} format incorrect in date: {1}", this._ServerUrl, DateTime.Now));
            }
            catch (UnauthorizedAccessException e) // Catch the exception of error in openning the file
            {
                Console.WriteLine("Error openning the file");

                // Write error log to the log file
                _log.WriteLog(string.Format("Error - Unauthorized access at : {0}" ,DateTime.Now));
            }
            catch (WebException e) // Catch the exception of error in web request
            {
                Console.WriteLine("Error - The Server returned an error");
               
                // Write error log to the log file
                _log.WriteLog(string.Format("Server {0} returned an ERROR in date: {1}", this._ServerUrl, DateTime.Now));
            }
        }
    }
}
