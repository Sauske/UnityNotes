set WORKSPACE=..
set PROJECTSPACE=..\..\..
set PROJECT_DATA_PATH=%PROJECTSPACE%\ExcelTool\Assets\Config\json
set PROJECT_CODE_PATH=%PROJECTSPACE%\ExcelTool\Assets\Scrpts\Config
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
	-c cs-simple-json ^
	-d json ^
    --conf %CONF_ROOT%\luban.conf ^
	-x inputDataDir=%CONF_ROOT%\Datas ^
	-x outputCodeDir=%PROJECT_CODE_PATH% ^
	-x outputDataDir=%PROJECT_DATA_PATH%
    -x pathValidator.rootDir=%WORKSPACE%\ExcelTool ^
    -x l10n.textProviderFile=*@%CONF_ROOT%\Datas\l10n\texts.json

pause