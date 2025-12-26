import React, { useState, useRef, useEffect, type ReactElement } from 'react';

import HeaderToolbar from './top';
import { Me, PauseOne, Send, UploadLogs, Windows } from '@icon-park/react';
import { Button, Spinner, Tooltip } from '@heroui/react';

// 简单的分割线组件
const Divider = () => (
    <div className="h-4 w-[1px] bg-gray-200 mx-1" />
);

// 工具栏按钮组件
const ToolButton = ({ icon: Icon, tip }: {
    icon: React.ComponentType<any>,
    tip: string | React.ReactNode
}) => (
    <Tooltip color="foreground" content={tip} placement="bottom">
        <Button isIconOnly onPress={() => false} className=" bg-white  hover:bg-gray-100 rounded-md text-gray-500 transition-colors">
            <Icon theme="outline" size="24" fill="#333" />
        </Button>
    </Tooltip>
);


import { Bot } from '@/components/icons'

import {
    User, Brain, ChevronDown, ChevronRight,
    Copy, Check, FileText, BarChart, Table as TableIcon,
    Download, Loader2, Play
} from 'lucide-react';
import type { ChatMessage } from '@/types/messages';
import { apiService } from '@/lib/api';
import type { EventSourceMessage } from '@microsoft/fetch-event-source';
// --- A. 思考块 (Thought Block) ---
const ThoughtRenderer = ({ block }: { block: any }) => {
    const [isCollapsed, setIsCollapsed] = useState(block.isCollapsed ?? true);

    return (
        <div className="mb-4 rounded-lg border border-gray-200 bg-gray-50 overflow-hidden">
            <div
                onClick={() => setIsCollapsed(!isCollapsed)}
                className="flex items-center gap-2 px-3 py-2 cursor-pointer hover:bg-gray-100 transition-colors"
            >
                <Brain size={16} className="text-purple-500" />
                <span className="text-xs font-medium text-gray-500">
                    深度思考 {block.duration ? `(${block.duration}s)` : ''}
                </span>
                {isCollapsed ? <ChevronRight size={14} className="text-gray-400 ml-auto" /> : <ChevronDown size={14} className="text-gray-400 ml-auto" />}
            </div>

            {!isCollapsed && (
                <div className="px-3 py-3 border-t border-gray-200 text-sm text-gray-600 bg-white/50 italic leading-relaxed whitespace-pre-wrap animate-in slide-in-from-top-2">
                    {block.content}
                    {block.status === 'streaming' && <span className="animate-pulse inline-block w-1.5 h-3.5 bg-purple-400 ml-1 align-middle"></span>}
                </div>
            )}
        </div>
    );
};


// --- B. 文本块 (Text Block) ---
const TextRenderer = ({ block }: { block: any }) => {
    return (
        <div className="text-sm   text-gray-800 leading-relaxed whitespace-pre-wrap">
            {block.text}
            {/* 光标逻辑：只有状态是 streaming 时才显示 */}
            {block.status === 'streaming' && (
                <span className="inline-block w-1.5 h-4 bg-gray-800 ml-0.5 align-middle animate-pulse"></span>
            )}
        </div>
    );
};

// --- C. SQL 块 (SQL Block) ---
const SqlRenderer = ({ block }: { block: any }) => {
    const [copied, setCopied] = useState(false);

    const handleCopy = () => {
        navigator.clipboard.writeText(block.sql);
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
    };

    return (
        <div className="mb-4 rounded-md border border-gray-200 overflow-hidden shadow-sm">
            <div className="flex items-center justify-between px-3 py-1.5 bg-gray-100 border-b border-gray-200">
                <div className="flex items-center gap-1.5">
                    <div className="w-3 h-3 rounded-full bg-red-400" />
                    <div className="w-3 h-3 rounded-full bg-yellow-400" />
                    <div className="w-3 h-3 rounded-full bg-green-400" />
                    <span className="ml-2 text-xs font-mono text-gray-500">SQL Query</span>
                </div>
                <div className="flex items-center gap-2">
                    <button className="flex items-center gap-1 text-xs text-gray-500 hover:text-blue-600 transition-colors">
                        <Play size={12} /> 运行
                    </button>
                    <button onClick={handleCopy} className="text-gray-400 hover:text-gray-600 transition-colors">
                        {copied ? <Check size={14} className="text-green-500" /> : <Copy size={14} />}
                    </button>
                </div>
            </div>
            <div className="p-3 bg-[#1e1e1e] overflow-x-auto">
                <pre className="text-xs sm:text-sm font-mono text-gray-300">
                    <code>{block.sql}</code>
                    {block.status === 'streaming' && <span className="inline-block w-2 h-4 bg-gray-400 ml-1 align-middle animate-pulse" />}
                </pre>
            </div>
        </div>
    );
};

