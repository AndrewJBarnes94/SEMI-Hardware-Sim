using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PT_Sim
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    public static class Logger
    {
        private static readonly string logFilePath = "C:\\Users\\AJ\\Documents\\BroCode_Software\\PT_Sim\\log.txt"; // Change this path if needed

        public static void Log(object message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    if (message is List<float> floatList)
                    {
                        // Convert List<float> to a comma-separated string
                        writer.WriteLine($"{DateTime.Now}: {string.Join(", ", floatList)}");
                    }
                    else
                    {
                        // Log normal text
                        writer.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Logging error: {ex.Message}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }

}
