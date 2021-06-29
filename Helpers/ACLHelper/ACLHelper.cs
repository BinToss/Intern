using System;
using System.Security.AccessControl;
using System.Security.Permissions;

namespace Intern.Helpers
{
    public class ACLComposite
    {
        public enum ObjectType
        {
            _unknown = 0,
            Directory,
            File,
            Process,
            RegistryKey
        }

        public FileIOPermission FileIOPermission(PermissionState state)
        {
            return new FileIOPermission(state);
        }

        public FileIOPermission IOPermission;

        public object ACL = new FileIOPermission(PermissionState.None); // alternatively, PermissionState.Unrestricted
    }
}