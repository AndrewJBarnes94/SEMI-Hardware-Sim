using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PT_Sim
{
    public static class Logger
    {
        private static readonly string logFilePath = "C:\\Users\\AJ\\Documents\\BroCode_Software\\PT_Sim\\log.txt"; // Change this path if needed

        public static void Log(string component, object message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    if (message is List<float> floatList)
                    {
                        // Log the component and the float list in a readable format
                        writer.WriteLine($"{timestamp} [{component}]: {string.Join(", ", floatList)}");
                    }
                    else
                    {
                        // Log normal text with component name
                        writer.WriteLine($"{timestamp} [{component}]: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Logging error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
