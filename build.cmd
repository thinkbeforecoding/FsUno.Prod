@echo off

.nuget\nuget.exe install FAKE -OutputDirectory tools -ExcludeVersion
.nuget\nuget.exe install FSharp.Formatting.CommandTool -OutputDirectory tools -ExcludeVersion
cls  

set encoding=utf-8
tools\FAKE\tools\FAKE.exe build.fsx %*

