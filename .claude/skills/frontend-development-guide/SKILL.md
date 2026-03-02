---
name: frontend-development-guide
description:  Create a front-end program suitable for this project
---

This skill guides creation of distinctive, production-grade frontend interfaces that avoid generic "AI slop" aesthetics. Implement real working code with exceptional attention to aesthetic details and creative choices.

The user provides frontend requirements: a component, page, application, or interface to build. They may include context about the purpose, audience, or technical constraints.

## Project Technology Stack
类型 | 技术栈 | 目前最新版本 |
|---------|------|------|
**包管理** | pnpm | -  
**JS框架** | React |  v19.2
**基础框架** | Vite |  v7.3.1
**路由框架** | reactrouter |  v7.13.0
**UI框架** | @heroui/react |  v2.8.7
**UI组件** | tailwindcss , @tailwindcss/vite |  v4.1.4
**Icon** | @icon-park/react |  v1.4.2

## Development Thinking
在你每次编码之前你都需要询问自己下面的问题后，再进行下一步决策：
1. 了解项目基础信息
- 了解项目中已存在的包 Read `package.json`

2. 现有组件库评估
- 了解项目中已存在的包 Read `package.json`
- 判断现有组件库 Read [components_index.md](components_index.MD) 是否存在适合的组件？
- 如果存在 Read [heroui.md](heroui.md) 进行后续开发
- 如果不存在 Read [tailwindCss.md](tailwindCss.md) 进行后续开发