using System;
using System.Management;

namespace Intern.Helpers
{
    /// <summary>
    ///     An extension to System.Diagnostic.Process using Windows Management Instrumentation.<br/>
    /// </summary>
    public class Process : System.Diagnostics.Process
    {
        public string ProcessOwner => GetProcessOwner();

        /// <summary>
        ///     Code used by other GetProcessOwner methods.
        /// </summary>
        /// <param name="obj">A Process represented as a WMI Instance.</param>
        /// <returns>The name of the process owner as a string.</returns>
        internal static string GetProcessOwnerInternal(ManagementObject obj)
        {
            string[] argList = new string[] { string.Empty, string.Empty };
            int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
            if (returnVal == 0)
            {
                // return DOMAIN\user
                return argList[1] + "\\" + argList[0];
            }
            else return "NO OWNER";
        }

        /// <summary>
        ///     Get the name of the process owner formatted similarly to 'DOMAIN\PRINCPAL'.
        /// </summary>
        /// <returns>The name of the process owner as a string.</returns>
        /// <remarks>
        ///     Originally written by <see href="https://stackoverflow.com/users/40347/dirk-vollmar">Dirk Vollmar</see>
        ///     in <see href="https://stackoverflow.com/a/777567/14894786">How do I determine the owner of a process in C#?</see>.
        /// </remarks>
        internal static string GetProcessOwnerSearch(string query)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                GetProcessOwnerInternal(obj);
            }

            return "NO OWNER";
        }

        internal string GetProcessOwner()
        {
            return GetProcessOwner(Id);
        }

        /// <summary>
        ///     Get the name of the process owner formatted similarly to 'DOMAIN\PRINCPAL'.
        /// </summary>
        /// <param name="processId">The numeral ID of the process.</param>
        /// <returns>The name of the process owner as a string.</returns>
        /// <remarks>
        ///     Originally written by <see href="https://stackoverflow.com/users/40347/dirk-vollmar">Dirk Vollmar</see>
        ///     in <see href="https://stackoverflow.com/a/777567/14894786">How do I determine the owner of a process in C#?</see>.
        /// </remarks>
        public static string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            return GetProcessOwnerSearch(query);
        }

        /// <summary>
        ///     Get the name of the process owner formatted similarly to 'DOMAIN\PRINCPAL'.
        /// </summary>
        /// <param name="processName">The name of the process's executable. The main module as determined by Process.MainModule.</param>
        /// <returns>The name of the process owner as a string.</returns>
        /// <remarks>
        ///     Originally written by <see href="https://stackoverflow.com/users/40347/dirk-vollmar">Dirk Vollmar</see>
        ///     in <see href="https://stackoverflow.com/a/777567/14894786">How do I determine the owner of a process in C#?</see>.
        /// </remarks>
        public static string GetProcessOwner(string processName)
        {
            string query = "Select * from Win32_Process Where Name = \"" + processName + "\"";
            return GetProcessOwnerSearch(query);
        }
    }
}