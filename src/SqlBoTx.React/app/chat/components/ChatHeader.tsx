import { Button } from "./ui/button";
import { Menu, Plus, MoreVertical, Share, Download } from "lucide-react";
import { motion } from "motion/react";

interface ChatHeaderProps {
  onToggleSidebar: () => void;
  onNewChat: () => void;
  chatTitle?: string;
}

export function ChatHeader({ onToggleSidebar, onNewChat, chatTitle }: ChatHeaderProps) {
  return (
    <motion.header
      initial={{ y: -50, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.3, ease: "easeOut" }}
      className="border-b border-gray-200 dark:border-gray-700 bg-white/90 dark:bg-gray-900/90 backdrop-blur-md supports-[backdrop-filter]:bg-white/60 dark:supports-[backdrop-filter]:bg-gray-900/60"
    >
      <div className="flex items-center justify-between h-14 sm:h-16 px-3 sm:px-4">
        <div className="flex items-center gap-2 sm:gap-4">
          {/* Mobile menu button */}
          <Button
            variant="ghost"
            size="sm"
            onClick={onToggleSidebar}
            className="lg:hidden p-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-100 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-xl transition-all duration-200 min-h-[44px] touch-manipulation"
          >
            <Menu className="w-5 h-5" />
          </Button>

          {/* Logo and title */}
          <div className="flex items-center gap-3 sm:gap-3 min-w-0">
            <div className="w-7 h-7 sm:w-8 sm:h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center shadow-lg shrink-0">
              <span className="text-white text-xs sm:text-sm font-bold">AI</span>
            </div>
            <div className="min-w-0 ">
              <h1 className="font-semibold text-gray-900 dark:text-gray-100 text-base sm:text-lg truncate">
                {chatTitle || "AI助手"}
              </h1>
              <p className="text-xs text-gray-500 dark:text-gray-400 -mt-1 hidden sm:block">
                智能对话助手 • 在线
              </p>
            </div>
            <div className="min-w-0">
              <h1 className="font-semibold text-orange-600">
                仅供技术服务相关人员服务，切勿滥用
              </h1>

            </div>
          </div>
        </div>

        {/* Right actions */}
        <div className="flex items-center gap-1 sm:gap-2">
          {/* Share button */}
          <Button
            variant="ghost"
            size="sm"
            className="hidden md:flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-100 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-xl transition-all duration-200 px-3 py-2 touch-manipulation"
          >
            <Share className="w-4 h-4" />
            <span className="text-sm">分享</span>
          </Button>

          {/* Export button */}
          <Button
            variant="ghost"
            size="sm"
            className="hidden md:flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-100 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-xl transition-all duration-200 px-3 py-2 touch-manipulation"
          >
            <Download className="w-4 h-4" />
            <span className="text-sm">导出</span>
          </Button>

          {/* New chat button */}
          <Button
            onClick={onNewChat}
            className="bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white border-0 rounded-xl shadow-lg hover:shadow-xl active:scale-95 transition-all duration-200 px-3 sm:px-4 py-2 min-h-[44px] touch-manipulation"
          >
            <Plus className="w-4 h-4" />
            <span className="hidden sm:inline ml-2">新对话</span>
          </Button>

          {/* More options */}
          <Button
            variant="ghost"
            size="sm"
            className="p-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-100 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-xl transition-all duration-200 min-h-[44px] touch-manipulation"
          >
            <MoreVertical className="w-4 h-4" />
          </Button>
        </div>
      </div>
    </motion.header>
  );
}