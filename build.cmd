@echo off
"%~dp0\tools\nuget\nuget.exe" "install" "FAKE" "-OutputDirectory" "%~dp0\tools" "-ExcludeVersion" "-Verbosity" "quiet"
"%~dp0\tools\FAKE\tools\FAKE.exe" %*