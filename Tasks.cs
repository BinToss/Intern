///////
/// Copyright Noah Sherwin 2021
/// This file is part of the 'Intern' project

using System;
using System.Collections.Generic;
using System.IO;

namespace Intern
{
    public class Tasks
    {
        /** TODO: Create template functions for each task. Result will resemble the System.Threading.Task namespace */

        /// <summary>
        ///     The Type of Task to execute.
        /// </summary>
        public enum Type
        {
            None,
            FileSystemDelete,
            ComparePermissions,
            ModifyPermissions,
            ReadPermissions
        }

        public List<Task> TaskList;

        /// <summary>
        ///     A structured template for a Task's metadata.
        /// </summary>
        public struct Task
        {
            public Type Type;
            public string Path;
            public string Parameters;
        }

        /// <summary> Execute the specified task—with the given path if necessary. </summary> <param
        /// name="task">The <see cref="Type"> of task to execute.</param> <param name="path">A
        /// network, filesystem, Window Registry path.</param>
        public static void DoTask(Task task)
        {
            switch (task.Type)
            {
                case Type.FileSystemDelete:
                    FileSystemDelete(task);

                    break;

                case Type.ModifyPermissions:
                    if (task.Path == null) throw Exceptions.PathIsNullException(task.Type);
                    break;

                default: throw new ArgumentOutOfRangeException($"The Intern does not recognize the task, \"{task}\".");
            }
        }

        private static void FileSystemDelete(Task task)
        {
            if (string.IsNullOrEmpty(task.Path)) throw Exceptions.PathIsNullException(task.Type);

            /// <see href="https://stackoverflow.com/a/1395226/14894786" />
            try
            {
                FileAttributes attr = File.GetAttributes(task.Path);
                bool isDir = attr.HasFlag(FileAttributes.Directory);

                if (attr.HasFlag(FileAttributes.Directory))
                    new DirectoryInfo(task.Path).Delete(recursive: true);
                else new FileInfo(task.Path).Delete();
            }
            catch (ArgumentException) { } /** invalid path */
            catch (PathTooLongException) { }
            catch (NotSupportedException) { } /** invalid format */
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
            catch (IOException) /** The file is in use by another process */
            {
                // TODO: try to close the handle(s) of the file; Employ
            }
            catch (UnauthorizedAccessException)
            {
                // TODO: determine if we can obtain permission to continue
                // - new process with elevation or alternative authorization or change the item's permissions
            }
        }

        public static class Exceptions
        {
            public static ArgumentNullException PathIsNullException(Type task)
            {
                throw new ArgumentNullException($"The Intern must be provided with a file or directory path for the task \"{task}\".");
            }

            public static UnauthorizedAccessException UnauthorizedAccessException(Type task, string path)
            {
                throw new UnauthorizedAccessException($"The Intern was denied required permissions for \"{path}\" while working on the task, \"{task}\".");
            }
        }
    }
}