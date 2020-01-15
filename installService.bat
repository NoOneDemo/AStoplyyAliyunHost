@echo off
set exeName=JobSchedulerService.exe
set BASE_DIR=%~dp0
set insPath=%windir%\Microsoft.NET\Framework64\v4.0.30319
echo ############ 开始安装后台服务=>%exeName% 
echo.
%BASE_DIR:~0,2%
cd %BASE_DIR%
echo. 
echo ############ 1.清理原有服务项 按任意键开始
pause
installutil.exe /U %exeName%
echo. 
echo ############ 2.清理完毕，按任意键开始安装后台服务  
pause
echo.
installutil.exe %exeName%
echo.  
echo ############ 3.操作结束，请在 InstallUtil.InstallLog 中查看具体的操作结果。   
echo.
pause