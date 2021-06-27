using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return new System.Security.Permissions.FileIOPermission(state);
        }

        public FileIOPermission IOPermission;

        public object ACL = new System.Security.Permissions.FileIOPermission(PermissionState.None); // alternatively, PermissionState.Unrestricted
    }
}