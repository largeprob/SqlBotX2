import { Input } from "@heroui/react";
import {
    Modal,
    ModalContent,
    ModalHeader,
    ModalBody,
    ModalFooter,
    Button,
    useDisclosure,
} from "@heroui/react";

import { addToast } from "@heroui/react";
import { Bot, Github } from "@/components/icons";
import { useState } from "react";
import { apiService, login } from "@/lib/api"

export function AuthLogin({ auth }: {
    auth: boolean
}) {

    const { isOpen, onOpen, onClose } = useDisclosure();

    const [userAccount, setUserAccount] = useState<string>("");
    const [userPwd, setUserPwd] = useState<string>("");

    const [logining, setLogining] = useState<boolean>(false);


    const Login = async () => {

        if (userAccount == "" || userPwd == "") {
            addToast({
                description: '账号或密码不能为空',
                color: "warning",
                variant: 'bordered',
            })
            return
        }
        setLogining(true);
        const res = await apiService.login(userAccount, userPwd);
        setLogining(false);
        if (res.code == 200) {
        } else {
            addToast({
                description: res.msg,
                color: "danger",
                variant: 'bordered',
            })
        }
    }

    return (
        <div>
            {/* 登录 */}
            <Modal backdrop="blur" isOpen={auth} onClose={onClose} className="w-[340px]" placement="center">
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex items-center gap-3">
                                <div className="w-10 h-10 flex items-center justify-center">
                                    <Bot />
                                </div>
                                <div>
                                    <div className="text-lg font-semibold">SQLBotx</div>
                                </div>
                            </ModalHeader>

                            <ModalBody>
                                <div className="space-y-4">
                                    <div className="text-xs text-gray-400">
                                        本程序仅供以演示学习为目的，任何商业化行为都需作者授权。
                                    </div>
                                    <div className="text-xs text-gray-400">
                                        账号获取请加QQ群：<span className="font-medium">123456789</span>
                                    </div>
                                </div>

                                <div className="space-y-4">

                                    <div className="text-sm text-gray-700">
                                        <Input label="账号" type="text" value={userAccount} onValueChange={setUserAccount} />
                                    </div>
                                    <div className="text-sm text-gray-700">
                                        <Input label="密码" type="password" value={userPwd} onValueChange={setUserPwd} />
                                    </div>

                                    <Button
                                        isLoading={logining}
                                        onPress={Login}
                                        className="inline-flex items-center justify-center gap-2 w-full px-4 py-2 rounded-lg bg-[#2E2E2E] text-white hover:brightness-90 transition  select-none" aria-label="Login with GitHub">
                                        登录
                                    </Button>
                                </div>
                            </ModalBody>
                        </>
                    )}
                </ModalContent>
            </Modal>
        </div>
    )
}