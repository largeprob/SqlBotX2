/**
 * SQLBotX API 测试脚本
 * 基于对 SqlBoTx.Net.ApiService 的API接口分析
 * 生成时间: 2026-01-18
 */

const axios = require('axios');
const http = require('http');
const readline = require('readline');
const { stdin, stdout } = process;

// 忽略SSL验证
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

// 配置
const CONFIG = {
    BASE_URL: 'http://localhost:5000',
    PORT: 60827,
    TIMEOUT: 10000,
    LOGIN_USER: 'admin',
    LOGIN_PASS: '123456'
};

// API定义
const APIS = [
    {
        tag: 'Mini API - 系统接口',
        apis: [
            {
                name: '登录接口',
                method: 'GET',
                path: '/login',
                auth: false,
                description: '用户登录，返回Cookie凭证',
                mock: [
                    { account: 'admin', password: '123456' },
                    { account: 'test', password: '123456' },
                    { account: 'user1', password: '123456' },
                    { account: 'user2', password: '123456' },
                    { account: 'user3', password: '123456' }
                ]
            },
            {
                name: '检查用户登录状态',
                method: 'GET',
                path: '/checkUser',
                auth: true,
                description: '检查当前用户登录状态',
                mock: []
            }
        ]
    },
    {
        tag: 'DatabaseSetting - 数据库连接管理',
        apis: [
            {
                name: '获取数据库连接列表',
                method: 'GET',
                path: '/DatabaseSetting/list',
                auth: false,
                description: '获取数据库连接列表（不包含表结构）',
                mock: []
            },
            {
                name: '获取带表结构的数据库连接列表',
                method: 'GET',
                path: '/DatabaseSetting/list-with-tables',
                auth: false,
                description: '获取数据库连接列表（包含表结构）',
                mock: []
            },
            {
                name: '新增数据库连接',
                method: 'POST',
                path: '/DatabaseSetting/add',
                auth: false,
                description: '新增数据库连接',
                mock: [
                    {
                        connectionName: 'TestDB_1',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=TestDB1;Trusted_Connection=true;',
                        userName: 'testuser1',
                        userPassword: 'testpass1',
                        description: '测试数据库连接1'
                    },
                    {
                        connectionName: 'TestDB_2',
                        connectionType: 1,
                        connectionString: 'Server=192.168.1.100;Database=ProductionDB;User ID=sa;Password=Admin123;',
                        userName: 'sa',
                        userPassword: 'Admin123',
                        description: '生产数据库连接'
                    },
                    {
                        connectionName: 'TestDB_3',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=SalesDB;Trusted_Connection=true;',
                        userName: 'sales_user',
                        userPassword: 'sales_pass',
                        description: '销售数据库'
                    },
                    {
                        connectionName: 'TestDB_4',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=InventoryDB;Trusted_Connection=true;',
                        userName: 'inventory_user',
                        userPassword: 'inventory_pass',
                        description: '库存数据库'
                    },
                    {
                        connectionName: 'TestDB_5',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=CustomerDB;Trusted_Connection=true;',
                        userName: 'customer_user',
                        userPassword: 'customer_pass',
                        description: '客户数据库'
                    }
                ]
            },
            {
                name: '更新数据库连接',
                method: 'POST',
                path: '/DatabaseSetting/update',
                auth: false,
                description: '更新数据库连接',
                mock: [
                    {
                        connectionId: 1,
                        connectionName: 'UpdatedDB_1',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=UpdatedDB1;Trusted_Connection=true;',
                        userName: 'updated_user1',
                        userPassword: 'updated_pass1',
                        description: '更新后的数据库连接1',
                        lastModifiedDate: new Date().toISOString()
                    },
                    {
                        connectionId: 2,
                        connectionName: 'UpdatedDB_2',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=UpdatedDB2;Trusted_Connection=true;',
                        userName: 'updated_user2',
                        userPassword: 'updated_pass2',
                        description: '更新后的数据库连接2',
                        lastModifiedDate: new Date().toISOString()
                    },
                    {
                        connectionId: 3,
                        connectionName: 'UpdatedDB_3',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=UpdatedDB3;Trusted_Connection=true;',
                        userName: 'updated_user3',
                        userPassword: 'updated_pass3',
                        description: '更新后的数据库连接3',
                        lastModifiedDate: new Date().toISOString()
                    },
                    {
                        connectionId: 4,
                        connectionName: 'UpdatedDB_4',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=UpdatedDB4;Trusted_Connection=true;',
                        userName: 'updated_user4',
                        userPassword: 'updated_pass4',
                        description: '更新后的数据库连接4',
                        lastModifiedDate: new Date().toISOString()
                    },
                    {
                        connectionId: 5,
                        connectionName: 'UpdatedDB_5',
                        connectionType: 1,
                        connectionString: 'Server=localhost;Database=UpdatedDB5;Trusted_Connection=true;',
                        userName: 'updated_user5',
                        userPassword: 'updated_pass5',
                        description: '更新后的数据库连接5',
                        lastModifiedDate: new Date().toISOString()
                    }
                ]
            }
        ]
    },
    {
        tag: 'SQLChat - SQL对话',
        apis: [
            {
                name: 'SQL对话接口',
                method: 'POST',
                path: '/SQLChat',
                auth: false,
                description: 'SQL对话（SSE流式响应）',
                mock: [
                    {
                        content: [{ type: 'text', text: '查询最近7天的销售数据' }],
                        role: 'user',
                        status: 'streaming',
                        createdAt: Date.now()
                    },
                    {
                        content: [{ type: 'text', text: '统计每个产品的月度销售额' }],
                        role: 'user',
                        status: 'streaming',
                        createdAt: Date.now()
                    },
                    {
                        content: [{ type: 'text', text: '查询订单数量最多的前10个客户' }],
                        role: 'user',
                        status: 'streaming',
                        createdAt: Date.now()
                    },
                    {
                        content: [{ type: 'text', text: '分析库存低于警戒线的产品' }],
                        role: 'user',
                        status: 'streaming',
                        createdAt: Date.now()
                    },
                    {
                        content: [{ type: 'text', text: '查询本月的总收入' }],
                        role: 'user',
                        status: 'streaming',
                        createdAt: Date.now()
                    }
                ]
            }
        ]
    },
    {
        tag: 'RAGDemo - RAG演示',
        apis: [
            {
                name: 'RAG演示接口',
                method: 'POST',
                path: '/RAGDemo',
                auth: false,
                description: 'RAG向量存储演示',
                mock: []
            }
        ]
    }
];

