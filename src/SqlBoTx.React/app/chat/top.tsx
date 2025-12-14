
import React from "react";
import { Link, Tooltip } from "@heroui/react";
import {
    Drawer,
    DrawerContent,
    DrawerHeader,
    DrawerBody,
    DrawerFooter,
    Button,
    useDisclosure,
} from "@heroui/react";
import ChatHistory from "./history";
import { HistoryQuery } from "@icon-park/react";


// 头部工具栏组件
const IconSidebar = () => <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect width="18" height="18" x="3" y="3" rx="2" ry="2" /><path d="M9 3v18" /></svg>;
const IconShare = () => <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M4 12v8a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-8" /><polyline points="16 6 12 2 8 6" /><line x1="12" x2="12" y1="2" y2="15" /></svg>;
const IconLayout = () => <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect width="18" height="18" x="3" y="3" rx="2" ry="2" /><path d="M3 9h18" /><path d="M9 21V9" /></svg>;
const IconHistory = () => <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8" /><path d="M3 3v5h5" /><path d="M12 7v5l4 2" /></svg>;
const HeaderToolbar = () => {

    const { isOpen, onOpen, onOpenChange } = useDisclosure();
    const [placement, setPlacement] = React.useState<string>("left");

    const handleOpen = (placement: any) => {
        setPlacement(placement);
        onOpen();
    };

    return (
        <div>
            <header className="w-full h-11 bg-white border-b border-gray-200 px-4 flex items-center justify-between">

                {/* 1. 左侧区域 (Left Section) */}
                {/* 使用 flex 和 gap 保持内部元素间距 */}
                <div className="flex items-center gap-3">

                    {/* 侧边栏按钮 */}
                    <Tooltip color="foreground" content='显示/隐藏对话' placement="bottom">
                        <Button isIconOnly onPress={() => handleOpen(placement)} className=" bg-white  hover:bg-gray-100 rounded-md text-gray-500 transition-colors">
                            <HistoryQuery theme="outline" size="24" fill="#333" />
                        </Button>
                    </Tooltip>

                    {/* 头像 */}
                    {/* <div className="w-8 h-8 rounded-full bg-orange-100 flex items-center justify-center overflow-hidden border border-gray-100">
                        <span className="text-lg">🐵</span>
                    </div> */}

                    {/* 标题 */}
                    {/* <h1 className="font-semibold text-gray-800 text-sm whitespace-nowrap">
                        随便聊聊
                    </h1> */}
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

            {/* 历史对话 */}
            <Drawer isOpen={isOpen} placement='right' onOpenChange={onOpenChange} radius="none"
                classNames={{
                    base: "sm:data-[placement=right]:m-2 sm:data-[placement=left]:m-2  rounded-medium",
                }}
            >
                <DrawerContent>
                    {(onClose) => (
                        <>
                            <DrawerHeader className="absolute top-0 inset-x-0 z-50 flex flex-row gap-2 px-2 py-2 border-b border-default-200/50 justify-between bg-content1/50 backdrop-saturate-150 backdrop-blur-lg">

                                <Tooltip color="foreground" content="Close">
                                    <Button
                                        isIconOnly
                                        className="text-default-400"
                                        size="sm"
                                        variant="light"
                                        onPress={onClose}
                                    >
                                        <svg
                                            fill="none"
                                            height="20"
                                            stroke="currentColor"
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                            strokeWidth="2"
                                            viewBox="0 0 24 24"
                                            width="20"
                                            xmlns="http://www.w3.org/2000/svg"
                                        >
                                            <path d="m13 17 5-5-5-5M6 17l5-5-5-5" />
                                        </svg>
                                    </Button>
                                </Tooltip>

                                <div className="w-full flex justify-start gap-2">
                                    历史会话
                                </div>

                                {/* 
                                <div className="w-full flex justify-start gap-2">
                                    <Button
                                        className="font-medium text-small text-default-500"
                                        size="sm"
                                        startContent={
                                            <svg
                                                height="16"
                                                viewBox="0 0 16 16"
                                                width="16"
                                                xmlns="http://www.w3.org/2000/svg"
                                            >
                                                <path
                                                    d="M3.85.75c-.908 0-1.702.328-2.265.933-.558.599-.835 1.41-.835 2.29V7.88c0 .801.23 1.548.697 2.129.472.587 1.15.96 1.951 1.06a.75.75 0 1 0 .185-1.489c-.435-.054-.752-.243-.967-.51-.219-.273-.366-.673-.366-1.19V3.973c0-.568.176-.993.433-1.268.25-.27.632-.455 1.167-.455h4.146c.479 0 .828.146 1.071.359.246.215.43.54.497.979a.75.75 0 0 0 1.483-.23c-.115-.739-.447-1.4-.99-1.877C9.51 1 8.796.75 7.996.75zM7.9 4.828c-.908 0-1.702.326-2.265.93-.558.6-.835 1.41-.835 2.29v3.905c0 .879.275 1.69.833 2.289.563.605 1.357.931 2.267.931h4.144c.91 0 1.705-.326 2.268-.931.558-.599.833-1.41.833-2.289V8.048c0-.879-.275-1.69-.833-2.289-.563-.605-1.357-.931-2.267-.931zm-1.6 3.22c0-.568.176-.992.432-1.266.25-.27.632-.454 1.168-.454h4.145c.54 0 .92.185 1.17.453.255.274.43.698.43 1.267v3.905c0 .569-.175.993-.43 1.267-.25.268-.631.453-1.17.453H7.898c-.54 0-.92-.185-1.17-.453-.255-.274-.43-.698-.43-1.267z"
                                                    fill="currentColor"
                                                    fillRule="evenodd"
                                                />
                                            </svg>
                                        }
                                        variant="flat"
                                    >
                                        Copy Link
                                    </Button>
                                    <Button
                                        className="font-medium text-small text-default-500"
                                        endContent={
                                            <svg
                                                fill="none"
                                                height="16"
                                                stroke="currentColor"
                                                strokeLinecap="round"
                                                strokeLinejoin="round"
                                                strokeWidth="2"
                                                viewBox="0 0 24 24"
                                                width="16"
                                                xmlns="http://www.w3.org/2000/svg"
                                            >
                                                <path d="M7 17 17 7M7 7h10v10" />
                                            </svg>
                                        }
                                        size="sm"
                                        variant="flat"
                                    >
                                        Event Page
                                    </Button>
                                </div> */}
                            </DrawerHeader>

                            <DrawerBody className="pt-16">
                                <ChatHistory />
                            </DrawerBody>

                            {/* <DrawerFooter className="flex flex-col gap-1">
                                <Link className="text-default-400" href="mailto:hello@heroui.com" size="sm">
                                    Contact the host
                                </Link>
                                <Link className="text-default-400" href="mailto:hello@heroui.com" size="sm">
                                    Report event
                                </Link>
                            </DrawerFooter> */}
                        </>
                    )}
                </DrawerContent>
            </Drawer>
        </div>

    );
};

export default HeaderToolbar;