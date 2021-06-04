using System;

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
            FileSystemTakeOwnership
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

                    break;

                case Type.FileSystemTakeOwnership:
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
                throw new UnauthorizedAccessException($"The Intern was denied required permissions to \"{path}\" while working on the task, \"{task}\".");
            }
        }
    }
}