// --- D. 表格块 (Table Block) ---
const TableRenderer = ({ block }: { block: any }) => {
    return (
        <div className="mb-4 border border-gray-200 rounded-lg overflow-hidden shadow-sm">
            <div className="px-3 py-2 bg-gray-50 border-b border-gray-200 flex items-center gap-2">
                <TableIcon size={16} className="text-blue-500" />
                <span className="text-xs font-medium text-gray-600">数据预览</span>
            </div>
            <div className="overflow-x-auto">
                <table className="w-full text-sm text-left">
                    <thead className="text-xs text-gray-700 uppercase bg-gray-50/50">
                        <tr>
                            {block.columns.map((col: any) => (
                                <th key={col.key} className="px-4 py-3 font-medium whitespace-nowrap">
                                    {col.title}
                                </th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {block.item.map((row: any, i: number) => (
                            <tr key={i} className="bg-white border-b hover:bg-gray-50 last:border-0">
                                {block.columns.map((col: any) => (
                                    <td key={`${i}-${col.key}`} className="px-4 py-3 text-gray-600 whitespace-nowrap">
                                        {row[col.dataIndex]}
                                    </td>
                                ))}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            {block.pagination && (
                <div className="px-3 py-2 bg-gray-50 border-t border-gray-200 text-xs text-gray-500 flex justify-between">
                    <span>共 {block.pagination.total} 条</span>
                    <span>第 {block.pagination.current}/{Math.ceil(block.pagination.total / block.pagination.pageSize)} 页</span>
                </div>
            )}
        </div>
    );
};

// --- E. 图表块 (ECharts Block) ---
const ChartRenderer = ({ block }: { block: any }) => {
    return (
        <div className="mb-4 p-4 border border-gray-200 rounded-lg shadow-sm bg-white">
            <div className="flex items-center gap-2 mb-3">
                <BarChart size={18} className="text-indigo-500" />
                <span className="text-sm font-semibold text-gray-700">分析图表</span>
            </div>
            {/* 实际项目中这里应放置 <ReactECharts option={JSON.parse(block.options)} /> */}
            <div className="h-48 bg-gray-50 rounded border border-dashed border-gray-300 flex flex-col items-center justify-center text-gray-400">
                {block.status === 'streaming' ? (
                    <div className="flex flex-col items-center gap-2">
                        <Loader2 size={24} className="animate-spin text-indigo-400" />
                        <span className="text-xs">图表生成中...</span>
                    </div>
                ) : (
                    <>
                        <span className="text-xs">ECharts 渲染区域</span>
                        <span className="text-[10px] mt-1 font-mono max-w-[80%] truncate text-gray-300">{block.options}</span>
                    </>
                )}
            </div>
        </div>
    );
};

// --- F. 文件块 (File Block) ---
const FileRenderer = ({ block }: { block: any }) => {
    return (
        <div className="mb-4 flex items-center p-3 border border-gray-200 rounded-lg bg-gray-50 hover:bg-gray-100 transition-colors group cursor-pointer">
            <div className="w-10 h-10 rounded bg-red-100 flex items-center justify-center text-red-500 mr-3">
                <FileText size={20} />
            </div>
            <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-gray-900 truncate">{block.name}</p>
                <p className="text-xs text-gray-500">PDF 文档</p>
            </div>
            <button className="p-2 text-gray-400 hover:text-blue-600 transition-colors">
                <Download size={18} />
            </button>
        </div>
    )
}

// 消息块分发
const BlockDispatcher = ({ block }: { block: any }) => {
    switch (block.type) {
        case 'thought': return <ThoughtRenderer block={block} />;
        case 'text': return <TextRenderer block={block} />;
        case 'sql': return <SqlRenderer block={block} />;
        case 'table': return <TableRenderer block={block} />;
        case 'echarts': return <ChartRenderer block={block} />;
        case 'file': return <FileRenderer block={block} />;
        default: return <div className="text-red-500 text-xs">未知块类型: {block.type}</div>;
    }
};

// 消息处理
const MessageItem = ({ message }: { message: ChatMessage }) => {
    const isUser = message.role === 'user';
    const isAssistant = message.role === 'assistant';
    const isSys = message.role === 'system';

    let userTooltipContent = '';
    if (isUser) {
        userTooltipContent = '用户：你自己'
    }
    if (isAssistant) {
        userTooltipContent = 'SqlBotX'
    }
    if (isSys) {
        userTooltipContent = '系统提示'
    }

    return (
        <div className={`flex gap-2 w-full ${isUser ? 'flex-row-reverse' : 'flex-row'} mb-8 animate-in fade-in slide-in-from-bottom-2`}>

            {/* 头像 */}
            <Tooltip color="foreground" content={userTooltipContent} placement="top">
                <div className={`flex-shrink-0 w-9 h-9 rounded-full flex items-center justify-center    ${isUser ? 'shadow-sm' : ''}`}>

                    {isUser && (
                        <Me theme="outline" size="24" fill="#333" />
                    )}

                    {isAssistant && (
                        <Bot size={24} />
                    )}

                    {isSys && (
                        <Windows theme="outline" size="24" fill="#333" />
                    )}
                </div>
            </Tooltip>

            {/* 消息内容容器 */}
            <div className={`relative max-w-[85%] sm:max-w-[75%] px-0   flex flex-col
                ${isUser ? 'items-end' : 'items-start'}      `}>

                {/* 时间 */}
                <div className='text-[10px] p-2 text-gray-400'>
                    9:50:21
                </div>

                {/* 用户消息只是纯气泡，AI 消息是富文本块 */}
                {isUser ? (
                    <div className="text-sm  text-black px-5 py-3 rounded-2xl rounded-tr-sm shadow-sm">
                        {/* {message.content[0]?.text} */}
                        {message.content.map((block: any) => (
                            <BlockDispatcher key={block.id} block={block} />
                        ))}
                    </div>
                ) : (
                    <div className="w-full">
                        {/* 遍历 content 数组渲染 */}
                        {message.content.map((block: any) => (
                            <BlockDispatcher key={block.id} block={block} />
                        ))}

                        {/* 底部状态栏 (可选) */}
                        {message.status === 'streaming' && (
                            <div className="flex items-center gap-1.5 mt-2 text-xs text-gray-400">
                                <Loader2 size={12} className="animate-spin" />
                                <span>正在生成...</span>
                            </div>
                        )}
                    </div>
                )}

            </div>
        </div>
    );
};

export default function Main() {
    const [inputText, setInputText] = useState("");
    const [isWaitting, setWaitting] = useState(false);

    // --- 模拟数据 ---
    const aa = [
        {
            id: '1',
            role: 'user',
            content: [{ type: 'text', text: '请帮我分析一下2024年第一季度的销售数据，并给出SQL和图表。', status: 'done' }],
            status: 'done',
            createdAt: 123456
        },
        {
            id: '2',
            role: 'assistant',
            status: 'streaming', // 整体正在流式传输
            createdAt: 123457,
            content: [
                {
                    id: 'b1',
                    type: 'thought',
                    status: 'done',
                    createdAt: 0,
                    content: '用户需要2024 Q1的销售分析。\n1. 首先需要查询 Sales 表。\n2. 需要按月份分组统计。\n3. 生成 SQL 语句。\n4. 准备 ECharts 配置项。',
                    duration: 1.2,
                    isCollapsed: false // 默认展开让用户看
                },
                {
                    id: 'b2',
                    type: 'text',
                    status: 'done',
                    createdAt: 1,
                    text: '好的，我已经为您查询到了相关数据。首先是查询使用的 **SQL 语句**：'
                },
                {
                    id: 'b3',
                    type: 'sql', // 修正为 type
                    blockType: 'sql', // 兼容你的定义
                    status: 'done',
                    createdAt: 2,
                    sql: "SELECT \n  DATE_FORMAT(order_date, '%Y-%m') as month, \n  SUM(amount) as total_sales \nFROM orders \nWHERE order_date BETWEEN '2024-01-01' AND '2024-03-31' \nGROUP BY month;"
                },
                {
                    id: 'b4',
                    type: 'text',
                    status: 'done',
                    createdAt: 3,
                    text: '执行结果如下表所示：'
                },
                {
                    id: 'b5',
                    type: 'table',
                    status: 'done',
                    createdAt: 4,
                    columns: [
                        { title: '月份', dataIndex: 'month', key: 'm' },
                        { title: '销售额 (万元)', dataIndex: 'total_sales', key: 's' }
                    ],
                    item: [
                        { month: '2024-01', total_sales: 120 },
                        { month: '2024-02', total_sales: 98 },
                        { month: '2024-03', total_sales: 145 },
                    ],
                    pagination: { current: 1, pageSize: 10, total: 3 }
                },
                {
                    id: 'b6',
                    type: 'echarts',
                    status: 'streaming', // 这个块正在生成中
                    createdAt: 5,
                    options: '{"xAxis": {"type": "category", "data": ["Jan", "Feb", "Mar"]}, "yAxis": {"type": "value"}, "series": [{"data": [120, 98, 145], "type": "bar"}]}'
                }
            ]
        }
    ]
    const [mockMessages, setMockMessages] = useState<Array<ChatMessage>>(aa);

    // 自动滚动到底部
    const messagesEndRef = useRef<HTMLDivElement>(null);
    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [mockMessages]);

    // 发送信息
    const sendMsg = async function () {
        if (!inputText.trim()) return
        setWaitting(true)
        const timestamp = Number(new Date());
        const userMsg: ChatMessage = {
            id: null,
            role: 'user',
            content: [{ type: 'text', id: '', status: 'done', createdAt: timestamp, text: inputText }],
            status: 'done',
            createdAt: timestamp
        };
        setInputText('')
        setMockMessages((prev) => [...prev, userMsg]);

        let isFirst = false;
        //发送请求
        await apiService.sseEventSource(`/chat`, userMsg,
            //请求成功后
            () => {

            },
            //响应数据
            (msg: EventSourceMessage) => {

                //如果是流
                if (msg.event == 'delta') {

                }

                //如果是消息块
                if (msg.event == 'message') {
                    const botMsg = JSON.parse(msg.data) as ChatMessage;
                    console.log(botMsg)

                    setMockMessages((prev) => {
                        const exists = prev.some((m) => m.id === botMsg.id);
                        if (exists) {
                            return prev.map((m) => (m.id === botMsg.id ? botMsg : m));
                        } else {
                            return [...prev, botMsg];
                        }
                    });
                }

                //如果是会话
                if (msg.event == 'session') {
                    window.history.replaceState(null, '', `/chat/${msg.data}`);
                }

                if (isFirst == false) {

                    isFirst = true;
                }
            },
            () => {
                // setMockMessages((prev) => prev.slice(0, -1));
                setWaitting(false)
            }
        );

    }

    // 停止
    const doneMsg = function () {
        apiService.sseEventStop(() => {
            //如果存在最后一个加载中就移除，并且追加一个done
        });
    }

    return (
        <>
            {/* 对话 */}
            <main className="flex-1 overflow-y-auto p-4 sm:p-6 scroll-smooth ">
                <div className='container mx-auto  sm:max-w-6xl pt-10   pl-5 pr-5 sm:pl-0 sm:pr-0'>
                    {mockMessages.map((msg: any) => (
                        <MessageItem key={msg.id} message={msg} />
                    ))}
                </div>

                {/* 滚动到底部 */}
                <div ref={messagesEndRef} />
            </main>

            {/* 对话区域 */}
            <div className="w-full max-w-5xl mx-auto p-2">

                <div className="bg-white border border-gray-200 rounded-2xl shadow-sm p-3 relative hover:shadow-md transition-shadow duration-300">

                    {/* 上部分：输入框 */}
                    <textarea
                        value={inputText}
                        onChange={(e) => setInputText(e.target.value)}
                        placeholder="输入聊天内容，按 Ctrl Enter 键换行..."
                        className="w-full min-h-[60px] max-h-[200px] resize-none border-none outline-none text-gray-700 placeholder-gray-400 text-sm leading-relaxed bg-transparent px-1"
                        rows={2}
                    />

                    {/* 下部分：工具栏 */}
                    <div className="flex items-center justify-between mt-2 pt-2">

                        {/* 左侧工具组 */}
                        <div className="flex items-center gap-1">

                            <button className="p-1.5 rounded-full bg-pink-100 text-pink-500 hover:bg-pink-200 transition-colors mr-1">
                                <svg fill="#F86AA4" fill-rule="evenodd" height="22" viewBox="0 0 24 24" width="22" xmlns="http://www.w3.org/2000/svg" color="#fff" ><title>OpenAI</title><path d="M21.55 10.004a5.416 5.416 0 00-.478-4.501c-1.217-2.09-3.662-3.166-6.05-2.66A5.59 5.59 0 0010.831 1C8.39.995 6.224 2.546 5.473 4.838A5.553 5.553 0 001.76 7.496a5.487 5.487 0 00.691 6.5 5.416 5.416 0 00.477 4.502c1.217 2.09 3.662 3.165 6.05 2.66A5.586 5.586 0 0013.168 23c2.443.006 4.61-1.546 5.361-3.84a5.553 5.553 0 003.715-2.66 5.488 5.488 0 00-.693-6.497v.001zm-8.381 11.558a4.199 4.199 0 01-2.675-.954c.034-.018.093-.05.132-.074l4.44-2.53a.71.71 0 00.364-.623v-6.176l1.877 1.069c.02.01.033.029.036.05v5.115c-.003 2.274-1.87 4.118-4.174 4.123zM4.192 17.78a4.059 4.059 0 01-.498-2.763c.032.02.09.055.131.078l4.44 2.53c.225.13.504.13.73 0l5.42-3.088v2.138a.068.068 0 01-.027.057L9.9 19.288c-1.999 1.136-4.552.46-5.707-1.51h-.001zM3.023 8.216A4.15 4.15 0 015.198 6.41l-.002.151v5.06a.711.711 0 00.364.624l5.42 3.087-1.876 1.07a.067.067 0 01-.063.005l-4.489-2.559c-1.995-1.14-2.679-3.658-1.53-5.63h.001zm15.417 3.54l-5.42-3.088L14.896 7.6a.067.067 0 01.063-.006l4.489 2.557c1.998 1.14 2.683 3.662 1.529 5.633a4.163 4.163 0 01-2.174 1.807V12.38a.71.71 0 00-.363-.623zm1.867-2.773a6.04 6.04 0 00-.132-.078l-4.44-2.53a.731.731 0 00-.729 0l-5.42 3.088V7.325a.068.068 0 01.027-.057L14.1 4.713c2-1.137 4.555-.46 5.707 1.513.487.833.664 1.809.499 2.757h.001zm-11.741 3.81l-1.877-1.068a.065.065 0 01-.036-.051V6.559c.001-2.277 1.873-4.122 4.181-4.12.976 0 1.92.338 2.671.954-.034.018-.092.05-.131.073l-4.44 2.53a.71.71 0 00-.365.623l-.003 6.173v.002zm1.02-2.168L12 9.25l2.414 1.375v2.75L12 14.75l-2.415-1.375v-2.75z"></path></svg>
                            </button>

                            <ToolButton icon={UploadLogs} tip='上传附件' />

                            <Divider />

                        </div>

                        {/* 右侧发送组 */}
                        <div className="flex items-center gap-2">
                            {/* 发送按钮 */}
                            {
                                !isWaitting && (
                                    <Button
                                        onPress={sendMsg}
                                        isIconOnly
                                        className={`rounded-md transition-all duration-200 ${(inputText.trim() || isWaitting) ? 'bg-blue-600 text-white shadow-sm' : 'text-gray-400 bg-transparent cursor-not-allowed'}`}
                                        disabled={!inputText.trim() || isWaitting}
                                        isLoading={isWaitting}
                                    >
                                        <Send theme="outline" size={18} fill={inputText.trim() ? "currentColor" : "#333"} />
                                    </Button>
                                )
                            }
                            {/* 停止按钮 */}
                            {
                                isWaitting && (
                                    <Tooltip color="foreground" content='点击停止' placement="top">
                                        <Button
                                            onPress={doneMsg}
                                            isIconOnly className="grid place-items-center w-8 h-8 cursor-pointer relative group">
                                            <Spinner
                                                size="md"
                                                color="current"
                                                className="col-start-1 row-start-1"
                                            />
                                            <PauseOne
                                                theme="outline"
                                                size="24"
                                                fill="#333"
                                                className="col-start-1 row-start-1 z-10"
                                            />
                                        </Button>
                                    </Tooltip>
                                )
                            }

                        </div>

                    </div>
                </div>
            </div>
        </>


    );

}
