/// Copyright Noah Sherwin 2021
/// This file is part of the 'Intern' project
///

namespace Intern
{
    public class Status
    {
        /* TODO: Task parameters? */

        public Status(bool success, string message, Tasks.Type task = Tasks.Type.None)
        {
            Success = success;
            Message = message;
            Task = task;
        }

        /// <summary>
        ///     Task or operation's success/failure state.
        /// </summary>
        public bool Success;

        /// <summary>
        ///     Message detailing the task or operation <br/>
        ///     and, if it failed, the reason for failure.
        /// </summary>
        public string Message;

        /// <summary>
        ///     (Optional) The task executed or to be executed.
        /// </summary>
        public Tasks.Type Task;
    }
}