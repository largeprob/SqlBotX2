import { Outlet, useLocation, useNavigate } from "react-router"; // 确保导入 useNavigate 用于跳转
import { Tooltip } from "@heroui/react";
import { Communication, Github, SeoFolder, TableFile } from "@icon-park/react";
import { Bot } from "@/components/icons";

export default function MainLayout() {
    const { pathname } = useLocation();
    const navigate = useNavigate();

    // 封装一个简单的导航按钮组件，减少重复代码
    const NavItem = ({ path, icon: Icon, tooltip }: { path: string; icon: any; tooltip: string }) => {
        const isActive = pathname === path;
        return (
            <div
                onClick={() => navigate(path)}
                className={`p-2 cursor-pointer rounded-md transition-all duration-200 ease-in-out group
          ${isActive ? "bg-[#E9E9E9]" : "hover:bg-[#EBEBEB]"}`}
            >
                <Tooltip color="foreground" content={tooltip} placement="right">
                    {/* 选中时变黑，未选中灰色 */}
                    <Icon theme="outline" size="24" fill={isActive ? "#000" : "#333"} />
                </Tooltip>
            </div>
        );
    };

    return (
        <div className="flex h-screen w-full bg-white text-slate-800 font-sans overflow-hidden">

            {/* ==================== 1. 最左侧侧边栏 (Sidebar) ==================== */}
            <aside className="flex flex-col items-center justify-between w-16 pl-1 pr-1  border-r border-gray-100 bg-gray-50/50 flex-shrink-0 z-20">

                {/* Top Section */}
                <div className="flex flex-col items-center gap-4">
                    <Bot width={48} height={48} />

                    <div className="flex flex-col gap-2 mt-2">
                        <NavItem path="/" icon={Communication} tooltip="会话" />
                        <NavItem path="/table" icon={TableFile} tooltip="数据表" />
                        <NavItem path="/db" icon={SeoFolder} tooltip="知识库" />
                    </div>
                </div>

                {/* Bottom Section */}
                <div className="flex flex-col items-center gap-4">
                    <button className="p-2 text-gray-400 hover:text-gray-600 transition-colors">
                        <Github theme="outline" size="24" fill="#333" />
                    </button>

                    <div className="w-8 h-8 bg-gray-200 rounded-full cursor-pointer hover:ring-2 ring-gray-300 transition-all">
                        <img
                            src="https://api.dicebear.com/7.x/avataaars/svg?seed=Felix"
                            alt="User"
                            className="w-full h-full rounded-full"
                        />
                    </div>
                </div>
            </aside>

            {/* ==================== 2. 右侧主体区域 ==================== */}
            <main className="w-full">
                <Outlet />
            </main>
        </div>
    );
}