// 全局变量
let cookies = {};
let testResults = [];

// 工具函数
const delay = (ms) => new Promise(resolve => setTimeout(resolve, ms));

const log = (message, color = '') => {
    const colors = {
        red: '\x1b[31m',
        green: '\x1b[32m',
        yellow: '\x1b[33m',
        blue: '\x1b[34m',
        cyan: '\x1b[36m',
        gray: '\x1b[90m',
        bold: '\x1b[1m',
        reset: '\x1b[0m'
    };

    if (color && colors[color]) {
        console.log(`${colors[color]}${message}${colors.reset}`);
    } else {
        console.log(message);
    }
};

const formatTime = (ms) => {
    if (ms < 1000) return `${ms}ms`;
    return `${(ms / 1000).toFixed(2)}s`;
};

// HTTP请求函数
async function makeRequest(method, path, data = null, useAuth = false) {
    const url = `${CONFIG.BASE_URL}${path}`;
    const headers = {
        'Content-Type': 'application/json'
    };

    if (useAuth && cookies.auth_user) {
        headers.Cookie = `auth_user=${cookies.auth_user}`;
    }

    const startTime = Date.now();

    try {
        const config = {
            method,
            url,
            headers,
            timeout: CONFIG.TIMEOUT
        };

        if (data && (method === 'POST' || method === 'PUT' || method === 'PATCH')) {
            config.data = data;
        }

        if (method === 'GET' && data) {
            config.params = data;
        }

        const response = await axios(config);
        const duration = Date.now() - startTime;

        // 保存Cookie
        if (response.headers['set-cookie']) {
            const cookieHeader = response.headers['set-cookie'].find(c => c.includes('auth_user'));
            if (cookieHeader) {
                const match = cookieHeader.match(/auth_user=([^;]+)/);
                if (match) {
                    cookies.auth_user = match[1];
                }
            }
        }

        return {
            success: true,
            status: response.status,
            data: response.data,
            duration,
            error: null
        };
    } catch (error) {
        const duration = Date.now() - startTime;
        return {
            success: false,
            status: error.response?.status || 0,
            data: error.response?.data || error.message,
            duration,
            error: error.message
        };
    }
}

