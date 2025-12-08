import React, { useState, useRef, useEffect } from 'react';



import { Send, Mic, Plus, Smile, MoreVertical, Search, Star, Database, BarChart, Table as TableIcon, FileText } from 'lucide-react';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

import { apiService } from 'lib/api';
import type { BlockMessage, ContentItem, TableBlock, DeltaMessage, SqlBlock, SqlContentItem, SSEMessage, TableContentItem, EchartsBlock } from '@/types/messages';


// --- 主页面组件 ---
export default function SqlBotChat() {

  return (
    <div className="min-h-screen bg-gray-50">

      {/* Container: use a 10-column grid on medium+ screens */}
      <div className="max-w-7xl mx-auto">
        <div className="grid grid-cols-1 md:grid-cols-10 gap-4">

          <aside className="hidden md:block md:col-span-2 bg-white rounded-lg shadow p-4">
            <div className="text-sm font-semibold text-gray-700 mb-3">侧边栏</div>
            <nav className="space-y-2 text-sm">
              <a className="block px-3 py-2 rounded hover:bg-gray-100">概览</a>
              <a className="block px-3 py-2 rounded hover:bg-gray-100">我的项目</a>
              <a className="block px-3 py-2 rounded hover:bg-gray-100">报表</a>
              <a className="block px-3 py-2 rounded hover:bg-gray-100">设置</a>
            </nav>
          </aside>
 
          <main className="col-span-1 md:col-span-8 bg-white rounded-lg shadow p-6">
            <header className="flex items-center justify-between mb-6">
              <h1 className="text-xl font-bold text-gray-800">页面主内容</h1>
              <div className="text-sm text-gray-500">账号 • 退出</div>
            </header>


            <section className="space-y-4">
              <div className="p-4 border rounded-md">主要卡片 1：这是主区域，手机端占满屏幕宽度。</div>
              <div className="p-4 border rounded-md">主要卡片 2：可放图表、表格或表单。</div>
              <div className="p-4 border rounded-md">主要卡片 3：更多内容...</div>
            </section>


          </main>


        </div>
      </div>
    </div>
  );
}