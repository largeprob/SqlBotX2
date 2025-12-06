import React, { useState, useRef, useEffect } from 'react';
import ReactECharts from 'echarts-for-react';
import { Light as SyntaxHighlighter } from 'react-syntax-highlighter';
// 直接指向具体的文件，避开 index.js
import atomOneDark from 'react-syntax-highlighter/dist/esm/styles/hljs/atom-one-dark';


import { Send, Mic, Plus, Smile, MoreVertical, Search, Star, Database, BarChart, Table as TableIcon, FileText } from 'lucide-react';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

import { apiService } from 'lib/api';
import type { BlockMessage, ContentItem, TableBlock, DeltaMessage, SqlBlock, SqlContentItem, SSEMessage, TableContentItem, EchartsBlock } from '@/types/messages';


// --- 工具函数 ---
function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

// --- 类型定义 ---
type MessageType = 'text' | 'sql' | 'table' | 'echarts';

interface TableData {
  columns: string[];
  rows: string[][];
}

enum MessageRole {
  USER = 0,
  Assistant = 1,
}

interface Message {
  id: string;
  role: MessageRole;
  type: MessageType;
  content: string | TableData | any;
  isLoading?: boolean;
}



// --- 子组件：特定类型的渲染块 ---

// 1. SQL 渲染块
const SqlBlock = ({ code }: { code: string }) => (
  <div className="w-full   overflow-hidden rounded-lg border border-gray-200 bg-[#282c34] shadow-sm">
    <div className="flex items-center justify-between bg-[#21252b] px-4 py-2 text-xs text-gray-400">
      <div className="flex items-center gap-2">
        <Database size={14} className="text-blue-400" />
        <span className="font-bold text-blue-400">SQL Generated</span>
      </div>
      <span className="cursor-pointer hover:text-white">Copy</span>
    </div>
    <div className="text-sm">
      <SyntaxHighlighter
        language="sql"
        style={atomOneDark}
        customStyle={{ margin: 0, padding: '1rem', background: 'transparent' }}
        wrapLongLines={true}
      >
        {code}
      </SyntaxHighlighter>
    </div>
  </div>
);

