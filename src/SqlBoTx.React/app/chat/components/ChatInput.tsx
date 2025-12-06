import { useState, useRef, useEffect } from "react";
import { Button } from "./ui/button";
import { Send, Paperclip, Mic, Square } from "lucide-react";
import { motion } from "motion/react";

interface ChatInputProps {
  onSendMessage: (message: string) => void;
  isLoading?: boolean;
  placeholder?: string;
}

export function ChatInput({ onSendMessage, isLoading, placeholder = "输入您的问题..." }: ChatInputProps) {
  const [message, setMessage] = useState("");
  const [isFocused, setIsFocused] = useState(false);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (message.trim() && !isLoading) {
      onSendMessage(message.trim());
      setMessage("");
      // Reset textarea height
      if (textareaRef.current) {
        textareaRef.current.style.height = 'auto';
      }
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSubmit(e);
    }
  };

  // Auto-resize textarea
  useEffect(() => {
    if (textareaRef.current) {
      textareaRef.current.style.height = 'auto';
      textareaRef.current.style.height = `${Math.min(textareaRef.current.scrollHeight, 160)}px`;
    }
  }, [message]);

  return (
    <div className="border-t border-gray-200 dark:border-gray-700 bg-white/80 dark:bg-gray-900/80 backdrop-blur-sm pb-safe">
      <div className="max-w-4xl mx-auto px-3 sm:px-4 py-3 sm:py-6">
        <motion.form
          onSubmit={handleSubmit}
          className="relative"
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.3 }}
        >
          <div className={`
            relative flex items-end gap-2 sm:gap-3 p-3 sm:p-4 rounded-2xl border-2 transition-all duration-200 ease-in-out
            ${isFocused
              ? "border-blue-500 dark:border-blue-400 shadow-lg shadow-blue-500/20 dark:shadow-blue-400/20"
              : "border-gray-200 dark:border-gray-600 shadow-sm hover:shadow-md"
            }
            bg-white dark:bg-gray-800
          `}>
            {/* Attachment Button */}
            <Button
              type="button"
              variant="ghost"
              size="sm"
              className="shrink-0 self-end mb-1 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 rounded-xl transition-colors p-2 touch-manipulation"
            >
              <Paperclip className="w-4 h-4 sm:w-5 sm:h-5" />
            </Button>

            {/* Message Input */}
            <div className="flex-1 relative">
              <textarea
                ref={textareaRef}
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                onKeyDown={handleKeyDown}
                onFocus={() => setIsFocused(true)}
                onBlur={() => setIsFocused(false)}
                placeholder={placeholder}
                disabled={isLoading}
                className="w-full resize-none border-0 bg-transparent text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:ring-0 text-base leading-relaxed min-h-[24px] max-h-[120px] sm:max-h-[160px] py-1 touch-manipulation"
                style={{ height: 'auto' }}
                rows={1}
                autoCapitalize="sentences"
                autoCorrect="on"
                spellCheck="true"
              />
            </div>

            {/* Voice/Send Button */}
            <div className="shrink-0 self-end mb-1">
              {message.trim() ? (
                <Button
                  type="submit"
                  size="sm"
                  disabled={isLoading}
                  className={`
                    rounded-xl px-3 py-2 min-h-[44px] transition-all duration-200 touch-manipulation
                    ${isLoading
                      ? "bg-gray-400 dark:bg-gray-600"
                      : "bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 shadow-lg hover:shadow-xl active:scale-95"
                    }
                    text-white border-0
                  `}
                >
                  {isLoading ? (
                    <Square className="w-4 h-4" />
                  ) : (
                    <Send className="w-4 h-4" />
                  )}
                </Button>
              ) : (
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  className="rounded-xl text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 transition-colors p-2 min-h-[44px] touch-manipulation"
                >
                  <Mic className="w-4 h-4 sm:w-5 sm:h-5" />
                </Button>
              )}
            </div>
          </div>

          {/* Helper Text */}
          <div className="text-xs text-gray-500 dark:text-gray-400 text-center mt-2 sm:mt-3 transition-opacity duration-200 hidden sm:block">
            <span className={isFocused ? "opacity-100" : "opacity-70"}>
              按 Enter 发送消息，Shift + Enter 换行
            </span>
          </div>
        </motion.form>
      </div>
    </div>
  );
}