set projectDir=%CD%\mvc
set targetdir=%CD%\OutPut

del /q %targetdir%\*
for /d %%x in (%targetdir%\*) do @rd /s /q ^"%%x^"

xcopy /e /v %projectDir% %targetdir% 

del /q %targetdir%\appSettings.config
del /q %targetdir%\behaviors.config
del /q %targetdir%\bindings.config
del /q %targetdir%\browserCaps.config
del /q %targetdir%\client.config
del /q %targetdir%\compilation.config
del /q %targetdir%\connectionStrings.config
del /q %targetdir%\email.config
del /q %targetdir%\services.config
del /q %targetdir%\trace.config
rd  "%targetdir%\Uploads" /S /Q

PAUSE