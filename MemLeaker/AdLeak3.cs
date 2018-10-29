using System;
using System.Linq;
using System.DirectoryServices.AccountManagement;

namespace MemLeaker
{
    /// <summary>
    /// One of the un-managed memory leaks is fixed.
    /// Application still leaks a lot of memory. 
    /// Use the mem profiler to see what's going on:
    /// - Enable native code debugging for this project
    /// - Enable Heap profiling in the mem profile
    /// - Go to the native mem profile and disable the Hide unresolved allocations filter
    /// - See what is causing the leaks
    /// </summary>
    class AdLeak3
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
            var group = GroupPrincipal.FindByIdentity(_context, groupName);
            var members = group.Members.Where(s => s is UserPrincipal).Select(s => s.SamAccountName).Aggregate((a, b) => $"{a},{b}");
            Console.WriteLine(members);
            return group.Name;
        }
    }
}
