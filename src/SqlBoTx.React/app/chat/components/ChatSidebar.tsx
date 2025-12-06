import { Plus, MessageSquare, Settings, Moon, Sun, Search, Edit3, Trash2, Check, X } from "lucide-react";
import { Button } from "./ui/button";
import { ScrollArea } from "./ui/scroll-area";
import { motion } from "motion/react";
import { useState, useRef, useEffect } from "react";

interface ChatHistory {
  id: string;
  title: string;
  timestamp: string;
}

interface ChatSidebarProps {
  chatHistory: ChatHistory[];
  currentChatId: string | null;
  onNewChat: () => void;
  onSelectChat: (chatId: string) => void;
  onEditChat: (chatId: string, newTitle: string) => void;
  onDeleteChat: (chatId: string) => void;
  isDarkMode: boolean;
  onToggleTheme: () => void;
}

export function ChatSidebar({ 
  chatHistory, 
  currentChatId, 
  onNewChat, 
  onSelectChat, 
  onEditChat,
  onDeleteChat,
  isDarkMode, 
  onToggleTheme 
}: ChatSidebarProps) {
  const [hoveredChat, setHoveredChat] = useState<string | null>(null);
  const [editingChat, setEditingChat] = useState<string | null>(null);
  const [editTitle, setEditTitle] = useState("");
  const [deletingChat, setDeletingChat] = useState<string | null>(null);
  const editInputRef = useRef<HTMLInputElement>(null);

  // 聚焦到编辑输入框
  useEffect(() => {
    if (editingChat && editInputRef.current) {
      editInputRef.current.focus();
      editInputRef.current.select();
    }
  }, [editingChat]);

  // 开始编辑
  const startEdit = (chat: ChatHistory) => {
    setEditingChat(chat.id);
    setEditTitle(chat.title);
  };

  // 保存编辑
  const saveEdit = () => {
    if (editingChat && editTitle.trim()) {
      onEditChat(editingChat, editTitle.trim());
    }
    setEditingChat(null);
    setEditTitle("");
  };

  // 取消编辑
  const cancelEdit = () => {
    setEditingChat(null);
    setEditTitle("");
  };

  // 处理键盘事件
  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter") {
      saveEdit();
    } else if (e.key === "Escape") {
      cancelEdit();
    }
  };

  // 确认删除
  const confirmDelete = (chatId: string) => {
    setDeletingChat(chatId);
  };

  // 执行删除
  const executeDelete = () => {
    if (deletingChat) {
      onDeleteChat(deletingChat);
      setDeletingChat(null);
    }
  };

  // 取消删除
  const cancelDelete = () => {
    setDeletingChat(null);
  };

  return (
    <motion.div 
      initial={{ x: -300 }}
      animate={{ x: 0 }}
      transition={{ type: "spring", damping: 20, stiffness: 100 }}
      className="w-72 sm:w-80 h-screen bg-white dark:bg-gray-900 border-r border-gray-200 dark:border-gray-700 flex flex-col shadow-xl"
    >
      {/* Header */}
      <div className="p-4 sm:p-6 border-b border-gray-200 dark:border-gray-700 pt-safe">
        <div className="flex items-center gap-3 mb-4">
          <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
            <span className="text-white text-sm font-bold">AI</span>
          </div>
          <div>
            <h1 className="font-semibold text-gray-900 dark:text-gray-100">AI助手</h1>
            <p className="text-xs text-gray-500 dark:text-gray-400">智能对话</p>
          </div>
        </div>
        
        <Button 
          onClick={onNewChat}
          className="w-full justify-center gap-2 bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white border-0 rounded-xl shadow-lg hover:shadow-xl active:scale-95 transition-all duration-200 min-h-[44px] touch-manipulation"
        >
          <Plus className="w-4 h-4" />
          新建对话
        </Button>
      </div>

      {/* Search */}
      <div className="p-3 sm:p-4 border-b border-gray-200 dark:border-gray-700">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            type="text"
            placeholder="搜索对话..."
            className="w-full pl-10 pr-4 py-2.5 sm:py-2 bg-gray-50 dark:bg-gray-800 border border-gray-200 dark:border-gray-600 rounded-lg text-sm text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 transition-all duration-200 touch-manipulation"
          />
        </div>
      </div>

      {/* Chat History */}
      <ScrollArea className="flex-1">
        <div className="p-2">
          <div className="text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider mb-3 px-3">
            最近对话
          </div>
          <div className="space-y-1">
            {chatHistory.map((chat) => (
              <motion.div
                key={chat.id}
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.2 }}
                className="relative group"
                onMouseEnter={() => setHoveredChat(chat.id)}
                onMouseLeave={() => setHoveredChat(null)}
              >
                {editingChat === chat.id ? (
                  // 编辑模式
                  <div className="w-full p-3 bg-gray-50 dark:bg-gray-800 rounded-xl border-2 border-blue-500 dark:border-blue-400">
                    <div className="flex items-center gap-2">
                      <MessageSquare className="w-4 h-4 shrink-0 text-blue-600 dark:text-blue-400" />
                      <input
                        ref={editInputRef}
                        value={editTitle}
                        onChange={(e) => setEditTitle(e.target.value)}
                        onKeyDown={handleKeyDown}
                        className="flex-1 bg-transparent border-none outline-none text-sm font-medium text-gray-900 dark:text-gray-100"
                        placeholder="输入对话标题..."
                      />
                    </div>
                    <div className="flex items-center justify-between mt-2">
                      <div className="text-xs text-gray-500 dark:text-gray-400">{chat.timestamp}</div>
                      <div className="flex gap-1">
                        <Button
                          variant="ghost"
                          size="sm"
                          className="h-6 w-6 p-0 text-green-600 hover:text-green-700 dark:text-green-400 dark:hover:text-green-300"
                          onClick={saveEdit}
                        >
                          <Check className="w-3 h-3" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="h-6 w-6 p-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
                          onClick={cancelEdit}
                        >
                          <X className="w-3 h-3" />
                        </Button>
                      </div>
                    </div>
                  </div>
                ) : (
                  // 正常显示模式
                  <>
                    <Button
                      variant="ghost"
                      className={`
                        w-full justify-start gap-3 h-auto py-3 px-3 rounded-xl transition-all duration-200
                        ${currentChatId === chat.id 
                          ? "bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/30 dark:to-purple-900/30 text-blue-700 dark:text-blue-300 shadow-sm" 
                          : "text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800"
                        }
                      `}
                      onClick={() => onSelectChat(chat.id)}
                    >
                      <MessageSquare className={`w-4 h-4 shrink-0 ${currentChatId === chat.id ? "text-blue-600 dark:text-blue-400" : "text-gray-400"}`} />
                      <div className="flex-1 text-left truncate">
                        <div className="truncate font-medium">{chat.title}</div>
                        <div className="text-xs text-gray-500 dark:text-gray-400">{chat.timestamp}</div>
                      </div>
                    </Button>
                    
                    {/* Action buttons */}
                    {hoveredChat === chat.id && (
                      <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        className="absolute right-2 top-1/2 transform -translate-y-1/2 flex gap-1"
                      >
                        <Button
                          variant="ghost"
                          size="sm"
                          className="h-6 w-6 p-0 text-gray-400 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                          onClick={(e) => {
                            e.stopPropagation();
                            startEdit(chat);
                          }}
                        >
                          <Edit3 className="w-3 h-3" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="h-6 w-6 p-0 text-gray-400 hover:text-red-500 transition-colors"
                          onClick={(e) => {
                            e.stopPropagation();
                            confirmDelete(chat.id);
                          }}
                        >
                          <Trash2 className="w-3 h-3" />
                        </Button>
                      </motion.div>
                    )}
                  </>
                )}
              </motion.div>
            ))}
          </div>
        </div>
      </ScrollArea>

      {/* Footer */}
      <div className="p-4 border-t border-gray-200 dark:border-gray-700 space-y-2">
        <Button
          variant="ghost"
          className="w-full justify-start gap-3 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 rounded-xl transition-all duration-200"
          onClick={onToggleTheme}
        >
          <div className="w-8 h-8 rounded-lg bg-gray-100 dark:bg-gray-800 flex items-center justify-center">
            {isDarkMode ? <Sun className="w-4 h-4" /> : <Moon className="w-4 h-4" />}
          </div>
          <span>{isDarkMode ? "浅色模式" : "深色模式"}</span>
        </Button>
        
        <Button
          variant="ghost"
          className="w-full justify-start gap-3 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 rounded-xl transition-all duration-200"
        >
          <div className="w-8 h-8 rounded-lg bg-gray-100 dark:bg-gray-800 flex items-center justify-center">
            <Settings className="w-4 h-4" />
          </div>
          <span>设置</span>
        </Button>
      </div>

      {/* 删除确认对话框 */}
      {deletingChat && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50">
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            className="bg-white dark:bg-gray-800 rounded-2xl p-6 mx-4 max-w-sm w-full shadow-2xl"
          >
            <div className="flex items-center gap-3 mb-4">
              <div className="w-10 h-10 bg-red-100 dark:bg-red-900/30 rounded-full flex items-center justify-center">
                <Trash2 className="w-5 h-5 text-red-600 dark:text-red-400" />
              </div>
              <div>
                <h3 className="font-semibold text-gray-900 dark:text-gray-100">删除对话</h3>
                <p className="text-sm text-gray-500 dark:text-gray-400">此操作无法撤销</p>
              </div>
            </div>
            
            <p className="text-gray-700 dark:text-gray-300 mb-6">
              确定要删除这个对话吗？所有消息将永久丢失。
            </p>
            
            <div className="flex gap-3">
              <Button
                variant="outline"
                className="flex-1"
                onClick={cancelDelete}
              >
                取消
              </Button>
              <Button
                className="flex-1 bg-red-600 hover:bg-red-700 text-white border-0"
                onClick={executeDelete}
              >
                删除
              </Button>
            </div>
          </motion.div>
        </div>
      )}
    </motion.div>
  );
}