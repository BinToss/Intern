/// From
/// https://stackoverflow.com/questions/1220213/detect-if-running-as-administrator-with-or-without-elevated-privileges/4497572#4497572
/// Original https://archive.codeplex.com/?p=uachelpers Edit 1
/// https://stackoverflow.com/a/4497572/80274 Edit 2 https://stackoverflow.com/a/55079599/14894786
/// Edit 3
///
/// NuGet GitHub https://github.com/falahati/UACHelper

using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using static Intern.Helpers.UacHelper.TokenElevationType;

namespace Intern.Helpers
{
    // TODO https://stackoverflow.com/a/38676215/14894786
    public static class UacHelper
    {
        private const string uacRegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
        private const string uacRegistryValue = "EnableLUA";

        private static uint STANDARD_RIGHTS_READ = 0x00020000;
        private static uint TOKEN_QUERY = 0x0008;
        private static uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TokenInformationClass TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        public enum TokenInformationClass
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        public enum TokenElevationType
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        public static bool IsUacEnabled
        {
            get
            {
                using (RegistryKey uacKey = Registry.LocalMachine.OpenSubKey(uacRegistryKey, false))
                {
                    bool result = uacKey.GetValue(uacRegistryValue).Equals(1);
                    return result;
                }
            }
        }

        public static bool IsProcessElevated
        {
            get
            {
                if (IsUacEnabled)
                {
                    IntPtr tokenHandle = IntPtr.Zero;
                    if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_READ, out tokenHandle))
                    {
                        throw new ApplicationException("Could not get process token.  Win32 Error Code: " +
                                                       Marshal.GetLastWin32Error());
                    }

                    try
                    {
                        TokenElevationType elevationResult = TokenElevationTypeDefault;

                        int elevationResultSize = Marshal.SizeOf(typeof(TokenElevationType));
                        uint returnedSize = 0;

                        IntPtr elevationTypePtr = Marshal.AllocHGlobal(elevationResultSize);
                        try
                        {
                            bool success = GetTokenInformation(tokenHandle, TokenInformationClass.TokenElevationType,
                                                               elevationTypePtr, (uint)elevationResultSize,
                                                               out returnedSize);
                            if (success)
                            {
                                elevationResult = (TokenElevationType)Marshal.ReadInt32(elevationTypePtr);
                                bool isProcessAdmin = elevationResult == TokenElevationTypeFull;
                                return isProcessAdmin;
                            }
                            else
                            {
                                throw new ApplicationException("Unable to determine the current elevation.");
                            }
                        }
                        finally
                        {
                            if (elevationTypePtr != IntPtr.Zero)
                                Marshal.FreeHGlobal(elevationTypePtr);
                        }
                    }
                    finally
                    {
                        if (tokenHandle != IntPtr.Zero)
                            CloseHandle(tokenHandle);
                    }
                }
                else
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    bool result = principal.IsInRole(WindowsBuiltInRole.Administrator)
                               || principal.IsInRole(0x200); //Domain Administrator
                    return result;
                }
            }
        }

        ///////////////
        /// Section 2
        /// https://stackoverflow.com/a/1220234/14894786
        /// Roughly translated from C++ to C# -- Noah Sherwin
        ///////////////

        private static bool IsCurrentProcessElevated => GetProcessTokenElevationType() == TokenElevationTypeFull;    //elevated

        public static TokenElevationType GetProcessTokenElevationType(Process process = null)
        {
            if (process == null)
                process = Process.GetCurrentProcess();

            IntPtr hToken = new IntPtr();
            try
            {
                if (!OpenProcessToken(process.Handle, TOKEN_QUERY, out hToken))
                    throw new Win32Exception(GetLastError());

                TokenElevationType elevationType;
                long dwSize;
                if (!GetTokenInformation(hToken, TokenInformationClass.TokenElevationType, out elevationType, IntPtr.Zero, out dwSize))
                    throw new Win32Exception(GetLastError());

                return elevationType;
            }
            finally
            {
                CloseHandle(hToken);
            }
        }

        private static int GetLastError()
        {
            return GetLastWin32Error();
        }

        private static int GetLastWin32Error()
        {
            throw new NotImplementedException();
        }
    }
}