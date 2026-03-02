import React, { useState, useMemo, useEffect, useRef, useCallback } from 'react';
import { Outlet, useLocation, useNavigate } from "react-router";
import { Button, ButtonGroup, Select, SelectItem, Tooltip, useDisclosure } from "@heroui/react";
import { Add, AddMode, AddOne, AddThree, BatteryEmpty, Communication, Data, Delete, Edit, Find, InsertTable, Key, SeoFolder, Server, SettingConfig, TableFile, TipsOne } from '@icon-park/react';




export default function layout() {
    const { pathname } = useLocation();
    const navigate = useNavigate();

    const NavItem = ({ path, icon: Icon, tooltip }: { path: string; icon: any; tooltip: string }) => {
        const isActive = pathname === path;
        return (
            <div
                onClick={() => navigate(path)}
                // 修改点：添加了 flex flex-col items-center justify-center
                className={`p-2 cursor-pointer rounded-md transition-all duration-200 ease-in-out group text-center flex flex-col items-center justify-center
                ${isActive ? "bg-[#E9E9E9]" : "hover:bg-[#EBEBEB]"}`}
            >
                <Tooltip color="foreground" content={tooltip} placement="right">
                    {/* 选中时变黑，未选中灰色 */}
                    <Icon theme="outline" size="24" fill={isActive ? "#000" : "#333"} />
                </Tooltip>
                <p className="text-[10px]">{tooltip}</p>
            </div>
        );
    };

    return (
        <div className="flex h-screen bg-gray-50 font-sans overflow-hidden">

            {/* --- 左侧边栏：数据库管理 --- */}
            <aside className="w-20 bg-white border-r border-gray-200 flex flex-col z-10 shadow-sm">
                <div className="flex flex-col gap-2 mt-2">
                    <NavItem path="/baseSetting/businessObjective" icon={Communication} tooltip="业务目标管理" />
                    <NavItem path="/baseSetting/businessMetric" icon={Communication} tooltip="业务指标管理" />
                </div>
            </aside>

            {/* --- 右侧主区域 --- */}
            <main className="flex-1 flex flex-col min-w-0 bg-white/50 relative">
                <Outlet />
            </main>

        </div>
    );
}
