---
name: generate-api-test-script
description: 生成API测试脚本
---

# OutPut Files Path
所有输出文件保存在 `/Result` 目录

## Additional resources
- 生成bat文件核心规范, see [reference.md](reference.md)
- NodeJS CLI核心规范, see [reference.md](reference.md)

# 核心步骤
1. 仔细阅读项目文档，理解API规范
2. 使用 `rm -rf`命令删除 `/Result/` 目录
3. 生成`测试脚本`与`一键启动`
4. 生成脚本后进行脚本自检，检查代码和逻辑是否出现错误

# 测试脚本生成规范
1. **命名**：`api-test.js`

2. **运行环境**：
   - 必须使用 Node.js 原生 `http` 模块，启动 HTTP 服务
   - port 60827
   - timeout 10s
   - 启动Http服务时输出当前执行测试服务的服务信息
   - 调用接口时使用`axios`请求接口,本地环境 忽略SSL验证 `process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';`

3. **授权与鉴权**：
    - login 用户密码 admin 123456
    - 若 API 文档包含身份验证流程，则需自动调用鉴权接口获取凭证并在后续请求中应用

4. **Mock 数据要求（非文件上传接口）**：
    - 每个接口至少生成 5 条 Mock 输入数据，直接生成Mock数据
    - Mock 数据必须符合该接口参数定义与类型约束，尽可能还原真实业务输入场景

5. **测试模式** 需生成三类测试模式，并输出独立结果：
    1. **单一测试**：
    - 逐个接口执行并输出测试报告：
        - 📊 总请求数 (Total)
        - ✅ 成功数 (Success 2xx)
        - ❌ 失败数 (Failed)
        - ⏱️ 耗时统计: 平均耗时、最大耗时、最小耗时
        - 📉 错误详情: 如果有失败，列出具体的错误原因和 Response Body

    2. **业务模块测试**：
    - 按模块顺序执行接口并输出模块级测试报告：
        - 📊 总请求数 (Total)
        - ✅ 成功数 (Success 2xx)
        - ❌ 失败数 (Failed)
        - ⏱️ 耗时统计: 平均耗时、最大耗时、最小耗时
        - 📉 错误详情: 如果有失败，列出具体的错误原因和 Response Body

    3. **业务集成测试**：
    - 按模块顺序执行业务链路，并输出模块级集成测试报告：
        - 📊 总请求数 (Total)
        - ✅ 成功数 (Success 2xx)
        - ❌ 失败数 (Failed)
        - ⏱️ 耗时统计: 平均耗时、最大耗时、最小耗时
        - 📉 错误详情: 如果有失败，列出具体的错误原因和 Response Body
    
6. **NodeJS CLI支持** ：
支持在命令行中通过键盘上下键选择不同的 测试模式 选项并回车确认,测试完成后可以返回重新显示菜单


# 一键启动生成规范
1. **命名**：`start.bat`

# 基础数据组织示例
将分析的API数据按照如下参考格式组织，作为后续测试的基础数据
```TypeScript
     const apis = [
      {
         tag:'...',
         apis:[
            {
               api:'...',
               auth:/* true or false */,
               mock:[
                { /* 根据入参字段生成 mock 数据*/ }
               ]
            }
         ]
      }
     ];
```






