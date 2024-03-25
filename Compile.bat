set projectDir=%CD%\mvc
set targetdir=%CD%\compiled

del /q %targetdir%\*
for /d %%x in (%targetdir%\*) do @rd /s /q ^"%%x^"

rd "%projectDir%\obj" /S /Q
md "%projectDir%\obj"
md "%projectDir%\obj\Debug"
md "%projectDir%\obj\Release"

C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_compiler -c -p %projectDir% -v / %targetdir%

PAUSE