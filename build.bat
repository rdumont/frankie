@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
 
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

set nuget=
if "%nuget%" == "" (
	set nuget=nuget
)

set nunit="tools\nunit\nunit-console.exe"

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild src\Frankie.sln /p:Configuration=%config% /v:M

%nunit% /noresult ^
  src\Tests\bin\%config%\RDumont.Frankie.Tests.dll ^
  src\Specs\bin\%config%\RDumont.Frankie.Specs.dll

mkdir Build

%nuget% pack src\Core\Core.csproj -verbosity detailed -o Build -Version %version%
%nuget% pack src\CommandLine\CommandLine.csproj -IncludeReferencedProjects -verbosity detailed -o Build -Version %version%