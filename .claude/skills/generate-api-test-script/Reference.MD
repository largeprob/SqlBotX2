# 一键启动Bat脚本规范
- **严格谨记**，此脚本运行在Windows环境之上，确保bat文件中使用的命令、运行格式、编码格式符合Windows环境
- **Stability Constraints (核心稳定性要求)**
    1. **禁止深层嵌套**：绝对不要使用深层嵌套的 `if ... ( if ... )` 结构，因为这极易导致语法错误和闪退。
    2. **使用 GOTO 跳转模式**：必须使用 `GOTO Label` 的方式来处理流程控制（如错误跳转、安装成功跳转），确保代码是线性执行的。
    3. **编码处理**：必须在开头设置 `chcp 65001` 以支持 UTF-8 中文输出，并重定向输出 `>nul` 防止乱码干扰。
    4. **路径处理**：开头必须使用 `cd /d "%~dp0"` 确保脚本在当前目录下运行，即使路径中包含空格也能正常工作。
    5. **变量延迟**：开启 `setlocal enabledelayedexpansion`。
    6. **用户体验**：
        - 使用 `echo =====` 等符号做清晰的视觉分隔。
        - 在脚本结束或报错时使用 `pause`，确保窗口不会自动关闭，让用户能看到结果。
- **Example**
    ``` CMD
    @echo off
    chcp 65001 >nul
    setlocal enabledelayedexpansion
    cd /d "%~dp0"

    echo ============================================================
    echo   .NET API 测试工具 - 一键启动脚本
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
    ```

# NodeJS CLI规范
- Functional Requirements (功能要求)
    1. **主循环结构**：
    - 使用 `while (true)` 循环包裹主程序逻辑。
    - 每次循环开始时清屏并显示主菜单（假设 `showMenu` 函数已存在）。
    - 根据用户选择执行相应的异步任务。

    2. **按键暂停机制 (Press Enter to Continue)**：
    - 在每个任务执行完毕后，不要立即清屏或重新显示菜单。
    - 输出提示文本：“按回车键返回主菜单...”。
    - 程序进入 **阻塞等待状态**，直到用户按下回车键。

    3. **Technical Implementation (技术实现)**：
    - 使用 Node.js 原生 `process.stdin` 实现，不依赖第三方库。
    - 创建一个名为 `waitKeyPress` 的 Promise 函数来实现暂停。
    - 在等待期间开启 `stdin.setRawMode(true)` 以捕获按键。
    - 捕获到回车键 (`\r` 或 `\n`) 后：
        1. 关闭 Raw Mode (`stdin.setRawMode(false)`).
        2. 移除事件监听器 (避免内存泄漏).
        3. Resolve Promise 以继续执行。..
- Example Code Structure (参考结构),请参考以下逻辑流编写 `main` 函数：
    ```javascript
    async function main() {
        console.clear();
        log('🧪 .NET API 测试工具', colors.bold);
        log('OpenAPI 3.1.1 测试脚本', colors.gray);
        log('');

        // Check Node.js
        log(`Node.js 版本: ${process.version}`, colors.gray);
        log(`服务器地址: ${CONFIG.BASE_URL}`, colors.gray);
        log(`端口: ${CONFIG.PORT}`, colors.gray);
        log('');

        // Login
        await login();
        await delay(500);

        // Show menu
        while (true) {
            const mode = await showMenu();

            if (mode === 'exit') {
            log('\n👋 再见!', colors.green);
            process.exit(0);
            }

            currentMode = mode;
            console.clear();
            log('🧪 开始测试...', colors.cyan);
            log('');

            let result;
            const startTime = Date.now();

            try {
            if (mode === 'single') {
                result = await executeSingleTest();
                generateReport(result, '单一测试');
            } else if (mode === 'module') {
                result = await executeBusinessModuleTest();
                generateReport(result, '业务模块测试');
            } else if (mode === 'integration') {
                result = await executeBusinessIntegrationTest();
                generateReport(result, '业务集成测试');
            }

            const totalTime = Date.now() - startTime;
            log(`\n⏱️  总执行时间: ${totalTime}ms`, colors.cyan);

            } catch (error) {
            log(`\n❌ 测试执行出错: ${error.message}`, colors.red);
            console.error(error);
            }

            log('\n按回车键返回菜单...', colors.gray);
            await new Promise(resolve => {
            stdin.setRawMode(true);
            stdin.resume();
            stdin.setEncoding('utf8');
            const onData = (key) => {
                if (key === '\r' || key === '\n' || key === '\u0003') {
                stdin.setRawMode(false);
                stdin.pause();
                stdin.removeListener('data', onData);
                resolve();
                }
            };
            stdin.on('data', onData);
            });
        }
    }