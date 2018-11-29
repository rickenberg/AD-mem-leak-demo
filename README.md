# Memory Leak Demo #

This program reads one Active Directory group and gets all users from it. It is used to demonstrate how to use the Visual Studio tools to find managed and native memory leaks in your code.

I assume that you have never debugged native code before and have only barely noticed the memory profiler tool, that was introduced in Visual Studio 2015.

All descriptions and screen-shots are for Visual Studio 2017.

If you want to run the tool on your machine, make sure to change the value of the AdDomain field in Program class to your AD domain.

## AdLeak1 - Simple Managed Leak ##
Make sure that only leak1 is executed in Program.Main method:
```
            var leak1 = new AdLeak1();
            while (true)
            {
                leak1.Execute();
            }
```
Now, start the debugger by pressing F5. Switch to the 'Memory Usage' tab in the 'Diagnostic Tools' window. Take a snapshot, wait a bit and take one more snapshot.

Inspect the 'Heap Size (Diff)' column. The value in parenthesis indicates the change in managed memory consumption since the last snapshot. 
In our case, this will be quite significant. Click on the link and a detailed list will show up as shown below.

![Leak1](/Leak1.png "Managed Memory Leak")

## AdLeak2 - Native Leaks ##
Make sure that only leak2 is executed in Program.Main method.

Code is leaking un-managed resources using the .NET managed code library to access Active Directory.
Memory consumption is increasing rapidly but the managed mem profiler shows no difference - inspect the Heap Size column. So we are leaking memory from un-managed or native code!

Enable code analysis while building to find one of the leaks.

Now, start the debugger by pressing F5. Switch to the 'Memory Usage' tab in the 'Diagnostic Tools' window. Take a snapshot, wait a bit and take one more snapshot.

Again, inspect the 'Heap Size (Diff)' column. This time there is zero difference as shown below. But from the 'Process Memory' diagram above you can see
that you are clearly leaking memory.

![Leak2-1](/Leak2-1.png "Native Memory Leak")

Let us try to find the memory leaks using the Code Analysis in Visual Studio. Go to project settings / Code Analysis and select 'Enable Code Analysis on build' 
and rebuild the application. This will show the error below. AdLeak3 class fixes this issue.
```
Warning	CA1001	Implement IDisposable on 'AdLeak2' because it creates members of the following IDisposable types: 'PrincipalContext'.
```

## AdLeak 3 - One Native Leak Remaining ##
Make sure that only leak3 is executed in Program.Main method.

Go to the MemLeaker project settings / Debug tab and enable the setting 'Enable native code debugging' as shown below.
![Leak3-1](/Leak3-1.png "Enable native code debugging")

Now, start the debugger by pressing F5. Notice the new button called 'Heap profiling' on the Memory Usage tab. Click to activate it.
![Leak3-2](/Leak3-2.png "Enable heap profiling")

You will notice two new columns being added to the grid below: Native allocations and Native Heap size. Now we can see what is going on with unmanaged code 
memory consumption.

Again, take two snapshots with some time in between. Then, click on the diff value in the Native Heap size column of the last snapshot as shown below.
![Leak3-3](/Leak3-3.png "Native memory consumption")

This opens up a new window which is completely empty. We are looking for memory leaks, so memory that is no longer in use - also called unresolved allocations.
We need to enable these to show in the debugger. Click on the little filter icon and disable the filter 'Hide Unresolved Allocations' as shown below.
![Leak3-4](/Leak3-4.png "Enable unresolved allocations")

This shows now on row as you can see below.
![Leak3-5](/Leak3-5.png "Show unresolved allocations")

Click on the 'Unresolved allocations' row, sort by Size Diff column and then on one of the entries. Scroll-down in the call stack window below until you see something familiar.
The screenshot below shows that the System.DirectoryServices.AccountManagement.ni.dll is involved in the memory leak - so that must have something to do with the
code querying the AD.
![Leak3-6](/Leak3-6.png "Inspect memory leak")

The static method 'GroupPrincipal.FindByIdentity' returns a GroupPrincipal object that needs to be disposed in order to free the native resources being used.

## AdNoLeak - Both leaks removed ##
You can check-out the AdNoLeak implementation. For this little example we have found and fixed the two leak by adding using statements to the code. These take
care of freeing the native resource once the code has finished.

```
            using (_context = new PrincipalContext(ContextType.Domain, Program.AdDomain))
            {
				...
            }
```			
```
            using (var group = GroupPrincipal.FindByIdentity(_context, groupName))
            {
				...
            }
```

To verify the changes, run the application again with F5 and the NoAdLeak code enabled in Program.Main.

The ProcessMemory graph shows clearly that the memory consumption is no longer increasing in general. There are periodic peaks and then again memory is freed.
![NoLeak](/NoLeak.png "Inspect memory leak")
