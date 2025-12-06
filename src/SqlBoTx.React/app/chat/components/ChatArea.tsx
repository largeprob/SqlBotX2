import { useEffect, useRef } from "react";
import { ScrollArea } from "./ui/scroll-area";
import { ChatMessage } from "./ChatMessage";
import { motion } from "motion/react";
import { Sparkles, Lightbulb, FileText, Search, Target, Zap, Globe, Code } from "lucide-react";

interface Message {
  id: string;
  role: "user" | "assistant";
  content: string;
  timestamp: string;
}

interface ChatAreaProps {
  messages: Message[];
  isLoading?: boolean;
  onSendMessage?: (message: string) => void;
}

const suggestions = [
  {
    icon: Lightbulb,
    title: "创意想法",
    description: "帮我想一些创意点子",
    color: "from-yellow-500 to-orange-500"
  },
  {
    icon: FileText,
    title: "文本编写",
    description: "协助我写文章或文案",
    color: "from-blue-500 to-indigo-500"
  },
  {
    icon: Search,
    title: "信息查询",
    description: "查找和整理资料",
    color: "from-green-500 to-emerald-500"
  },
  {
    icon: Target,
    title: "问题解答",
    description: "解答疑惑和难题",
    color: "from-purple-500 to-pink-500"
  },
  {
    icon: Code,
    title: "编程助手",
    description: "代码编写和调试",
    color: "from-red-500 to-rose-500"
  },
  {
    icon: Globe,
    title: "语言翻译",
    description: "多语言翻译服务",
    color: "from-cyan-500 to-blue-500"
  }
];

export function ChatArea({ messages, isLoading, onSendMessage }: ChatAreaProps) {
  const scrollRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (scrollRef.current) {
      const scrollContainer = scrollRef.current.querySelector('[data-radix-scroll-area-viewport]');
      if (scrollContainer) {
        scrollContainer.scrollTop = scrollContainer.scrollHeight;
      }
    }
  }, [messages, isLoading]);

  return (
    <div className="flex-1 flex flex-col bg-gray-50/30 dark:bg-gray-800/30 overflow-hidden">
      <ScrollArea className="flex-1 h-0" ref={scrollRef}>
        {messages.length === 0 ? (
          <div className="w-full py-6 sm:py-12">
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, ease: "easeOut" }}
              className="text-center max-w-4xl mx-auto px-4 sm:px-6"
            >
              {/* Hero section */}
              <div className="mb-6 sm:mb-8 lg:mb-12">
                <motion.div
                  initial={{ scale: 0.8, opacity: 0 }}
                  animate={{ scale: 1, opacity: 1 }}
                  transition={{ duration: 0.5, delay: 0.2 }}
                  className="w-16 h-16 sm:w-20 sm:h-20 mx-auto mb-3 sm:mb-4 lg:mb-6 bg-gradient-to-br from-blue-500 via-purple-600 to-indigo-700 rounded-2xl flex items-center justify-center shadow-2xl"
                >
                  <Sparkles className="w-8 h-8 sm:w-10 sm:h-10 text-white" />
                </motion.div>

                <motion.h1
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5, delay: 0.3 }}
                  className="text-xl sm:text-2xl lg:text-4xl font-bold bg-gradient-to-r from-gray-900 via-blue-800 to-purple-800 dark:from-white dark:via-blue-200 dark:to-purple-200 bg-clip-text text-transparent mb-2 sm:mb-4"
                >
                  欢迎使用 AI 助手
                </motion.h1>

                <motion.p
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5, delay: 0.4 }}
                  className="text-sm sm:text-base lg:text-lg text-gray-600 dark:text-gray-300 mb-4 sm:mb-6 lg:mb-8 leading-relaxed"
                >
                  我是您的智能助手，可以帮您解答问题、协助思考和提供建议。
                  <br className="hidden sm:block" />
                  <span className="sm:hidden"> </span>让我们开始一场有趣的对话吧！
                </motion.p>
              </div>

              {/* Capabilities grid */}
              <motion.div
                initial={{ opacity: 0, y: 30 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.6, delay: 0.5 }}
                className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 sm:gap-4 mb-4 sm:mb-6 lg:mb-8 auto-rows-max"
              >
                {suggestions.map((suggestion, index) => {
                  const Icon = suggestion.icon;
                  return (
                    <motion.div
                      key={index}
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.4, delay: 0.6 + index * 0.1 }}
                      whileHover={{
                        scale: 1.05,
                        boxShadow: "0 10px 25px rgba(0,0,0,0.1)",
                        transition: { duration: 0.2 }
                      }}
                      className="group p-4 sm:p-6 bg-white dark:bg-gray-800 rounded-2xl border border-gray-200 dark:border-gray-700 shadow-sm hover:shadow-lg cursor-pointer transition-all duration-300 touch-manipulation active:scale-95"
                      onClick={() => onSendMessage?.(suggestion.description)}
                    >
                      <div className={`w-10 h-10 sm:w-12 sm:h-12 rounded-xl bg-gradient-to-br ${suggestion.color} flex items-center justify-center mb-3 sm:mb-4 group-hover:scale-110 transition-transform duration-200`}>
                        <Icon className="w-5 h-5 sm:w-6 sm:h-6 text-white" />
                      </div>
                      <h3 className="font-semibold text-gray-900 dark:text-gray-100 mb-1 sm:mb-2 group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors text-sm sm:text-base">
                        {suggestion.title}
                      </h3>
                      <p className="text-xs sm:text-sm text-gray-600 dark:text-gray-400 leading-relaxed">
                        {suggestion.description}
                      </p>
                    </motion.div>
                  );
                })}
              </motion.div>

              {/* Quick start tips */}
              <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ duration: 0.5, delay: 1 }}
                className="flex flex-wrap justify-center gap-2 sm:gap-3 text-xs sm:text-sm text-gray-500 dark:text-gray-400 pb-4"
              >
                <div className="flex items-center gap-1.5 sm:gap-2 px-3 sm:px-4 py-1.5 sm:py-2 bg-white dark:bg-gray-800 rounded-full border border-gray-200 dark:border-gray-700">
                  <Zap className="w-3 h-3 sm:w-4 sm:h-4 text-yellow-500" />
                  <span>快速响应</span>
                </div>
                <div className="flex items-center gap-1.5 sm:gap-2 px-3 sm:px-4 py-1.5 sm:py-2 bg-white dark:bg-gray-800 rounded-full border border-gray-200 dark:border-gray-700">
                  <Globe className="w-3 h-3 sm:w-4 sm:h-4 text-blue-500" />
                  <span>多语言支持</span>
                </div>
                <div className="flex items-center gap-1.5 sm:gap-2 px-3 sm:px-4 py-1.5 sm:py-2 bg-white dark:bg-gray-800 rounded-full border border-gray-200 dark:border-gray-700">
                  <FileText className="w-3 h-3 sm:w-4 sm:h-4 text-green-500" />
                  <span>丰富内容</span>
                </div>
              </motion.div>
            </motion.div>
          </div>
        ) : (
          <div className="w-full">
            {messages.map((message, index) => (
              <motion.div
                key={message.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3, delay: index * 0.1 }}
              >
                <ChatMessage
                  role={message.role}
                  content={message.content}
                  timestamp={message.timestamp}
                />
              </motion.div>
            ))}
            {isLoading && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3 }}
              >
                <ChatMessage
                  role="assistant"
                  content=""
                  timestamp={new Date().toLocaleTimeString()}
                  isLoading={true}
                />
              </motion.div>
            )}
          </div>
        )}
      </ScrollArea>
    </div>
  );
}