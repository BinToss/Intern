/// Copyright Noah Sherwin 2021
/// This file is part of the 'Intern' project
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using static System.IO.Path;

namespace Intern
{
    public class Memo
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="taskList"></param>
        /// <returns></returns>
        public Status Write(List<Task> taskList)
        {
            var memoPath = GetTempFileName();

            using (var fs = new FileStream(memoPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            using (var ms = new MemoryStream(0xFFFF))
            using (var xw = XmlWriter.Create(ms))
            {
                /// TODO: Finish Memo.Write()
                /// 1. Task list to XML
                /// 2. Copy

                ms.CopyTo(fs);
            }

            return new Status(true, "Memo read successfully; Task started in background");
        }

        /// TODO: Finish Memo.Read()
        /// <summary>
        ///     Read the text contents of the given memo file and execute the instructed task(s).
        /// </summary>
        /// <param name="memoPath">The path the to memo file.</param>
        /// <returns>An instance of <see cref="Status"/>.</returns>
        public Status Read(string memoPath)
        {
            bool readSuccess = false;
            bool tasksStarted = false;

            try
            {
                using (var fs = new FileStream(memoPath, FileMode.Open, FileAccess.Read))
                using (var ms = new MemoryStream(0xFFFF))
                using (var xr = XmlReader.Create(ms))
                {
                    fs.CopyTo(ms);
                }

                readSuccess = true;

                var tasks = new Tasks().TaskList;

                var queue = new Task(() =>
                {
                    foreach (Tasks.Task task in tasks)
                    {
                        Tasks.DoTask(task);
                    };
                });
                queue.Start();
                tasksStarted = true;
            }
            catch (Exception e)
            {
                if (!readSuccess)
                    return new Status(false, "The Intern could not read the memo. Reason: " + e.Message);
                else if (!tasksStarted)
                    return new Status(false, "The Intern could not start the task(s). Reason: " + e.Message);
                else
                    return new Status(false, "The Intern failed to complete the task(s). Reason: " + e.Message);
            }

            var list = new List<bool>
            {
                tasksStarted,
                readSuccess
            };

            /** Read succeeded */
            return new Status(true, "The Intern read the memo and started the task(s).");
        }
    }
}