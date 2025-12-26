import { NavLink, useLocation } from "react-router";
import { useState } from "react";
import { useLoaderData } from "react-router";
import { Tooltip, Button } from "@heroui/react";
import { Tabs, Tab, Card, CardBody, Switch } from "@heroui/react";
import { Communication, Github, SeoFolder, TableFile } from "@icon-park/react";

import { Bot } from "@/components/icons";

//服务端加载数据
export async function loader(): Promise<any[]> {
  return [

  ]
}

export default function Home() {

  // 示例项目数据
  const projects: any[] = useLoaderData<typeof loader>();

  const { pathname } = useLocation();
  const [isVertical, setIsVertical] = useState(true);

  console.log("当前路径:", pathname);

  return (
    <div className="flex h-screen w-full bg-white text-slate-800 font-sans overflow-hidden">

      {/* ==================== 1. 最左侧导航栏 (Sidebar) ==================== */}
      <aside className=" flex flex-col items-center justify-between w-16   pr-1 pl-1  border-r border-gray-100 bg-gray-50/50 flex-shrink-0 z-20">

        {/* Top Section */}
        <div className="flex flex-col items-center  gap-2">

          <Bot width={48} height={48} />

          <div className={`p-2 hover:bg-[#EBEBEB] rounded-md transition-all duration-200 ease-in-out
          ${pathname === '/' ? 'bg-[#E9E9E9] text-[black]' : ''}`}
            key='/chat'
          >
            <Tooltip color="foreground" content='会话' placement='right'>
              <Communication theme="outline" size="24" fill="#333" />
            </Tooltip>
          </div>

          <div className={`p-2 hover:bg-[#EBEBEB] rounded-md transition-all duration-200 ease-in-out
          ${pathname === '/table' ? 'bg-[#E9E9E9] text-[black]' : ''}`}
            key='/table'
          >
            <Tooltip color="foreground" content='数据表' placement='right'>
              <TableFile theme="outline" size="24" fill="#333" />
            </Tooltip>
          </div>


          <div className={`p-2 hover:bg-[#EBEBEB] rounded-md transition-all duration-200 ease-in-out
          ${pathname === '/db' ? 'bg-[#E9E9E9] text-[black]' : ''}`}
            key='/db'
          >
            <Tooltip color="foreground" content='知识库' placement='right'>
              <SeoFolder theme="outline" size="24" fill="#333" />
            </Tooltip>
          </div>

        </div>

        {/* Bottom Section */}
        <div className="flex flex-col items-center gap-4 pb-2">
          <button className="p-2 text-gray-400 hover:text-gray-600 transition-colors">
            <Github theme="outline" size="24" fill="#333" />
          </button>

          <div className="w-8 h-8 bg-gray-200 rounded-full cursor-pointer hover:ring-2 ring-gray-300 transition-all">
            <img src="https://api.dicebear.com/7.x/avataaars/svg?seed=Felix" alt="User" className="w-full h-full rounded-full" />
          </div>
        </div>
      </aside>


      {/* ==================== 2. 右侧主体 ==================== */}
      <div className="flex w-full border-amber-500 border ">
        <HeaderToolbar />
      </div>



    </div>
  );
};


// 头部工具栏组件
const IconSidebar = () => <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect width="18" height="18" x="3" y="3" rx="2" ry="2" /><path d="M9 3v18" /></svg>;
const IconShare = () => <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M4 12v8a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-8" /><polyline points="16 6 12 2 8 6" /><line x1="12" x2="12" y1="2" y2="15" /></svg>;
const IconLayout = () => <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect width="18" height="18" x="3" y="3" rx="2" ry="2" /><path d="M3 9h18" /><path d="M9 21V9" /></svg>;
const IconHistory = () => <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8" /><path d="M3 3v5h5" /><path d="M12 7v5l4 2" /></svg>;
const HeaderToolbar = () => {
  return (
    // 最外层容器：Flex布局 + 两端对齐 + 底部边框 + 垂直居中
    <header className="w-full h-14 bg-white border-b border-gray-200 px-4 flex items-center justify-between">

      {/* 1. 左侧区域 (Left Section) */}
      {/* 使用 flex 和 gap 保持内部元素间距 */}
      <div className="flex items-center gap-3">
        {/* 侧边栏按钮 */}
        <button className="p-1.5 hover:bg-gray-100 rounded-md text-gray-500 transition-colors">
          <IconSidebar />
        </button>

        {/* 头像 */}
        <div className="w-8 h-8 rounded-full bg-orange-100 flex items-center justify-center overflow-hidden border border-gray-100">
          {/* 这里用 emoji 模拟您的头像 */}
          <span className="text-lg">🐵</span>
        </div>

        {/* 标题 */}
        <h1 className="font-semibold text-gray-800 text-sm whitespace-nowrap">
          随便聊聊
        </h1>
      </div>

      {/* 2. 中间区域 (Center Section) */}
      {/* 如果希望无论左右宽度如何，中间都在正中央，可以加 flex-1 justify-center */}
      <div className="flex items-center gap-2">
        {/* 胶囊标签 1 */}
        <div className="flex items-center gap-1.5 bg-gray-100 px-3 py-1 rounded-md text-xs font-medium text-gray-600 cursor-pointer hover:bg-gray-200 transition">
          <span>⚙️</span>
          <span>gpt-5-mini</span>
        </div>

        {/* 胶囊标签 2 */}
        <div className="flex items-center gap-1.5 bg-gray-100 px-3 py-1 rounded-md text-xs font-medium text-gray-600 cursor-pointer hover:bg-gray-200 transition">
          <IconHistory />
          <span>20</span>
        </div>
      </div>

      {/* 3. 右侧区域 (Right Section) */}
      <div className="flex items-center justify-end gap-1">
        <button className="p-2 hover:bg-gray-100 rounded-md text-gray-500 transition-colors">
          <IconLayout />
        </button>
        <button className="p-2 hover:bg-gray-100 rounded-md text-gray-500 transition-colors">
          <IconShare />
        </button>
        <button className="p-2 hover:bg-gray-100 rounded-md text-gray-500 transition-colors">
          <IconSidebar /> {/* 右侧那个类似的图标 */}
        </button>
      </div>

    </header>
  );
};