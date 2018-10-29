using System;
using System.Linq;
using System.DirectoryServices.AccountManagement;

namespace MemLeaker
{
    /// <summary>
    /// Code is leaking un-managed resources using the .NET managed code library to access Active Directory.
    /// Memory consumption is increasing rapidly but the managed mem profiler shows no difference.
    /// Enable code analysis while building to find one of the leaks.
    /// </summary>
    class AdLeak2
    {
        private PrincipalContext _context;

        public void Execute()
        {
            _context = new PrincipalContext(ContextType.Domain, Program.AdDomain);

            var groupName = GetGroup(Program.AdGroupName);
            Console.WriteLine($"{groupName}");
        }

        private string GetGroup(string groupName)
        {
            var group = GroupPrincipal.FindByIdentity(_context, groupName);
            var members = group.Members.Where(s => s is UserPrincipal).Select(s => s.SamAccountName).Aggregate((a, b) => $"{a},{b}");
            Console.WriteLine(members);
            return group.Name;
        }
    }
}
