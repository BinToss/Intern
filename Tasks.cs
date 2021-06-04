/// Copyright Noah Sherwin 2021
/// This file is part of the 'Intern' project
///

using System;
using System.IO;

namespace Intern
{
    public class Tasks
    {
        /// <summary>
        ///     The Type of Task to execute.
        /// </summary>
        public enum Type
        {
            FileSystemDelete,
            FileSystemModifyPermissions
        }

        /// <summary>
        ///     Execute the specified task—with the given path if necessary.
        /// </summary>
        /// <param name="task">The <see cref="Type"> of task to execute.</param>
        /// <param name="path">A network, filesystem, Window Registry path.</param>
        public static void DoTask(Type task, string path = null)
        {
            switch (task)
            {
                case Type.FileSystemDelete:
                    if (path == null) throw Exceptions.PathIsNullException(task);

                    /// <see href="https://stackoverflow.com/a/1395226/14894786"/>
                    try
                    {
                        FileAttributes attr = File.GetAttributes(path);
                        if (attr.HasFlag(FileAttributes.Directory))
                            new DirectoryInfo(path).Delete(recursive: true);
                        else new FileInfo(path).Delete();
                    }
                    /** invalid path */
                    catch (ArgumentException) { }
                    catch (PathTooLongException) { }
                    /** invalid format */
                    catch (NotSupportedException) { }
                    catch (FileNotFoundException) { }
                    catch (DirectoryNotFoundException) { }
                    /** The file is in use by another process */
                    catch (IOException)
                    {
                        // TODO: try to close the handle(s) of the file
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // TODO: determine if we can obtain permission to continue
                        // - new process with elevation or alternative authorization or change the item's permissions
                    }

                    break;

                case Type.FileSystemModifyPermissions:
                    if (path == null) throw Exceptions.PathIsNullException(task);
                    break;

                default: throw new ArgumentOutOfRangeException($"The Intern does not recognize the task, \"{task}\".");
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