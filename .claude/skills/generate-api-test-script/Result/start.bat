@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
cd /d "%~dp0"

echo ============================================================
echo   SQLBotX API 测试工具 - 一键启动脚本
echo ============================================================
echo.

:: -----------------------------------------------------------
:: 1. 检查 Node.js
:: -----------------------------------------------------------
echo [1/3] 正在检查 Node.js 环境...

call node -v >nul 2>nul
if %errorlevel% neq 0 goto ErrorNode

:: 显示版本
for /f "tokens=*" %%v in ('node -v') do set NODE_VERSION=%%v
echo ✅ Node.js 版本: !NODE_VERSION!
echo.


:: -----------------------------------------------------------
:: 2. 检查依赖包 (Axios)
:: -----------------------------------------------------------
echo [2/3] 正在检查依赖包...

:: 逻辑优化：直接检查 axios 是否存在，而不是只检查 node_modules 文件夹
if exist "node_modules\axios" (
    echo ✅ 依赖包已安装
    goto RunTest
)

echo 📦 正在安装依赖包 (axios)...
echo    这可能需要几分钟时间...
echo.

:: --- 尝试使用 pnpm ---
where pnpm >nul 2>nul
if %errorlevel% equ 0 (
    echo ✅ 检测到 pnpm，正在安装...
    call pnpm install axios
    goto CheckInstallResult
)

:: --- 尝试使用 yarn ---
where yarn >nul 2>nul
if %errorlevel% equ 0 (
    echo ✅ 检测到 yarn，正在安装...
    call yarn add axios
    goto CheckInstallResult
)

:: --- 默认使用 npm ---
echo ✅ 未检测到 pnpm/yarn，使用 npm 安装...
call npm install axios

:CheckInstallResult
if %errorlevel% neq 0 goto ErrorInstall
echo ✅ 依赖包安装完成!
echo.


:: -----------------------------------------------------------
:: 3. 运行测试
:: -----------------------------------------------------------
:RunTest
echo [3/3] 正在启动 API 测试工具...
echo ============================================================
echo.

:: 再次确认脚本文件存在
if not exist "api-test.js" goto ErrorFile

:: 运行脚本
call node api-test.js

echo.
echo ============================================================
echo   测试执行结束
echo ============================================================
echo.
pause
exit /b 0


:: ===========================================================
:: 错误处理区域 (GOTO 跳转点)
:: ===========================================================

:ErrorNode
echo.
echo [错误] 未检测到 Node.js!
echo.
echo 请先安装 Node.js (推荐 LTS 版本):
echo 1. 访问 https://nodejs.org/zh-cn/
echo 2. 下载并安装
echo 3. 重启此脚本
echo.
pause
exit /b 1

:ErrorInstall
echo.
echo [错误] 依赖包安装失败!
echo 请检查网络连接，或尝试手动运行 npm install axios
echo.
pause
exit /b 1

:ErrorFile
echo.
echo [错误] 未找到 api-test.js 文件!
echo 请确保该文件在当前目录下。
echo.
pause
exit /b 1
