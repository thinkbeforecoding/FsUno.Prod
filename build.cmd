@echo off

.nuget\nuget.exe install FAKE -OutputDirectory tools -ExcludeVersion -source http://nuget.org/api/v2
.nuget\nuget.exe install FSharp.Formatting.CommandTool -OutputDirectory tools -ExcludeVersion -source http://nuget.org/api/v2
cls  

set encoding=utf-8
tools\FAKE\tools\FAKE.exe build.fsx %*

