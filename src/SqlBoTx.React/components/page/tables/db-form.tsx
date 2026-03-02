import React, { useState, useMemo, useEffect, useRef, useCallback } from 'react';
import { Button, ButtonGroup, Select, SelectItem, Tooltip, useDisclosure } from "@heroui/react";
import { Modal, ModalContent, ModalHeader, ModalBody, ModalFooter } from "@heroui/react";
import { addToast, ToastProvider } from "@heroui/react";
import { Form, Input } from "@heroui/react";
import { AddThree, Data, Delete, Edit, Server, TipsOne } from '@icon-park/react';
import { apiService } from '@/lib/api';
import TableForm from './table-form';

const api = {
    getDatabases: () => apiService.get('/DatabaseSetting/list'),
    deleteDb: (id: any) => apiService.delete(`/DatabaseSetting/delete/${id}`),
    addDb: (data: any) => apiService.post('/DatabaseSetting/add', data),
    updateDb: (data: any) => apiService.post('/DatabaseSetting/update', data)
};

export default function Index({ setSelectedTable, selectedTableKeys }: { setSelectedTable?: any, selectedTableKeys?: Array<string> }) {

    const refreshDatabases = async () => {
        const res = await api.getDatabases();
        setDatabases(res.data);
        if (res.data.length <= 0) {
            setSelectedDbId(null);
        }
        if (selectedDbId == null && res.data.length > 0) {
            setSelectedDbId(res.data[0].connectionId);
        }
    };

    React.useEffect(() => {
        refreshDatabases()
    }, []);

    const [databases, setDatabases] = useState<any[]>([]);
    const [selectedDbId, setSelectedDbId] = useState(null);
    const currentDb = databases.find(db => db.connectionId === selectedDbId);
    const [editingDb, setEditingDb] = useState<any>(null);
    const [dbToDelete, setDbToDelete] = useState<any>(null);
    const { isOpen: isDbModalOpen, onOpen: onDbModalOpen, onClose: onDbModalClose } = useDisclosure();
    const { isOpen: isDbDelModalOpen, onOpen: onDbDelModalOpen, onClose: onDbDelModalClose } = useDisclosure();
    const formDbRef = useRef<HTMLFormElement>(null);

    const submitformDb = async () => {
        const form = formDbRef.current!;
        if (form.checkValidity() == false) {
            form.reportValidity()
            return
        }
        const dataJson: any = Object.fromEntries(
            new FormData(form)
        );
        dataJson.connectionType = Number(dataJson.connectionType);

        if (!!editingDb) {
            dataJson.connectionId = editingDb.connectionId;
            await editDBApi(dataJson);
        } else {
            await addDBApi(dataJson);
        }
    }
    const submitDelDb = async () => {
        await deleteDBApi(dbToDelete.connectionId);
    }
    const addDBApi = async (dataJson: any) => {
        const response = await apiService.post('/DatabaseSetting/add', dataJson);
        if (response.code !== 200) {
            addToast({
                title: "提示",
                description: response.msg,
                radius: 'md',
                color: 'danger',
            });
        } else {
            addToast({
                title: "提示",
                description: '保存成功',
                radius: 'md',
                color: 'success',
            });
            onDbModalClose();
            await refreshDatabases();
        }
    }
    const editDBApi = async (dataJson: any) => {
        const response = await apiService.post('/DatabaseSetting/update', dataJson);
        if (response.code !== 200) {
            addToast({
                title: "提示",
                description: response.msg,
                radius: 'md',
                color: 'danger',
            });
        } else {
            addToast({
                title: "提示",
                description: '保存成功',
                radius: 'md',
                color: 'success',
            });
            onDbModalClose();
            await refreshDatabases();
        }
    }
    const deleteDBApi = async (connectionId: any) => {
        const response = await apiService.delete('/DatabaseSetting/delete/' + connectionId, null);
        if (response.code !== 200) {
            addToast({
                title: "提示",
                description: response.msg,
                radius: 'md',
                color: 'danger',
            });
        } else {
            addToast({
                title: "提示",
                description: response.msg,
                radius: 'md',
                color: 'success',
            });
            onDbDelModalClose();
            await refreshDatabases();
        }
    }
    const restformDb = async () => {
        formDbRef.current!.reset();
    }
    const openAddDbModal = (db: any) => {
        setEditingDb(null);
        onDbModalOpen();
    }
    const openEditDbModal = (db: any) => {
        setEditingDb(db);
        onDbModalOpen();
    }
    const openDelDbModal = (db: any) => {
        setDbToDelete(db);
        onDbDelModalOpen();
    }

    return (
        <div className="flex h-screen bg-gray-50 font-sans overflow-hidden">
            {/* --- 左侧边栏：数据库管理 --- */}
            <aside className="w-80 bg-white border-r border-gray-200 flex flex-col z-10 shadow-sm">
                {/* 侧边栏标题 */}
                <div className="p-5 border-b border-gray-100 flex items-center gap-3">
                    <span className="font-bold text-gray-800 tracking-tight">数据库结构管理</span>
                </div>

                {/* 数据库列表 */}
                <div className="flex-1 overflow-y-auto p-4 space-y-2">
                    <div className="flex items-center justify-between px-2 mb-2">
                        <span className="text-xs font-semibold text-gray-400 uppercase">Databases</span>
                        <Tooltip color="foreground" content='新增数据库连接' placement="bottom">
                            <Button isIconOnly onPress={openAddDbModal} className=" bg-[#F5F6F8] rounded-md text-gray-500 transition-colors">
                                <AddThree theme="outline" size="24" fill="#333" />
                            </Button>
                        </Tooltip>
                    </div>

                    {databases.map(db => (
                        <div
                            key={db.connectionId}
                            onClick={() => setSelectedDbId(db.connectionId)}
                            className={`group w-full flex items-center gap-2 px-3 py-3 rounded-xl cursor-pointer transition-all duration-200 ${selectedDbId === db.id
                                ? 'bg-indigo-50 text-indigo-700 shadow-sm ring-1 ring-indigo-200'
                                : 'text-gray-600 hover:bg-gray-50'
                                }`}
                        >
                            <Data theme="outline" size="24" fill={selectedDbId === db.connectionId ? '#432DD7' : '#333'} />
                            <div className="flex-1 min-w-0 flex flex-col">
                                <div className="font-medium truncate text-sm" title={db.connectionName}>{db.connectionName}</div>
                                <div className="text-[10px] opacity-70 flex items-center gap-1">
                                    {db.connectionType}
                                </div>
                            </div>

                            {/* 编辑/删除按钮 (固定显示) */}
                            <div className="flex gap-1 pl-1">
                                <Tooltip color="foreground" content='编辑' placement="bottom">
                                    <Button isIconOnly onPress={() => openEditDbModal(db)} className="w-[16px] p-0 bg-transparent  rounded-md text-gray-500 transition-colors">
                                        <Edit theme="outline" size="16" fill={selectedDbId === db.connectionId ? '#432DD7' : '#333'} />
                                    </Button>
                                </Tooltip>

                                <Tooltip color="foreground" content='删除' placement="bottom">
                                    <Button isIconOnly onPress={() => openDelDbModal(db)} className="bg-transparent  rounded-md text-gray-500 transition-colors">
                                        <Delete theme="outline" size="16" fill={selectedDbId === db.connectionId ? '#432DD7' : '#333'} />
                                    </Button>
                                </Tooltip>
                            </div>
                        </div>
                    ))}

                    {databases.length === 0 && (
                        <div className="text-center py-8 text-gray-400 text-sm">
                            暂无数据库<br />点击上方 + 号创建
                        </div>
                    )}
                </div>
            </aside>

            {/* --- 右侧主区域 --- */}
            <main className="flex-1 flex flex-col min-w-0 bg-white/50 relative">
                {/* 顶部 Header */}
                <header className="h-16 px-8 border-b border-gray-200 bg-white flex items-center justify-between sticky top-0 z-10">
                    <div className="flex items-center gap-3">
                        <div className="flex items-center gap-2 text-gray-500 text-sm">
                            <Data theme="outline" size="16" fill='#333' />
                            <span>/</span>
                            <span className="font-medium text-gray-900">{currentDb?.connectionName || '未选择数据库'}</span>
                        </div>
                    </div>
                    <div className="flex items-center gap-3">
                        <span className="px-3 py-1 bg-gray-100 text-gray-600 rounded-full text-xs font-semibold">v2.1.0</span>
                    </div>
                </header>

                {/* 内容区域 */}
                <div className="flex-1 overflow-y-auto p-8">
                    <div className="max-w-6xl mx-auto space-y-6">
                        {/* 数据表格卡片 */}
                        {!!selectedDbId ? (
                            <TableForm connectionId={selectedDbId} setSelectedTable={setSelectedTable} selectedTableKeys={selectedTableKeys} />
                        ) : (
                            <div className="flex flex-col items-center justify-center h-64 bg-gray-50 border-2 border-dashed border-gray-200 rounded-xl text-gray-400">
                                <Server theme="outline" size="48" fill="#D1D5DC" className="mb-4" />
                                <p>请先在左侧选择或创建一个数据库</p>
                            </div>
                        )}

                    </div>
                </div>
            </main>


            {/* 删除数据库确认 Modal */}
            <Modal isOpen={isDbDelModalOpen} onClose={onDbDelModalClose}>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader><TipsOne theme="outline" size="24" fill="#FB2C36" /></ModalHeader>
                            <ModalBody>
                                <p className="text-gray-500 text-sm">
                                    您确定要删除 <strong>{dbToDelete?.connectionName}</strong> 吗？<br />
                                    <span className="text-red-500 font-medium">警告：该操作将同时删除其下的所有数据表结构信息！</span>
                                </p>
                            </ModalBody>
                            <ModalFooter>
                                <Button variant="light" onPress={onClose}>
                                    <p className='text-gray-600'>取消</p>
                                </Button>
                                <Button color="danger" onPress={submitDelDb}>
                                    删除
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>

            {/*  新增、编辑数据库 Modal */}
            <Modal isOpen={isDbModalOpen} onClose={onDbModalClose}>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex flex-col gap-1">  {editingDb ? '编辑数据库' : '新建数据库'}</ModalHeader>
                            <ModalBody>
                                <Form
                                    ref={formDbRef}
                                    className="w-full  flex flex-col gap-4"
                                    onReset={() => console.log('rest')}
                                    onSubmit={(e) => {
                                        e.preventDefault();
                                        let data = Object.fromEntries(new FormData(e.currentTarget));
                                        console.log(data)
                                    }}
                                >
                                    <Input
                                        isRequired
                                        label="连接名称"
                                        labelPlacement="outside"
                                        name="connectionName"
                                        placeholder="输入连接名称"
                                        type="text"
                                        defaultValue={editingDb ? editingDb.connectionName : ''}
                                    />

                                    <Select
                                        defaultSelectedKeys={[editingDb ? editingDb.connectionType.toString() : '1']}
                                        isRequired
                                        label="连接类型"
                                        labelPlacement="outside"
                                        name="connectionType"
                                        placeholder="选择连接类型"
                                    >
                                        <SelectItem key="1">SQL Server</SelectItem>
                                    </Select>

                                    <Input
                                        isRequired
                                        label="连接"
                                        labelPlacement="outside"
                                        name="connectionString"
                                        placeholder="输入连接"
                                        type="text"
                                        defaultValue={editingDb ? editingDb.connectionString : ''}
                                    />

                                    <Input
                                        isRequired
                                        label="用户名"
                                        labelPlacement="outside"
                                        name="userName"
                                        placeholder="输入用户名"
                                        type="text"
                                        defaultValue={editingDb ? editingDb.userName : ''}
                                    />

                                    <Input
                                        isRequired
                                        label="密码"
                                        labelPlacement="outside"
                                        name="userPassword"
                                        placeholder="输入密码"
                                        type="password"
                                        defaultValue={editingDb ? editingDb.userPassword : ''}
                                    />
                                </Form>
                            </ModalBody>
                            <ModalFooter>
                                <Button color="danger" variant="light" onPress={onClose}>
                                    Close
                                </Button>
                                <Button color="danger" variant="light" onPress={restformDb}>
                                    重置
                                </Button>
                                <Button color="primary" onPress={submitformDb}>
                                    提交
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>


        </div>
    );
};