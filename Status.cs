using System;

/// Copyright Noah Sherwin 2021
/// This file is part of the 'Intern' project
///
namespace Intern
{
    public class Status
    {
        /* TODO: Task parameters? */

        /// <summary>
        ///     Create a new instance of Status with the given parameters.
        /// </summary>
        /// <param name="success">Boolean representing the success or failure state.</param>
        /// <param name="message">String detailing the success or failure.</param>
        /// <param name="task">(Optional) The <see cref="Tasks.Task"/> linked to this Status.</param>
        /// <param name="uri">(Optional) A URI-type path to a file, directory, web URL, et cetera.</param>
        /// <param name="e">(Optional) The Exception to be passed with the Status. If an exception was thrown, success will be false.</param>
        public Status(Type state, string message, Tasks.Type task = Tasks.Type.None, Uri uri = null, Exception e = null)
        {
            State = state;
            Message = message;
            Task = task;
            URI = uri;
            exception = e;
        }

        /// <summary>
        ///     Task or operation's success/failure state.
        /// </summary>
        public Type State;

        /// <summary>
        ///     Message detailing the task or operation <br/>
        ///     and, if it failed, the reason for failure.
        /// </summary>
        public string Message;

        /// <summary>
        ///     (Optional) The task executed or to be executed.
        /// </summary>
        public Tasks.Type Task;

        public Uri URI;

        public Exception exception;

        public enum Type
        {
            Succeeded,
            Failed,
            Undetermined
        }
    }
}