import React, { useState } from 'react';
import { ScrollShadow } from "@heroui/scroll-shadow";
import { Button } from "@heroui/button";

// --- 图标组件 (SVG) ---
const SearchIcon = () => (
    <svg className="w-4 h-4 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
    </svg>
);

const MoreIcon = () => (
    <svg className="w-5 h-5 text-gray-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 12h.01M12 12h.01M19 12h.01M6 12a1 1 0 11-2 0 1 1 0 012 0zm7 0a1 1 0 11-2 0 1 1 0 012 0zm7 0a1 1 0 11-2 0 1 1 0 012 0z" />
    </svg>
);

const WalletIcon = () => (
    <svg className="w-5 h-5 text-indigo-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
    </svg>
);

const ChatIcon = () => (
    <svg className="w-4 h-4 text-gray-400 mt-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
    </svg>
);

// --- 模拟数据 ---
const mockHistory = [
    { id: 1, title: "React 组件优化讨论", date: "刚刚" },
    { id: 2, title: "Tailwind CSS 布局技巧", date: "2小时前" },
    { id: 3, title: "翻译：如何使用 Next.js", date: "昨天" },
    { id: 4, title: "解释量子力学的基本原理解释量子力学的基本原理", date: "3天前" }, // 测试长文本截断
    { id: 5, title: "Python 爬虫脚本调试", date: "1周前" },
    { id: 6, title: "周报生成助手", date: "2周前" },
    { id: 7, title: "SQL 语句优化", date: "1个月前" },
    { id: 8, title: "旅游攻略规划", date: "1个月前" },
    { id: 9, title: "旅游攻略规划", date: "1个月前" },
    { id: 10, title: "旅游攻略规划", date: "1个月前" },
    { id: 11, title: "旅游攻略规划", date: "1个月前" },
    { id: 12, title: "旅游攻略规划", date: "1个月前" },
    { id: 13, title: "旅游攻略规划111", date: "1个月前" },
];

export default function ChatHistory() {
    const [activeId, setActiveId] = useState(1);

    return (
        // 外层容器：固定宽度，全高，Flex 纵向布局
        <div className="flex flex-col w-full h-screen gap-2">

            <div>
                <Button className="w-full" color="secondary">发起新对话</Button>
            </div>

            {/* 2. 中间：资产区域 */}
            <div className="flex-shrink-0">
                <div className="bg-white p-3 rounded-xl border border-gray-200 shadow-sm">
                    <div className="flex items-center justify-between mb-2">
                        <span className="text-xs font-semibold text-gray-500 uppercase tracking-wider">我的资产</span>
                        <span className="text-xs text-indigo-600 bg-indigo-50 px-2 py-0.5 rounded-full cursor-pointer hover:bg-indigo-100">充值</span>
                    </div>
                    <div className="flex items-center gap-3">
                        <div className="p-2 bg-indigo-50 rounded-lg">
                            <WalletIcon />
                        </div>
                        <div>
                            <div className="text-sm font-bold text-gray-800">2,450 积分</div>
                            <div className="text-xs text-gray-400">Pro 会员生效中</div>
                        </div>
                    </div>
                </div>
            </div>

            {/* 3 发起新对话 */}


            {/* 4. 底部：历史对话列表 (占据剩余空间 + 滚动) */}
            <div className="px-4 py-2">
                <h3 className="text-xs font-medium text-gray-400">历史对话</h3>
            </div>

            {/* 1. 顶部：搜索区域 */}
            <div className="flex-shrink-0">
                <div className="relative">
                    <input
                        type="text"
                        placeholder="搜索历史记录..."
                        className="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 transition-all"
                    />
                    <div className="absolute left-3 top-2.5">
                        <SearchIcon />
                    </div>
                </div>
            </div>

            <div className="flex-1 overflow-y-auto p-4 scroll-smooth">
                {mockHistory.map((item) => (
                    <div
                        key={item.id}
                        onClick={() => setActiveId(item.id)}
                        className={`
              group flex items-center justify-between p-3 rounded-lg cursor-pointer transition-colors duration-200
              ${activeId === item.id
                                ? 'bg-white shadow-sm border border-gray-100'
                                : 'hover:bg-gray-200/50 border border-transparent'}
            `}
                    >
                        {/* 左侧：图标 + 文本 */}
                        <div className="flex items-start gap-3 overflow-hidden">
                            <div className="min-w-0 flex flex-col">
                                <span className={`text-sm truncate ${activeId === item.id ? 'font-medium text-gray-900' : 'text-gray-700'}`}>
                                    {item.title}
                                </span>
                                <span className="text-xs text-gray-400 mt-0.5">{item.date}</span>
                            </div>
                        </div>

                        {/* 右侧：更多按钮 (悬停时显示效果更佳，这里设置为常显但低对比度，悬停高亮) */}
                        <button
                            className={`
                p-1 rounded-md transition-all
                ${activeId === item.id ? 'opacity-100 hover:bg-gray-100' : 'opacity-0 group-hover:opacity-100 hover:bg-gray-300'}
              `}
                            onClick={(e) => {
                                e.stopPropagation(); // 防止触发外层的 item 点击
                                alert(`点击了 "${item.title}" 的更多选项`);
                            }}
                        >
                            <MoreIcon />
                        </button>
                    </div>
                ))}
            </div>

        
            
        </div>
    );
}