import { Avatar, AvatarFallback } from "./ui/avatar";
import { Button } from "./ui/button";
import { Copy, ThumbsUp, ThumbsDown, User, Bot, Check } from "lucide-react";
import { motion } from "motion/react";
import { useState, useEffect } from "react";
import { copyTextLegacy, supportsLegacyCopy, showCopyDialog } from "./utils/simpleCopy";

interface ChatMessageProps {
  role: "user" | "assistant";
  content: string;
  timestamp: string;
  isLoading?: boolean;
}

export function ChatMessage({ role, content, timestamp, isLoading }: ChatMessageProps) {
  const isUser = role === "user";
  const [copySuccess, setCopySuccess] = useState(false);
  const [showCopyButton, setShowCopyButton] = useState(false);

  useEffect(() => {
    // 在客户端检查复制支持，总是显示复制按钮
    setShowCopyButton(true); // 我们总是显示复制按钮，但使用不同的策略
  }, []);

  const handleCopy = async () => {
    // 首先尝试简单的 execCommand 方法
    if (supportsLegacyCopy()) {
      const success = copyTextLegacy(content);
      if (success) {
        setCopySuccess(true);
        setTimeout(() => setCopySuccess(false), 2000);
        return;
      }
    }

    // 如果 execCommand 不可用或失败，显示复制对话框
    showCopyDialog(content);
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4, ease: "easeOut" }}
      className={`w-full px-4 py-6`}
    >
      <div className="max-w-4xl mx-auto">
        <div className={`flex gap-4 ${isUser ? "flex-row-reverse" : ""}`}>
          {/* Avatar */}
          <div className="flex-shrink-0">
            <div className={`
              w-10 h-10 rounded-full flex items-center justify-center
              ${isUser
                ? "bg-gradient-to-br from-blue-500 to-purple-600 text-white shadow-lg"
                : "bg-gradient-to-br from-emerald-500 to-teal-600 text-white shadow-lg"
              }
            `}>
              {isUser ? (
                <User className="w-5 h-5" />
              ) : (
                <Bot className="w-5 h-5" />
              )}
            </div>
          </div>

          {/* Message Content */}
          <div className={`flex-1 min-w-0 ${isUser ? "text-right" : ""}`}>
            {/* Header */}
            <div className={`flex items-center gap-2 mb-2 ${isUser ? "justify-end" : ""}`}>
              <span className="font-semibold text-sm text-gray-900 dark:text-gray-100">
                {isUser ? "你" : "AI助手"}
              </span>
              <span className="text-xs text-gray-500 dark:text-gray-400">
                {timestamp}
              </span>
            </div>

            {/* Message Bubble */}
            <div className={`
              inline-block max-w-full break-words
              ${isUser
                ? "bg-gradient-to-br from-blue-500 to-purple-600 text-white rounded-2xl rounded-tr-md px-4 py-3 shadow-lg"
                : "bg-white dark:bg-gray-700 text-gray-800 dark:text-gray-200 rounded-2xl rounded-tl-md px-4 py-3 shadow-sm border border-gray-200 dark:border-gray-600"
              }
            `}>
              {isLoading ? (
                <div className="flex items-center gap-2 py-2">
                  <div className="flex space-x-1">
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce delay-75"></div>
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce delay-150"></div>
                  </div>
                  <span className="text-sm text-gray-500 dark:text-gray-400 ml-2">正在思考...</span>
                </div>
              ) : (
                <div className="whitespace-pre-wrap leading-relaxed">
                  {content}
                </div>
              )}
            </div>

            {/* Action Buttons for AI messages */}
            {!isUser && !isLoading && (
              <div className="flex items-center gap-1 mt-3">
                {showCopyButton && (
                  <Button
                    variant="ghost"
                    size="sm"
                    className={`h-8 px-2 transition-all duration-200 ${copySuccess
                      ? "text-green-600 dark:text-green-400"
                      : "text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
                      }`}
                    onClick={handleCopy}
                  >
                    <motion.div
                      initial={false}
                      animate={{ scale: copySuccess ? 1.1 : 1 }}
                      transition={{ duration: 0.2 }}
                    >
                      {copySuccess ? (
                        <Check className="w-3.5 h-3.5" />
                      ) : (
                        <Copy className="w-3.5 h-3.5" />
                      )}
                    </motion.div>
                    <span className="ml-1 text-xs">
                      {copySuccess ? "已复制" : "复制"}
                    </span>
                  </Button>
                )}
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-8 px-2 text-gray-500 hover:text-green-600 dark:text-gray-400 dark:hover:text-green-400 transition-colors"
                >
                  <ThumbsUp className="w-3.5 h-3.5" />
                </Button>
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-8 px-2 text-gray-500 hover:text-red-500 dark:text-gray-400 dark:hover:text-red-400 transition-colors"
                >
                  <ThumbsDown className="w-3.5 h-3.5" />
                </Button>
              </div>
            )}
          </div>
        </div>
      </div>
    </motion.div>
  );
}