// 2. Table 渲染块
const TableBlock = ({ data }: { data: TableData }) => (
  <div className="w-full  overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm">
    <div className="flex items-center gap-2 border-b border-gray-100 bg-blue-50/50 px-4 py-2 text-xs text-blue-600">
      <TableIcon size={14} />
      <span className="font-bold">Data Table</span>
    </div>
    <div className="overflow-x-auto">
      <table className="w-full text-left text-sm text-gray-600">
        <thead className="bg-gray-50 text-xs font-semibold uppercase text-gray-500">
          <tr>
            {data.columns.map((col: any, idx) => (
              <th key={col.key} className="px-6 py-3">{col.lable}</th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100">
          {
            data.rows.map((row, rId) => (
              <tr key={rId} className="hover:bg-gray-50/50">
                {
                  data.columns.map((col: any, cId) => (
                    <td key={cId} className="px-6 py-3">{row[col.key]}</td>
                  ))
                }
              </tr>
            ))
          }
        </tbody>
      </table>
    </div>
  </div>
);

// 3. ECharts 渲染块
const ChartBlock = ({ options }: { options: any }) => (
  <div className="w-full overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm">
    <div className="flex items-center gap-2 border-b border-gray-100 bg-purple-50/50 px-4 py-2 text-xs text-purple-600">
      <BarChart size={14} />
      <span className="font-bold">ECharts Analytics</span>
    </div>
    <div className="p-4">
      <ReactECharts option={options} style={{ height: '500px', width: '100%' }} />
    </div>
  </div>
);

// 4. 加载动画组件
const TypingIndicator = () => (
  <div className="flex items-center gap-1 rounded-2xl rounded-tl-none bg-white px-4 py-3 shadow-sm border border-gray-100 w-fit">
    <div className="h-2 w-2 animate-bounce rounded-full bg-blue-400 [animation-delay:-0.3s]"></div>
    <div className="h-2 w-2 animate-bounce rounded-full bg-blue-400 [animation-delay:-0.15s]"></div>
    <div className="h-2 w-2 animate-bounce rounded-full bg-blue-400"></div>
  </div>
);

// --- 主页面组件 ---
export default function SqlBotChat() {
  const [messages, setMessages] = useState<Message[]>([
    {
      id: '1',
      role: MessageRole.Assistant,
      type: 'text',
      content: 'Hello! 我是你的 SQL 助手。我可以帮你查询数据、生成报表或绘制图表。请告诉我你想看什么？',
    },
  ]);

  const [inputValue, setInputValue] = useState('');
  const [isWaiting, setIsWaiting] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  // 自动滚动到底部
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages, isWaiting]);



  //发送消息
  const handleSendMessage = async () => {

    if (!inputValue.trim()) return;


    const userMsg: Message = {
      id: Date.now().toString(),
      role: MessageRole.USER,
      type: 'text',
      content: inputValue,
    };

    setMessages((prev) => [...prev, userMsg]);
    setInputValue('');
    setIsWaiting(true);

    await apiService.postStream(`/chat`, userMsg,
      () => {

      },
      (sse: SSEMessage) => {
        console.log('Received chunk:', sse)

        const SSEEventType = sse.type.toLocaleLowerCase();

        if (SSEEventType == 'delta') {
          const message = sse as DeltaMessage;
          const botMsg: Message = {
            id: (Date.now() + 1).toString(),
            role: MessageRole.Assistant,
            type: 'text',
            content: message.delta,
          };
          setMessages((prev) => [...prev, botMsg]);
        }

        if (SSEEventType == 'block') {
          const message = sse as BlockMessage;
          const contentItem = message.block as ContentItem;

          const botMsg: Message = {
            id: (Date.now() + 1).toString(),
            role: MessageRole.Assistant,
            type: 'text',
            content: '',
          }

          switch (contentItem.type) {
            case 'sql':
              const sqlblock = message.block as SqlBlock;
              botMsg.type = 'sql';
              botMsg.content = sqlblock.sql;
              break;
            case 'table':
              const tableBlock = message.block as TableBlock;
              botMsg.type = 'table';
              botMsg.content = {
                columns: tableBlock.columns,
                rows: tableBlock.rows,
              };
              break
            case 'echarts':
              const echartsBlock = message.block as EchartsBlock;
              botMsg.type = 'echarts';
              botMsg.content = echartsBlock.echartsOption ? JSON.parse(echartsBlock.echartsOption) : {};
              break;
            default:
              break;
          }
          console.log('Adding block message:', botMsg);
          setMessages((prev) => [...prev, botMsg]);
        }
        setIsWaiting(false);
      })
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  return (
    <div className="flex h-screen w-full flex-col bg-[#f0f4f8] font-sans text-gray-800">
      {/* --- Header --- */}
      <header className="flex items-center justify-between border-b border-gray-100 bg-white px-6 py-4 shadow-sm">
        <div className="flex items-center gap-3">
          <div className="relative">
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-gradient-to-br from-blue-500 to-purple-600 text-white shadow-md">
              <Database size={20} />
            </div>
            <span className="absolute bottom-0 right-0 h-3 w-3 rounded-full border-2 border-white bg-green-500"></span>
          </div>
          <div>
            <h1 className="text-lg font-bold text-gray-800">SQL Copilot</h1>
            <p className="text-xs text-green-600 font-medium">Online</p>
          </div>
        </div>
        <div className="flex items-center gap-4 text-gray-400">
          <button className="hover:text-blue-500"><Search size={20} /></button>
          <button className="hover:text-yellow-500"><Star size={20} /></button>
          <button className="hover:text-gray-600"><MoreVertical size={20} /></button>
        </div>
      </header>

      {/* --- Chat Area --- */}
      <main className="flex-1 w-full  overflow-y-auto px-4 py-6 sm:px-6">
        <div className="mx-auto flex max-w-7xl flex-col gap-6">

          {/* 日期分割线 */}
          <div className="flex items-center justify-center py-4">
            <span className="rounded-full bg-gray-200 px-3 py-1 text-xs text-gray-500">Today, 05:30 PM</span>
          </div>

          {messages.map((msg) => (
            <div
              key={msg.id}
              className={cn(
                "flex w-full",
                msg.role === MessageRole.USER ? "justify-end" : "justify-start"
              )}
            >
              {/* Avatar for Bot */}
              {msg.role === MessageRole.Assistant && (
                <div className="mr-3 flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-gradient-to-br from-blue-500 to-purple-600 text-white text-xs">
                  Bot
                </div>
              )}

              {/* Message Content Bubble */}
              <div
                className={cn(
                  "relative max-w-[85%] sm:max-w-[75%]",
                  msg.type === 'text'
                    ? (msg.role === MessageRole.USER
                      ? "bg-blue-600 text-white rounded-2xl rounded-tr-sm px-5 py-3 shadow-md"
                      : "bg-white text-gray-800 rounded-2xl rounded-tl-none px-5 py-3 shadow-sm border border-gray-100")
                    : "w-full" // 复杂组件如SQL/Table占宽一点
                )}
              >
                {/* Text Renderer */}
                {msg.type === 'text' && <p className="leading-relaxed whitespace-pre-wrap">{msg.content as string}</p>}

                {/* SQL Renderer */}
                {msg.type === 'sql' && <SqlBlock code={msg.content as string} />}

                {/* Table Renderer */}
                {msg.type === 'table' && <TableBlock data={msg.content as TableData} />}

                {/* Echarts Renderer */}
                {msg.type === 'echarts' && <ChartBlock options={msg.content} />}

                {/* Time Stamp inside bubble (optional style) */}
                {msg.type === 'text' && (
                  <span className={cn(
                    "mt-1 block text-[10px] opacity-70 text-right",
                    msg.role === MessageRole.USER ? "text-blue-100" : "text-gray-400"
                  )}>
                    05:32 PM
                  </span>
                )}
              </div>

              {/* Avatar for User (Optional, hidden in design typically, but spacing helps) */}
              {msg.role === MessageRole.USER && <div className="w-2" />}
            </div>
          ))}

          {/* Dynamic Waiting Animation */}
          {isWaiting && (
            <div className="flex w-full justify-start">
              <div className="mr-3 flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-gradient-to-br from-blue-500 to-purple-600 text-white text-xs">
                Bot
              </div>
              <TypingIndicator />
            </div>
          )}

          {/* Invisible div to scroll into view */}
          <div ref={messagesEndRef} />
        </div>
      </main>

      {/* --- Input Area --- */}
      <footer className="bg-white px-4 py-4 shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.05)]">
        <div className="mx-auto flex max-w-4xl items-center gap-3">
          {/* Action Buttons */}
          <button className="flex h-10 w-10 items-center justify-center rounded-full text-gray-400 hover:bg-gray-100 hover:text-blue-500 transition">
            <Plus size={22} />
          </button>

          {/* Input Field */}
          <div className="relative flex-1">
            <input
              type="text"
              className="w-full rounded-full border border-gray-200 bg-gray-50 py-3 pl-5 pr-12 text-gray-700 placeholder-gray-400 focus:border-blue-500 focus:bg-white focus:outline-none focus:ring-2 focus:ring-blue-100 transition-all"
              placeholder="Ask for data, sql or charts..."
              value={inputValue}
              onChange={(e) => setInputValue(e.target.value)}
              onKeyDown={handleKeyDown}
            />
            <div className="absolute right-2 top-1/2 flex -translate-y-1/2 items-center gap-1 pr-2">
              <button className="p-2 text-gray-400 hover:text-yellow-500 transition">
                <Smile size={20} />
              </button>
            </div>
          </div>

          {/* Voice/Send Button */}
          {inputValue.trim() ? (
            <button
              onClick={handleSendMessage}
              className="flex h-12 w-12 items-center justify-center rounded-full bg-blue-600 text-white shadow-lg hover:bg-blue-700 hover:scale-105 transition-all active:scale-95"
            >
              <Send size={20} className="ml-1" />
            </button>
          ) : (
            <button className="flex h-12 w-12 items-center justify-center rounded-full bg-blue-50 text-blue-600 hover:bg-blue-100 transition">
              <Mic size={22} />
            </button>
          )}
        </div>

      </footer>
    </div>
  );
}