// 登录函数
async function login() {
    log('🔐 正在登录...', 'cyan');

    const result = await makeRequest('GET', '/login', {
        account: CONFIG.LOGIN_USER,
        password: CONFIG.LOGIN_PASS
    });

    if (result.success) {
        log(`✅ 登录成功 (用户: ${CONFIG.LOGIN_USER})`, 'green');
        return true;
    } else {
        log(`❌ 登录失败: ${result.error}`, 'red');
        return false;
    }
}

// 测试执行函数
async function executeSingleTest() {
    log('🧪 开始单一测试模式', 'cyan');
    log('');

    const results = [];
    let totalRequests = 0;
    let successCount = 0;
    let failedCount = 0;
    const durations = [];

    for (const module of APIS) {
        log(`\n📊 模块: ${module.tag}`, 'bold');

        for (const api of module.apis) {
            totalRequests++;
            log(`  → ${api.method} ${api.path} (${api.name})`, 'gray');

            // 执行测试
            let testResult;
            if (api.mock.length > 0) {
                // 使用第一个mock数据
                testResult = await makeRequest(api.method, api.path, api.mock[0], api.auth);
            } else {
                testResult = await makeRequest(api.method, api.path, null, api.auth);
            }

            results.push({
                module: module.tag,
                api: api.name,
                method: api.method,
                path: api.path,
                ...testResult
            });

            durations.push(testResult.duration);

            if (testResult.success) {
                successCount++;
                log(`    ✅ 成功 (${formatTime(testResult.duration)})`, 'green');
            } else {
                failedCount++;
                log(`    ❌ 失败 (${formatTime(testResult.duration)})`, 'red');
                log(`       错误: ${testResult.error}`, 'red');
            }

            await delay(100);
        }
    }

    return {
        totalRequests,
        successCount,
        failedCount,
        durations,
        results
    };
}

async function executeBusinessModuleTest() {
    log('🧪 开始业务模块测试模式', 'cyan');
    log('');

    const results = [];
    let totalRequests = 0;
    let successCount = 0;
    let failedCount = 0;
    const durations = [];

    for (const module of APIS) {
        log(`\n📊 模块: ${module.tag}`, 'bold');

        let moduleSuccess = 0;
        let moduleFailed = 0;

        for (const api of module.apis) {
            totalRequests++;
            log(`  → ${api.method} ${api.path} (${api.name})`, 'gray');

            let testResult;
            if (api.mock.length > 0) {
                testResult = await makeRequest(api.method, api.path, api.mock[0], api.auth);
            } else {
                testResult = await makeRequest(api.method, api.path, null, api.auth);
            }

            results.push({
                module: module.tag,
                api: api.name,
                method: api.method,
                path: api.path,
                ...testResult
            });

            durations.push(testResult.duration);

            if (testResult.success) {
                successCount++;
                moduleSuccess++;
                log(`    ✅ 成功 (${formatTime(testResult.duration)})`, 'green');
            } else {
                failedCount++;
                moduleFailed++;
                log(`    ❌ 失败 (${formatTime(testResult.duration)})`, 'red');
                log(`       错误: ${testResult.error}`, 'red');
            }

            await delay(100);
        }

        log(`  📈 模块统计: ${moduleSuccess}成功 / ${moduleFailed}失败`, 'cyan');
    }

    return {
        totalRequests,
        successCount,
        failedCount,
        durations,
        results
    };
}

