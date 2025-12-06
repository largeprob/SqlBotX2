import { motion, AnimatePresence } from "motion/react";
import { X, Check, AlertCircle, Info } from "lucide-react";
import { useState, useEffect } from "react";

export interface ToastProps {
  id: string;
  type?: "success" | "error" | "info" | "warning";
  title?: string;
  message: string;
  duration?: number;
  onClose: (id: string) => void;
}

const iconMap = {
  success: Check,
  error: AlertCircle,
  info: Info,
  warning: AlertCircle,
};

const colorMap = {
  success: "text-green-600 dark:text-green-400",
  error: "text-red-600 dark:text-red-400",
  info: "text-blue-600 dark:text-blue-400",
  warning: "text-yellow-600 dark:text-yellow-400",
};

const backgroundMap = {
  success: "bg-green-50 dark:bg-green-950 border-green-200 dark:border-green-800",
  error: "bg-red-50 dark:bg-red-950 border-red-200 dark:border-red-800",
  info: "bg-blue-50 dark:bg-blue-950 border-blue-200 dark:border-blue-800",
  warning: "bg-yellow-50 dark:bg-yellow-950 border-yellow-200 dark:border-yellow-800",
};

export function Toast({ id, type = "info", title, message, duration = 3000, onClose }: ToastProps) {
  const Icon = iconMap[type];

  useEffect(() => {
    if (duration > 0) {
      const timer = setTimeout(() => {
        onClose(id);
      }, duration);
      return () => clearTimeout(timer);
    }
  }, [id, duration, onClose]);

  return (
    <motion.div
      layout
      initial={{ opacity: 0, y: 50, scale: 0.3 }}
      animate={{ opacity: 1, y: 0, scale: 1 }}
      exit={{ opacity: 0, y: 20, scale: 0.9 }}
      className={`
        relative flex items-start gap-3 p-4 rounded-xl border shadow-lg backdrop-blur-sm
        ${backgroundMap[type]}
      `}
    >
      <Icon className={`w-5 h-5 mt-0.5 ${colorMap[type]}`} />
      
      <div className="flex-1 min-w-0">
        {title && (
          <h4 className="font-medium text-sm text-gray-900 dark:text-gray-100 mb-1">
            {title}
          </h4>
        )}
        <p className="text-sm text-gray-700 dark:text-gray-300">
          {message}
        </p>
      </div>
      
      <button
        onClick={() => onClose(id)}
        className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
      >
        <X className="w-4 h-4" />
      </button>
    </motion.div>
  );
}

// Toast Container Component
export function ToastContainer({ toasts }: { toasts: ToastProps[] }) {
  return (
    <div className="fixed top-4 right-4 z-50 space-y-2 max-w-sm">
      <AnimatePresence mode="popLayout">
        {toasts.map((toast) => (
          <Toast key={toast.id} {...toast} />
        ))}
      </AnimatePresence>
    </div>
  );
}