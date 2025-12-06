import { useState, useEffect } from "react";
import { ChatSidebar } from "./components/ChatSidebar";
import { ChatArea } from "./components/ChatArea";
import { ChatInput } from "./components/ChatInput";
import { ChatHeader } from "./components/ChatHeader";
import "./index.css";
import { apiService } from "lib/api";



interface Message {
    id: string;
    role: "user" | "assistant";
    content: string;
    timestamp: string;
}

interface ChatHistory {
    id: string;
    title: string;
    timestamp: string;
    messages: Message[];
}

export default function App() {
    const [isDarkMode, setIsDarkMode] = useState(false);
    const [isSidebarOpen, setIsSidebarOpen] = useState(false); // 默认关闭侧边栏在移动端
    const [chatHistory, setChatHistory] = useState<ChatHistory[]>([]);
    const [currentChatId, setCurrentChatId] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    // 获取当前聊天的消息
    const currentMessages = currentChatId
        ? chatHistory.find(chat => chat.id === currentChatId)?.messages || []
        : [];

    // 初始化示例数据
    useEffect(() => {
        // const exampleChats: ChatHistory[] = [
        //     {
        //         id: "1",
        //         title: "关于AI的讨论",
        //         timestamp: "今天 14:30",
        //         messages: [
        //             {
        //                 id: "1",
        //                 role: "user",
        //                 content: "什么是人工智能？",
        //                 timestamp: "14:30"
        //             },
        //             {
        //                 id: "2",
        //                 role: "assistant",
        //                 content: "人工智能（AI）是计算机科学的一个分支，致力于创建能够执行通常需要人类智能的任务的系统。这包括学习、推理、问题解决、感知和语言理解等能力。",
        //                 timestamp: "14:31"
        //             }
        //         ]
        //     }
        // ];
        // setChatHistory(exampleChats);
        // setCurrentChatId("1");
    }, []);

    // 切换主题
    const toggleTheme = () => {
        setIsDarkMode(!isDarkMode);
        document.documentElement.classList.toggle("dark");
    };

    // 创建新对话
    const createNewChat = () => {
        const newChatId = Date.now().toString();
        const newChat: ChatHistory = {
            id: newChatId,
            title: "新对话",
            timestamp: new Date().toLocaleTimeString(),
            messages: []
        };
        setChatHistory(prev => [newChat, ...prev]);
        setCurrentChatId(newChatId);
        setIsSidebarOpen(false); // 在移动端创建新对话后关闭侧边栏
    };

    // 选择聊天
    const selectChat = (chatId: string) => {
        setCurrentChatId(chatId);
        setIsSidebarOpen(false); // 在移动端选择聊天后关闭侧边栏
    };

    // 编辑聊天标题
    const editChat = (chatId: string, newTitle: string) => {
        setChatHistory(prev => prev.map(chat =>
            chat.id === chatId
                ? { ...chat, title: newTitle }
                : chat
        ));
    };

    // 删除聊天
    const deleteChat = (chatId: string) => {
        setChatHistory(prev => prev.filter(chat => chat.id !== chatId));
        // 如果删除的是当前聊天，切换到第一个可用的聊天或创建新聊天
        if (currentChatId === chatId) {
            const remainingChats = chatHistory.filter(chat => chat.id !== chatId);
            if (remainingChats.length > 0) {
                setCurrentChatId(remainingChats[0].id);
            } else {
                setCurrentChatId(null);
            }
        }
    };

    // 发送消息
    const sendMessage = async (content: string) => {


        setIsLoading(true);

        let chatId = currentChatId;

        // 如果没有当前聊天，创建新的聊天
        if (!chatId) {
            const newChatId = Date.now().toString();
            const newChat: ChatHistory = {
                id: newChatId,
                title: content.slice(0, 30) + (content.length > 30 ? "..." : ""),
                timestamp: new Date().toLocaleTimeString(),
                messages: []
            };
            setChatHistory(prev => [newChat, ...prev]);
            setCurrentChatId(newChatId);
            chatId = newChatId;
        }

        // 记录发送消息前，判断该对话是否原本为空（用于设置标题）
        const prevChat = chatHistory.find(c => c.id === chatId);
        const wasEmpty = !prevChat || prevChat.messages.length === 0;

        // 添加用户消息
        const userMessage: Message = {
            id: Date.now().toString(),
            role: "user",
            content,
            timestamp: new Date().toLocaleTimeString()
        };
        setChatHistory(prev => prev.map(chat =>
            chat.id === chatId
                ? { ...chat, messages: [...chat.messages, userMessage] }
                : chat
        ));

        // 如果是第一条消息，更新聊天标题
        if (wasEmpty) {
            setChatHistory(prev => prev.map(chat =>
                chat.id === chatId
                    ? { ...chat, title: content.slice(0, 30) + (content.length > 30 ? "..." : "") }
                    : chat
            ));
        }

        // 添加 assistant 占位消息，用于流式更新内容
        const assistantId = (Date.now() + 1).toString();

        const modelId = 'DeepSeek-V3.1_NoThink';
        await apiService.postStream(`/Chat/Message/Stream/${modelId}?ask=${content}`, null,
            //成功回调
            () => {
                setIsLoading(false);
                const assistantMessage: Message = {
                    id: assistantId,
                    role: "assistant",
                    content: "",
                    timestamp: new Date().toLocaleTimeString()
                };
                setChatHistory(prev => prev.map(chat =>
                    chat.id === chatId
                        ? { ...chat, messages: [...chat.messages, assistantMessage] }
                        : chat
                ));
            },
            //获取到流数据回调
            (chunk: string) => {
                setChatHistory(prev => prev.map(chat =>
                    chat.id === chatId
                        ? {
                            ...chat,
                            messages: chat.messages.map(m =>
                                m.id === assistantId ? { ...m, content: m.content + chunk } : m
                            )
                        }
                        : chat
                ));
            });

    };

    return (
        <div className={`h-screen flex ${isDarkMode ? "dark" : ""} overflow-hidden touch-pan-y`}>
            {/* 侧边栏 */}
            <div className={`${isSidebarOpen ? "translate-x-0" : "-translate-x-full"} lg:translate-x-0 lg:block fixed lg:relative z-50 lg:z-auto h-full transition-transform duration-300 ease-in-out`}>
                <ChatSidebar
                    chatHistory={chatHistory}
                    currentChatId={currentChatId}
                    onNewChat={createNewChat}
                    onSelectChat={selectChat}
                    onEditChat={editChat}
                    onDeleteChat={deleteChat}
                    isDarkMode={isDarkMode}
                    onToggleTheme={toggleTheme}
                />
            </div>

            {/* 移动端遮罩 */}
            {isSidebarOpen && (
                <div
                    className="fixed inset-0 bg-black/50 backdrop-blur-sm z-40 lg:hidden transition-all duration-300"
                    onClick={() => setIsSidebarOpen(false)}
                />
            )}

            {/* 主内容区 */}
            <div className="flex-1 flex flex-col min-w-0 bg-white dark:bg-gray-900 transition-colors duration-200 h-full overflow-hidden">
                <ChatHeader
                    onToggleSidebar={() => setIsSidebarOpen(!isSidebarOpen)}
                    onNewChat={createNewChat}
                    chatTitle={currentChatId ? chatHistory.find(c => c.id === currentChatId)?.title : undefined}
                />

                <ChatArea
                    messages={currentMessages}
                    isLoading={isLoading}
                    onSendMessage={sendMessage}
                />

                <ChatInput
                    onSendMessage={sendMessage}
                    isLoading={isLoading}
                    placeholder="输入您的问题..."
                />
            </div>
        </div>
    );
}