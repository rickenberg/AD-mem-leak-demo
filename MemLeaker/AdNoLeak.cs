using System;
using System.Linq;
using System.DirectoryServices.AccountManagement;

namespace MemLeaker
{
    /// <summary>
    /// Both memory leaks are fixed. Just run it and see the difference.
    /// </summary>
    class AdNoLeak
    {
        private PrincipalContext _context;

        public void Execute()
        {
            using (_context = new PrincipalContext(ContextType.Domain, Program.AdDomain))
            {
                var groupName = GetGroup(Program.AdGroupName);
                Console.WriteLine($"{groupName}");
            }
        }

        private string GetGroup(string groupName)
        {
            using (var group = GroupPrincipal.FindByIdentity(_context, groupName))
            {
                var members = group.Members.Where(s => s is UserPrincipal).Select(s => s.SamAccountName).Aggregate((a, b) => $"{a},{b}");
                Console.WriteLine(members);
                return group.Name;
            }
        }
    }
}
