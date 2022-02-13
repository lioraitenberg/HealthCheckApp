using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    // This class is used to read Json file and returns the data
    class JSONFile
    {
        private String _FilePath = ""; // Path of the Json file (received as a argument to the application)
        private JsonData _Data = null; // JsonData instance is used to save the data inside the Json file

        public JSONFile(String FilePath)
        {
            this._FilePath = FilePath;
        }

        // Reads the data in the Json file and saves it in JsonData object
        public bool ReadFile() 
        {
            try
            {
                string JsonText = "";
                using (StreamReader reader = new StreamReader(_FilePath))
                {
                    JsonText = reader.ReadToEnd();
                }
                JsonData Data = JsonSerializer.Deserialize<JsonData>(JsonText);
                this._Data = Data;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        // Returns the data inside the Json file
        // If the file hasn't been read yet, returns null
        public JsonData GetData()
        {
            return this._Data;
        }
    }
}
