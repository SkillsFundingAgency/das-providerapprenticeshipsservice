
* Structure Of Project *

This is a .NetStandard project that provides access to Framework and Core specific instances of Urlhelper extensions class. There is no NetStanbdard definition of UrlHelper extensions, as there is no NetStandard definition for service resolution.

The csproj uses ItemGroup tags to conditionally include and exclude folders and files that are not appropriate to the current TargetFramework. 
  