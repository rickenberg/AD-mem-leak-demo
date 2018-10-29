using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace MemLeaker
{
    /// <summary>
    /// Simple managed code leak
    /// We just add a lot of stuff to a static list. This leak is easy to find in mem profiler.
    /// </summary>
    class AdLeak1
    {
        private PrincipalContext _context;
        private static IList<GroupPrincipal> _groupCache;

        public AdLeak1()
        {
            _groupCache = new List<GroupPrincipal>();
        }

        public void Execute()
        {
            _context = new PrincipalContext(ContextType.Domain, Program.AdDomain);

            var groupName = GetGroup(Program.AdGroupName);
            Console.WriteLine($"{groupName}");
        }

        private string GetGroup(string groupName)
        {
            var group = GroupPrincipal.FindByIdentity(_context, groupName);
            _groupCache.Add(group);
            return group.Name;
        }
    }
}
