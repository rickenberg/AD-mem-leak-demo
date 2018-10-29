Memory Leak Demo
================

This program reads one Active Directory group and gets all users from it. It is used to demonstrate how to use the Visual Studio tools to find managed and native memory leaks in your code.

I assume that you have never debugged native code before and have only barely noticed the memory profiler tool, that was introduced in Visual Studio 2015.

All descriptions and screen-shots are for Visual Studio 2017.

If you want to run the tool on your machine, make sure to change the value of the AdDomain field in Program class to your AD domain.

#. AdLeak1 - Simple Managed Leak
Make sure that only leak1 is executed in Program.Main method:
```
            var leak1 = new AdLeak1();
            while (true)
            {
                leak1.Execute();
            }
```
Now, start the debugger by pressing F5. Switch to the 'Memory Usage' tab in the 'Diagnostic Tools' window. Take a snapshop, wait a bit and take one more snapshot.

Inspect the 'Heap Size (Diff)' column. The value in parenthesis indicates the change in managed memory consumption since the last snapshot. 
In our case, this will be quite significant. Click on the link and a detailed list will show up as shown below.

![Leak1](/Leak1.png "Managed Memory Leak")