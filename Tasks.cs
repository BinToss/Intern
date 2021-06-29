///////
/// Copyright Noah Sherwin 2021
/// This file is part of the 'Intern' project

using Intern.Helpers;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
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

                case Type.ComparePermissions: // TODO: What do I want from this?
                    if (task.Path == null) throw Exceptions.PathIsNullException(task.Type);
                    // TODO: Check these per parameter. We might be given a procID and a file

                    var str1 = task.Path.Split(';')[0];
                    var str2 = task.Path.Split(';')[1];

                    bool arePaths = Path.IsPathRooted(str1) && Path.IsPathRooted(str2);

                    /// Check if Reg Keys
                    try
                    {
                        string[] hkeyNames = new string[]
                        {
                              "HKEY_CLASSES_ROOT",
                              "HKCR",
                              "HKEY_CURRENT_USER",
                              "HKCU",
                              "HKEY_LOCAL_MACHINE",
                              "HKLM",
                              "HKEY_USERS",
                              "HKU",
                              "HKEY_PERFORMANCE_DATA",
                              "HKEY_CURRENT_CONFIG",
                              "HKEY_DYN_DATA"
                        };

                        bool str1HasBase = false;
                        string objABase;
                        bool str2HasBase = false;
                        string objBBase;

                        foreach (string hkeyname in hkeyNames)
                        {
                            if (str1.StartsWith(hkeyname))
                            {
                                str1HasBase = true;
                                objABase = hkeyname;
                            }
                        }

                        foreach (string hkeyname in hkeyNames)
                        {
                            if (str2.StartsWith(hkeyname))
                            {
                                str2HasBase = true;
                                objBBase = hkeyname;
                            }
                        }

                        if (!str1HasBase || !str2HasBase)
                            throw new ArgumentException();

                        var hiveA = GetHiveByName(str1.Split('\\')[0]);
                        var hiveB = GetHiveByName(str2.Split('\\')[0]);
                        using (var keyA = RegistryKey.OpenBaseKey(hiveA, RegistryView.Default))
                        using (var keyB = RegistryKey.OpenBaseKey(hiveB, RegistryView.Default))
                        {
                            // TODO: Compare RegKey ACLs; Difficulty: easier than processes
                        }

                        RegistryHive GetHiveByName(string name)
                        {
                            RegistryHive hive;

                            if (name.Equals(hkeyNames[0], StringComparison.CurrentCultureIgnoreCase) ||
                                name.Equals(hkeyNames[1], StringComparison.CurrentCultureIgnoreCase))
                                hive = RegistryHive.ClassesRoot;
                            else if (name.Equals(hkeyNames[2], StringComparison.CurrentCultureIgnoreCase) ||
                                    name.Equals(hkeyNames[3], StringComparison.CurrentCultureIgnoreCase))
                                hive = RegistryHive.CurrentUser;
                            else if (name.Equals(hkeyNames[4], StringComparison.CurrentCultureIgnoreCase) ||
                                    name.Equals(hkeyNames[5], StringComparison.CurrentCultureIgnoreCase))
                                hive = RegistryHive.LocalMachine;
                            else if (name.Equals(hkeyNames[6], StringComparison.CurrentCultureIgnoreCase) ||
                                    name.Equals(hkeyNames[7], StringComparison.CurrentCultureIgnoreCase))
                                hive = RegistryHive.Users;
                            else if (name.Equals(hkeyNames[8], StringComparison.CurrentCultureIgnoreCase))
                                hive = RegistryHive.PerformanceData;
                            else if (name.Equals(hkeyNames[9], StringComparison.CurrentCultureIgnoreCase))
                                hive = RegistryHive.CurrentConfig;
                            else if (name.Equals(hkeyNames[10], StringComparison.CurrentCultureIgnoreCase))
                                hive = RegistryHive.DynData;
                            else throw new IndexOutOfRangeException();

                            return hive;
                        }
                    }
                    catch (ArgumentException) { }
                    catch (IndexOutOfRangeException) { }
                    catch (Exception) { }

                    // probably a process ID
                    if (!arePaths)
                    {
                        int ID1 = int.Parse(str1);
                        int ID2 = int.Parse(str2);
                        var proc1 = System.Diagnostics.Process.GetProcessById(ID1);
                        var proc2 = System.Diagnostics.Process.GetProcessById(ID2);
                    }

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