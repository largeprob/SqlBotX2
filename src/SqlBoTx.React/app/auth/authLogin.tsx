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
import { Bot } from "@/components/icons";
import { useState } from "react";
import { apiService } from "@/lib/api"

export function AuthLogin({ auth }: {
    auth: boolean
}) {
    const { isOpen, onOpen, onClose } = useDisclosure();

    const [userAccount, setUserAccount] = useState<string>("");
    const [userPwd, setUserPwd] = useState<string>("");
    async function Login() {
        console.log("Login", userAccount, userPwd);
        const res = await apiService.login(userAccount, userPwd);
        if (res.code == 200) {

        } else {
            addToast({
                title: "登陆失败",
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
                                <div>
                                    <div className="text-lg font-semibold">SQLBotx 演示地址</div>
                                </div>
                            </ModalHeader>

                            <ModalBody>
                                <div className="space-y-4">
                                    <div className="text-sm text-gray-700">
                                        <Input label="账号" type="text" value={userAccount} onValueChange={setUserAccount} />
                                    </div>
                                    <div className="text-sm text-gray-700">
                                        <Input label="密码" type="password" value={userPwd} onValueChange={setUserPwd} />
                                    </div>
                                    <div
                                        onClick={Login}
                                        className="inline-flex items-center justify-center gap-2 w-full px-4 py-2 rounded-lg bg-[#2E2E2E] text-white hover:brightness-90 transition  select-none" aria-label="Login with GitHub">
                                        <Bot />
                                        登录
                                    </div>
                                </div>
                            </ModalBody>
                        </>
                    )}
                </ModalContent>
            </Modal>
        </div>
    )
}