async function executeBusinessIntegrationTest() {
    log('🧪 开始业务集成测试模式', 'cyan');
    log('');

    const results = [];
    let totalRequests = 0;
    let successCount = 0;
    let failedCount = 0;
    const durations = [];

    // 业务流程1: 登录 -> 检查用户 -> 数据库连接管理
    log('\n📊 业务流程1: 用户登录与数据库管理', 'bold');

    // 1. 登录
    log('  → 登录', 'gray');
    const loginResult = await makeRequest('GET', '/login', {
        account: CONFIG.LOGIN_USER,
        password: CONFIG.LOGIN_PASS
    });
    totalRequests++;
    durations.push(loginResult.duration);

    if (loginResult.success) {
        successCount++;
        log(`    ✅ 登录成功 (${formatTime(loginResult.duration)})`, 'green');
    } else {
        failedCount++;
        log(`    ❌ 登录失败 (${formatTime(loginResult.duration)})`, 'red');
    }

    // 2. 检查用户
    log('  → 检查用户登录状态', 'gray');
    const checkUserResult = await makeRequest('GET', '/checkUser', null, true);
    totalRequests++;
    durations.push(checkUserResult.duration);

    if (checkUserResult.success) {
        successCount++;
        log(`    ✅ 检查用户成功 (${formatTime(checkUserResult.duration)})`, 'green');
    } else {
        failedCount++;
        log(`    ❌ 检查用户失败 (${formatTime(checkUserResult.duration)})`, 'red');
    }

    // 3. 获取数据库连接列表
    log('  → 获取数据库连接列表', 'gray');
    const listResult = await makeRequest('GET', '/DatabaseSetting/list');
    totalRequests++;
    durations.push(listResult.duration);

    if (listResult.success) {
        successCount++;
        log(`    ✅ 获取列表成功 (${formatTime(listResult.duration)})`, 'green');
    } else {
        failedCount++;
        log(`    ❌ 获取列表失败 (${formatTime(listResult.duration)})`, 'red');
    }

    // 4. 新增数据库连接
    log('  → 新增数据库连接', 'gray');
    const addResult = await makeRequest('POST', '/DatabaseSetting/add', APIS[1].apis[2].mock[0]);
    totalRequests++;
    durations.push(addResult.duration);

    if (addResult.success) {
        successCount++;
        log(`    ✅ 新增连接成功 (${formatTime(addResult.duration)})`, 'green');
    } else {
        failedCount++;
        log(`    ❌ 新增连接失败 (${formatTime(addResult.duration)})`, 'red');
    }

    // 业务流程2: SQL对话测试
    log('\n📊 业务流程2: SQL对话测试', 'bold');

    log('  → 发送SQL对话请求', 'gray');
    const sqlChatResult = await makeRequest('POST', '/SQLChat', APIS[2].apis[0].mock[0]);
    totalRequests++;
    durations.push(sqlChatResult.duration);

    if (sqlChatResult.success) {
        successCount++;
        log(`    ✅ SQL对话请求成功 (${formatTime(sqlChatResult.duration)})`, 'green');
    } else {
        failedCount++;
        log(`    ❌ SQL对话请求失败 (${formatTime(sqlChatResult.duration)})`, 'red');
    }

    // 业务流程3: RAG演示测试
    log('\n📊 业务流程3: RAG演示测试', 'bold');

    log('  → 发送RAG演示请求', 'gray');
    const ragResult = await makeRequest('POST', '/RAGDemo');
    totalRequests++;
    durations.push(ragResult.duration);

    if (ragResult.success) {
        successCount++;
        log(`    ✅ RAG演示请求成功 (${formatTime(ragResult.duration)})`, 'green');
    } else {
        failedCount++;
        log(`    ❌ RAG演示请求失败 (${formatTime(ragResult.duration)})`, 'red');
    }

    return {
        totalRequests,
        successCount,
        failedCount,
        durations,
        results
    };
}

// 报告生成函数
function generateReport(result, title) {
    log('\n' + '='.repeat(60), 'bold');
    log(`📊 ${title} 测试报告`, 'bold');
    log('='.repeat(60), 'bold');
    log('');

    // 统计
    const total = result.totalRequests;
    const success = result.successCount;
    const failed = result.failedCount;
    const successRate = total > 0 ? ((success / total) * 100).toFixed(1) : 0;

    // 耗时统计
    const durations = result.durations;
    const avgTime = durations.length > 0 ? durations.reduce((a, b) => a + b, 0) / durations.length : 0;
    const maxTime = durations.length > 0 ? Math.max(...durations) : 0;
    const minTime = durations.length > 0 ? Math.min(...durations) : 0;

    log(`📊 总请求数 (Total): ${total}`, 'cyan');
    log(`✅ 成功数 (Success): ${success} (${successRate}%)`, 'green');
    log(`❌ 失败数 (Failed): ${failed}`, failed > 0 ? 'red' : 'gray');
    log('');
    log(`⏱️ 耗时统计:`, 'cyan');
    log(`   平均耗时: ${formatTime(avgTime)}`, 'gray');
    log(`   最大耗时: ${formatTime(maxTime)}`, 'gray');
    log(`   最小耗时: ${formatTime(minTime)}`, 'gray');

    // 错误详情
    if (failed > 0) {
        log('');
        log(`📉 错误详情:`, 'red');
        const errors = result.results.filter(r => !r.success);
        errors.forEach((err, index) => {
            log(`   ${index + 1}. [${err.module}] ${err.method} ${err.path}`, 'red');
            log(`      错误: ${err.error}`, 'gray');
            if (err.data) {
                log(`      响应: ${JSON.stringify(err.data).substring(0, 200)}...`, 'gray');
            }
        });
    }

    log('');
    log('='.repeat(60), 'bold');
}

// CLI菜单函数
function showMenu() {
    return new Promise((resolve) => {
        console.clear();
        log('🧪 SQLBotX API 测试工具', 'bold');
        log('基于 SqlBoTx.Net.ApiService 的API接口分析', 'gray');
        log('');
        log(`Node.js 版本: ${process.version}`, 'gray');
        log(`服务器地址: ${CONFIG.BASE_URL}`, 'gray');
        log(`端口: ${CONFIG.PORT}`, 'gray');
        log('');
        log('='.repeat(60), 'bold');
        log('');
        log('请选择测试模式:', 'cyan');
        log('');
        log('  1. 单一测试 (逐个接口执行)', 'white');
        log('  2. 业务模块测试 (按模块分组执行)', 'white');
        log('  3. 业务集成测试 (完整业务链路)', 'white');
        log('  4. 退出', 'white');
        log('');
        log('输入数字 (1-4) 并回车: ', 'cyan');

        const rl = readline.createInterface({ input: stdin, output: stdout });

        rl.question('', (answer) => {
            rl.close();
            const choice = answer.trim();

            if (choice === '1') resolve('single');
            else if (choice === '2') resolve('module');
            else if (choice === '3') resolve('integration');
            else if (choice === '4' || choice.toLowerCase() === 'exit') resolve('exit');
            else {
                log('❌ 无效选择，请重新输入', 'red');
                setTimeout(() => {
                    showMenu().then(resolve);
                }, 1000);
            }
        });
    });
}

// 等待按键函数
function waitKeyPress() {
    return new Promise((resolve) => {
        log('\n按回车键返回主菜单...', 'gray');

        const rl = readline.createInterface({ input: stdin, output: stdout });

        rl.question('', (answer) => {
            rl.close();
            resolve();
        });
    });
}

// 启动HTTP测试服务
function startTestServer() {
    const server = http.createServer((req, res) => {
        if (req.url === '/health') {
            res.writeHead(200, { 'Content-Type': 'application/json' });
            res.end(JSON.stringify({ status: 'ok', timestamp: new Date().toISOString() }));
        } else {
            res.writeHead(200, { 'Content-Type': 'text/plain' });
            res.end('SQLBotX API Test Server Running');
        }
    });

    server.listen(CONFIG.PORT, () => {
        log(`🚀 HTTP测试服务已启动`, 'green');
        log(`   端口: ${CONFIG.PORT}`, 'gray');
        log(`   健康检查: http://localhost:${CONFIG.PORT}/health`, 'gray');
        log('');
    });

    return server;
}

// 主函数
async function main() {
    console.clear();
    log('🧪 SQLBotX API 测试工具', 'bold');
    log('OpenAPI 3.1.1 测试脚本', 'gray');
    log('');

    // 启动HTTP测试服务
    const testServer = startTestServer();
    await delay(500);

    // 检查Node.js版本
    log(`Node.js 版本: ${process.version}`, 'gray');
    log(`服务器地址: ${CONFIG.BASE_URL}`, 'gray');
    log(`端口: ${CONFIG.PORT}`, 'gray');
    log('');

    // 登录
    const loginSuccess = await login();
    if (!loginSuccess) {
        log('⚠️  登录失败，将继续执行测试（部分接口可能需要认证）', 'yellow');
    }
    await delay(500);

    // 显示菜单
    while (true) {
        const mode = await showMenu();

        if (mode === 'exit') {
            log('\n👋 再见!', 'green');
            testServer.close();
            process.exit(0);
        }

        console.clear();
        log('🧪 开始测试...', 'cyan');
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
            log(`\n⏱️  总执行时间: ${formatTime(totalTime)}`, 'cyan');

        } catch (error) {
            log(`\n❌ 测试执行出错: ${error.message}`, 'red');
            console.error(error);
        }

        await waitKeyPress();
    }
}

// 运行主函数
if (require.main === module) {
    main().catch((error) => {
        log(`\n❌ 致命错误: ${error.message}`, 'red');
        console.error(error);
        process.exit(1);
    });
}
