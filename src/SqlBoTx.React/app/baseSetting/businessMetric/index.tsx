import { AddThree, Delete, DropDownList, Edit, Find, InsertTable, MoreOne, PreviewOpen, TipsOne, Data, LinkTwo, Add } from "@icon-park/react";
import { Modal, ModalContent, ModalHeader, ModalBody, ModalFooter, Tooltip, Form, Textarea, useDisclosure, Select, SelectItem, Input, Alert } from "@heroui/react";
import { addToast, ToastProvider } from "@heroui/react";
import React from "react";

import {
    Table,
    TableHeader,
    TableColumn,
    TableBody,
    TableRow,
    TableCell,
    Button,
    DropdownTrigger,
    Dropdown,
    DropdownMenu,
    DropdownItem,
    Chip,
    Pagination,
    type Selection,
    type SortDescriptor
} from "@heroui/react";
import { apiService } from "@/lib/api";
import { useLoaderData } from "react-router";
import DbTableForm from '@/components/page/tables/db-form';

// 业务指标状态枚举
const BusinessMetricStatus = {
    AI: 1,
    VERIFIED: 2,
};

const StatusMap = {
    [BusinessMetricStatus.AI]: { label: "AI生成", color: "primary" },
    [BusinessMetricStatus.VERIFIED]: { label: "已核验", color: "success" },
};

export const columns = [
    { name: "ID", uid: "id", sortable: true },
    { name: "指标名称", uid: "metricName", sortable: true },
    { name: "指标编码", uid: "metricCode", sortable: true },
    { name: "近义词", uid: "alias", sortable: true },
    { name: "状态", uid: "status", sortable: true },
    { name: "主表", uid: "mainTable", sortable: true },
    { name: "连接路径", uid: "joinPaths" },
    { name: "计算公式", uid: "expression" },
    { name: "创建时间", uid: "createdDate", sortable: true },
    { name: "更新时间", uid: "updatedDate", sortable: true },
    { name: "ACTIONS", uid: "actions" },
];

const INITIAL_VISIBLE_COLUMNS = ["metricName", "metricCode", "status", "mainTable", "joinPaths", "actions"];

const api = {
    getList: () => apiService.get('/BusinessMetric/list'),
    delete: (id: any) => apiService.delete(`/BusinessMetric/delete/${id}`),
    add: (data: any) => apiService.post('/BusinessMetric/add', data),
    update: (data: any) => apiService.post('/BusinessMetric/update', data),
};

//服务端加载数据
export async function loader(): Promise<any[]> {
    const response = await api.getList();
    return response.data
}

export default function Index() {
    const initialList: any[] = useLoaderData<typeof loader>();
    console.log("初始列表数据:", initialList);

    const [list, setList] = React.useState<any[]>(initialList || []);

    const { isOpen: isShowJoinPathsOpen, onOpen: onShowJoinPathsOpen, onClose: onShowJoinPathsClose } = useDisclosure();
    const [showJoinPathList, setShowJoinPathList] = React.useState<any[]>([]);

    const [fromData, setFormData] = React.useState<any>(null);
    const { isOpen: isSelectTableModalOpenOfMain, onOpen: onSelectTableModalOpenOfMain, onClose: onSelectTableModalCloseOfMain } = useDisclosure();
    const { isOpen: isModalOpenOfJoinPath, onOpen: onModalOpenOfJoinPath, onClose: onModalCloseOfJoinPath } = useDisclosure();
    const { isOpen: isModalOpen, onOpen: onModalOpen, onClose: onModalClose } = useDisclosure();
    const { isOpen: isModalDeleteOpen, onOpen: onModalDeleteOpen, onClose: onModalDeleteClose } = useDisclosure();

    const [filterValue, setFilterValue] = React.useState("");
    const [selectedKeys, setSelectedKeys] = React.useState<Selection>(new Set([]));
    const [visibleColumns, setVisibleColumns] = React.useState<Selection>(new Set(INITIAL_VISIBLE_COLUMNS));
    const [statusFilter, setStatusFilter] = React.useState("all");
    const [rowsPerPage, setRowsPerPage] = React.useState(5);
    const [sortDescriptor, setSortDescriptor] = React.useState<SortDescriptor>({
        column: "id",
        direction: "ascending",
    });
    const [page, setPage] = React.useState(1);

    const hasSearchFilter = Boolean(filterValue);

    //隐藏显示列
    const headerColumns = React.useMemo(() => {
        if (visibleColumns === "all") return columns;
        return columns.filter((column) => Array.from(visibleColumns).includes(column.uid));
    }, [visibleColumns]);


    const filteredItems = React.useMemo(() => {
        let filteredUsers = [...list];

        if (hasSearchFilter) {
            filteredUsers = filteredUsers.filter((user) =>
                user.metricName?.toLowerCase().includes(filterValue.toLowerCase()) ||
                user.metricCode?.toLowerCase().includes(filterValue.toLowerCase()),
            );
        }

        return filteredUsers;
    }, [list, filterValue, statusFilter]);

    const pages = Math.ceil(filteredItems.length / rowsPerPage) || 1;

    const items = React.useMemo(() => {
        const start = (page - 1) * rowsPerPage;
        const end = start + rowsPerPage;

        return filteredItems.slice(start, end);
    }, [page, filteredItems, rowsPerPage]);

    const sortedItems = React.useMemo(() => {
        return [...items].sort((a, b) => {
            const first = a[sortDescriptor.column];
            const second = b[sortDescriptor.column];
            const cmp = first < second ? -1 : first > second ? 1 : 0;

            return sortDescriptor.direction === "descending" ? -cmp : cmp;
        });
    }, [sortDescriptor, items]);

    const renderCell = React.useCallback((item: any, columnKey: any) => {
        const cellValue = item[columnKey];

        switch (columnKey) {

            case "status":
                const statusInfo = StatusMap[cellValue] || { label: "未知", color: "default" };
                return (
                    <Chip color={statusInfo.color as any} variant="flat" size="sm">
                        {statusInfo.label}
                    </Chip>
                );

            case "mainTable":
                return (
                    <div className="flex items-center gap-2">
                        <Data theme="outline" size="16" fill="#333" />
                        <span className="text-sm">{item.mainTable?.tableName || item.mainTableId}</span>
                    </div>
                );

            case "joinPaths":
                return (
                    <Tooltip color="foreground" content='查看' placement="bottom">
                        <Button isIconOnly onPress={() => showJoinPaths(item)} className=" bg-white  hover:bg-gray-100 rounded-md text-gray-500 transition-colors">
                            <LinkTwo theme="outline" size="16" fill="#333" />
                        </Button>
                    </Tooltip>
                );

            case "expression":
                return (
                    <Tooltip color="foreground" content={cellValue} placement="bottom">
                        <span className="text-sm text-gray-600 max-w-[150px] truncate block">
                            {cellValue}
                        </span>
                    </Tooltip>
                );

            case "actions":
                return (
                    <Dropdown>
                        <DropdownTrigger>
                            <Button isIconOnly size="sm" variant="light">
                                <MoreOne theme="outline" size="24" fill="#333" />
                            </Button>
                        </DropdownTrigger>
                        <DropdownMenu>
                            <DropdownItem key="view">View</DropdownItem>
                            <DropdownItem key="edit" onPress={() => openModal('edit', item)} >Edit</DropdownItem>
                            <DropdownItem key="delete" onPress={() => openModal('delete', item)} >Delete</DropdownItem>
                        </DropdownMenu>
                    </Dropdown>
                );
            default:
                return cellValue;
        }
    }, []);

    const showJoinPaths = React.useCallback((data: any) => {
        setShowJoinPathList(data.joinPaths || []);
        onShowJoinPathsOpen();
    }, [])

    const onNextPage = React.useCallback(() => {
        if (page < pages) {
            setPage(page + 1);
        }
    }, [page, pages]);

    const onPreviousPage = React.useCallback(() => {
        if (page > 1) {
            setPage(page - 1);
        }
    }, [page]);

    const onRowsPerPageChange = React.useCallback((e: any) => {
        setRowsPerPage(Number(e.target.value));
        setPage(1);
    }, []);

    const onSearchChange = React.useCallback((value: any) => {
        if (value) {
            setFilterValue(value);
            setPage(1);
        } else {
            setFilterValue("");
        }
    }, []);

    const onClear = React.useCallback(() => {
        setFilterValue("");
        setPage(1);
    }, []);

    const topContent = React.useMemo(() => {
        return (
            <div className="flex flex-col gap-4">
                <Alert color='primary' title='解释'
                    description='业务指标是企业经营的‘逻辑结晶’与‘知识资产’。它是脱离了物理表结构的纯业务口径（如‘复购率’、‘人效’），封装了复杂的计算规则与去歧义逻辑。指标是连接自然语言与底层数据的‘语义桥梁’，也是AI能够像专家一样思考的核心依据。'
                />
                <div className="flex justify-between gap-3 items-end">
                    <Input
                        isClearable
                        className="w-full sm:max-w-[44%]"
                        placeholder="指标名称/编码搜索"
                        startContent={<Find theme="outline" size="16" fill="#333" />}
                        value={filterValue}
                        onClear={() => onClear()}
                        onValueChange={onSearchChange}
                    />
                    <div className="flex gap-3">
                        <Dropdown>
                            <DropdownTrigger className="hidden sm:flex">
                                <Button endContent={<DropDownList theme="outline" size="16" fill="#333" />} variant="flat">
                                    Columns
                                </Button>
                            </DropdownTrigger>
                            <DropdownMenu
                                disallowEmptySelection
                                aria-label="Table Columns"
                                closeOnSelect={false}
                                selectedKeys={visibleColumns}
                                selectionMode="multiple"
                                onSelectionChange={setVisibleColumns}
                            >
                                {columns.map((column) => (
                                    <DropdownItem key={column.uid} className="capitalize">
                                        {column.name}
                                    </DropdownItem>
                                ))}
                            </DropdownMenu>
                        </Dropdown>
                        <Button onPress={() => openModal('add')} color="secondary" startContent={<AddThree theme="outline" size="16" fill="#FFFFFF" />}>
                            添加业务指标
                        </Button>
                    </div>
                </div>
                <div className="flex justify-between items-center">
                    <span className="text-default-400 text-small">总数 {list.length}</span>
                    <label className="flex items-center text-default-400 text-small">
                        分页数:
                        <select
                            className="bg-transparent outline-solid outline-transparent text-default-400 text-small"
                            onChange={onRowsPerPageChange}
                        >
                            <option value="5">5</option>
                            <option value="10">10</option>
                            <option value="15">15</option>
                        </select>
                    </label>
                </div>
            </div>
        );
    }, [
        filterValue,
        statusFilter,
        visibleColumns,
        onRowsPerPageChange,
        list.length,
        onSearchChange,
        hasSearchFilter,
    ]);

    const bottomContent = React.useMemo(() => {
        return (
            <div className="py-2 px-2 flex justify-between items-center">
                <span className="w-[30%] text-small text-default-400">
                    {selectedKeys === "all"
                        ? "All items selected"
                        : `${selectedKeys instanceof Set ? selectedKeys.size : 0} of ${filteredItems.length} selected`}
                </span>
                <Pagination
                    isCompact
                    showControls
                    showShadow
                    color="primary"
                    page={page}
                    total={pages}
                    onChange={setPage}
                />
                <div className="hidden sm:flex w-[30%] justify-end gap-2">
                    <Button isDisabled={pages === 1} size="sm" variant="flat" onPress={onPreviousPage}>
                        Previous
                    </Button>
                    <Button isDisabled={pages === 1} size="sm" variant="flat" onPress={onNextPage}>
                        Next
                    </Button>
                </div>
            </div>
        );
    }, [selectedKeys, items.length, page, pages, hasSearchFilter]);


    //表单
    const formRef = React.useRef<HTMLFormElement>(null);

    const refreshList = React.useCallback(async () => {
        const response = await api.getList();
        setList(response.data)
    }, [])


    //提交表单
    const submitform = async () => {
        const form = formRef.current!;

        if (form.checkValidity() == false) {
            form.reportValidity()
            return
        }

        const dataJson: any = Object.fromEntries(
            new FormData(form)
        );

        // 处理状态
        dataJson.status = Number(dataJson.status);
        dataJson.mainTableId = selectedMainTable[0].tableId;
        dataJson.businessObjectiveId = Number(dataJson.businessObjectiveId);

        // 处理近义词 - 将数组转换为逗号分隔的字符串
        if (fromData && fromData.aliasArr && fromData.aliasArr.length > 0) {
            dataJson.alias = fromData.aliasArr.filter((item: string) => item.trim() !== '').join(',');
        } else {
            dataJson.alias = '';
        }

        // 处理连接路径
        if (fromData && fromData.joinPathsArr && fromData.joinPathsArr.length > 0) {
            dataJson.joinPaths = fromData.joinPathsArr.map((item: any) => ({
                tableId: item.tableId,
                alias: item.alias,
                joinType: item.joinType,
                onCondition: item.onCondition,
            }));
        }

        console.log("提交数据:", dataJson);

        if (!!fromData && fromData.id) {
            dataJson.id = fromData.id
            const response = await api.update(dataJson);
            if (response.code !== 200) {
                addToast({
                    title: "提示",
                    description: response.msg,
                    radius: 'md',
                    color: 'danger',
                });
            } else {
                await refreshList();
                addToast({
                    title: "提示",
                    description: '保存成功',
                    radius: 'md',
                    color: 'success',
                });
                onModalClose();
            }
        } else {
            const response = await api.add(dataJson);
            if (response.code !== 200) {
                addToast({
                    title: "提示",
                    description: response.msg,
                    radius: 'md',
                    color: 'danger',
                });
            } else {
                await refreshList();
                addToast({
                    title: "提示",
                    description: '保存成功',
                    radius: 'md',
                    color: 'success',
                });
                onModalClose();
            }
        }
    }

    const submitDelete = async () => {
        const response = await api.delete(fromData.id);
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
                description: '删除成功',
                radius: 'md',
                color: 'success',
            });
            await refreshList()
            onModalDeleteClose();
        }
    }

    // modal
    const openModal = React.useCallback((type: 'add' | 'edit' | 'delete', data?: any) => {
        if (type == 'add') {
            setSelectedMainTable(null);
            setFormData({
                aliasArr: [],
                joinPathsArr: [],
            });
            onModalOpen();
        }

        if (type == 'edit') {
            setSelectedMainTable(data.mainTable || null);
            setFormData(
                {
                    ...data,
                    aliasArr: data.alias ? data.alias.split(',').filter((item: string) => item.trim() !== '') : [],
                    joinPathsArr: data.joinPaths ? data.joinPaths.map((item: any) => ({
                        tableId: item.tableId,
                        alias: item.alias,
                        joinType: item.joinType,
                        onCondition: item.onCondition,
                    })) : [],
                }
            )
            onModalOpen();
        }

        if (type == 'delete') {
            setFormData({
                id: data.id,
            });
            onModalDeleteOpen();
        }
    }, [])


    // 近义词
    const addAlias = () => {
        setFormData((prev: any) => ({
            ...prev,
            aliasArr: [...(prev.aliasArr || []), '']
        }));
    }

    const updateAlias = (index: number, value: any) => {
        setFormData((prev: any) => ({
            ...prev,
            aliasArr: prev.aliasArr.map((item: any, i: number) => i === index ? value : item)
        }));
    }

    const removeAlias = (index: number) => {
        setFormData((prev: any) => ({
            ...prev,
            aliasArr: prev.aliasArr.filter((_: any, i: number) => i !== index)
        }));
    }

    // 连接路径
    const addJoinPath = () => {
        setFormData((prev: any) => ({
            ...prev,
            joinPathsArr: [...(prev.joinPathsArr || []), {
                tableId: null,
                alias: '',
                joinType: 'LEFT JOIN',
                onCondition: '',
            }]
        }));
    }

    const updateJoinPath = (index: number, field: string, value: any) => {
        setFormData((prev: any) => ({
            ...prev,
            joinPathsArr: prev.joinPathsArr.map((item: any, i: number) =>
                i === index ? { ...item, [field]: value } : item
            )
        }));
    }

    const removeJoinPath = (index: number) => {
        setFormData((prev: any) => ({
            ...prev,
            joinPathsArr: prev.joinPathsArr.filter((_: any, i: number) => i !== index)
        }));
    }


    // 选择表-主表
    const [selectedMainTable, setSelectedMainTable] = React.useState<any[]>([]);
    const confirmMainTable = React.useCallback(() => {
        if (selectedMainTable.length > 1) {
            addToast({
                title: "提示",
                description: '仅能选择一行数据',
                radius: 'md',
                color: 'danger',
            })
            return
        }
        setFormData((prev: any) => ({
            ...prev,
            mainTableArr: selectedMainTable,
            mainTableId: selectedMainTable[0].tableId
        }));
        onSelectTableModalCloseOfMain()
    }, [selectedMainTable])

    // 选择表-依赖路径
    const [selectedJoinPathTable, setSelectedJoinPathTable] = React.useState<any[]>([]);
    const [joinPathIndex, setJoinPathIndex] = React.useState<number>(-1);
    const openSelectJoinTable = (index: number) => {
        setJoinPathIndex(index);
        onModalOpenOfJoinPath();
    }
    const confirmJoinPath = React.useCallback(() => {
        console.log('确认选择', selectedJoinPathTable)
        if (selectedJoinPathTable.length > 1) {
            addToast({
                title: "提示",
                description: '仅能选择一行数据',
                radius: 'md',
                color: 'danger',
            })
            return
        }
        onModalCloseOfJoinPath();

        const table = selectedJoinPathTable[0];
        updateJoinPath(joinPathIndex, 'tableId', table.tableId);
        updateJoinPath(joinPathIndex, 'alias', table.tableName);
        setJoinPathIndex(-1);

    }, [selectedJoinPathTable])

    return (
        <main className="w-full">
            <header className="h-16 px-8 border-b border-gray-200 bg-white flex items-center justify-between sticky top-0 z-10">
                <div className="flex items-center gap-3">
                    <div>
                        <h1 className="text-2xl font-bold text-gray-900">业务指标管理</h1>
                        <p className="text-gray-500 text-sm mt-1">定义和管理业务指标</p>
                    </div>
                </div>
                <div className="flex items-center gap-3">
                    <span className="px-3 py-1 bg-gray-100 text-gray-600 rounded-full text-xs font-semibold">v2.1.0</span>
                </div>
            </header>

            {/* 内容区域 */}
            <div className="flex-1 overflow-y-auto p-8">

                <Table
                    isHeaderSticky
                    aria-label="Example table with custom cells, pagination and sorting"
                    bottomContent={bottomContent}
                    bottomContentPlacement="outside"
                    classNames={{
                        wrapper: "max-h-[382px]",
                    }}
                    sortDescriptor={sortDescriptor}
                    topContent={topContent}
                    topContentPlacement="outside"
                    onSelectionChange={setSelectedKeys}
                    onSortChange={setSortDescriptor}
                >
                    <TableHeader columns={headerColumns}>
                        {(column) => (
                            <TableColumn
                                key={column.uid}
                                align={column.uid === "actions" ? "center" : "start"}
                                allowsSorting={column.sortable}
                            >
                                {column.name}
                            </TableColumn>
                        )}
                    </TableHeader>
                    <TableBody emptyContent={"No list found"} items={sortedItems}>
                        {(item) => (
                            <TableRow key={item.id}>
                                {(columnKey) => <TableCell>{renderCell(item, columnKey)}</TableCell>}
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </div>

            {/*  新增、编辑表 Modal */}
            <Modal isOpen={isModalOpen} onClose={onModalClose} size='4xl' scrollBehavior='inside'>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex flex-col gap-1">  {!!fromData && fromData.id ? '编辑 ' : '新建 '}</ModalHeader>
                            <ModalBody>
                                <Form
                                    ref={formRef}
                                    className="w-full  flex flex-col gap-4"
                                >
                                    <div className="grid grid-cols-4 gap-2 w-full">

                                        <Input
                                            isRequired
                                            label="指标名称"
                                            labelPlacement="outside"
                                            name="metricName"
                                            placeholder="输入指标名称"
                                            type="text"
                                            defaultValue={fromData ? fromData.metricName : ''}
                                        />

                                        <Input
                                            isRequired
                                            label="指标编码"
                                            labelPlacement="outside"
                                            name="metricCode"
                                            placeholder="输入指标编码"
                                            type="text"
                                            defaultValue={fromData ? fromData.metricCode : ''}
                                        />

                                        <Select
                                            isRequired
                                            label="生命周期状态"
                                            labelPlacement="outside"
                                            name="status"
                                            placeholder="选择生命周期状态"
                                            defaultSelectedKeys={[fromData ? fromData.status?.toString() : BusinessMetricStatus.AI?.toString()]}
                                        >
                                            <SelectItem key={BusinessMetricStatus.AI}>AI生成</SelectItem>
                                            <SelectItem key={BusinessMetricStatus.VERIFIED}>已核验</SelectItem>
                                        </Select>

                                        <Input
                                            isRequired
                                            label="归属业务模块ID"
                                            labelPlacement="outside"
                                            name="businessObjectiveId"
                                            placeholder="输入业务模块ID"
                                            type="number"
                                            defaultValue={fromData ? fromData.businessObjectiveId : ''}
                                        />
                                    </div>

                                    {/* 近义词 */}
                                    <div className="w-full">
                                        {/* 字段列表标题 */}
                                        <div className="flex justify-between items-end mb-1">
                                            <label className="text-[14px]">近义词</label>
                                        </div>
                                        <div className="space-y-3 w-full">
                                            {(fromData?.aliasArr || []).map((item: any, index: number) => (
                                                <div
                                                    key={index}
                                                    className="group relative flex flex-col md:flex-row gap-3 items-start md:items-center p-3 bg-gray-50/50 border border-gray-200 rounded-xl hover:shadow-sm hover:border-indigo-300 hover:bg-white transition-all duration-200"
                                                >
                                                    <div className="hidden md:block w-8 text-center text-gray-400 font-mono text-xs select-none">
                                                        {index + 1}
                                                    </div>
                                                    <div className="flex-1 w-full space-y-1">
                                                        <input
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                            placeholder="近义词"
                                                            value={item}
                                                            onChange={(e) => updateAlias(index, e.target.value)}
                                                        />
                                                    </div>

                                                    <div className="flex items-center gap-2 justify-end md:justify-center w-full md:w-auto mt-2 md:mt-0 pl-2 border-l border-gray-100">
                                                        <Tooltip color='danger' content='删除' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => removeAlias(index)}
                                                                className="w-8 h-8 flex items-center justify-center rounded-lg text-gray-300 hover:bg-red-50 hover:text-red-500 transition-colors"
                                                                title="删除"
                                                            >
                                                                <Delete size={16} />
                                                            </button>
                                                        </Tooltip>
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                        <button
                                            type='button'
                                            onClick={addAlias}
                                            className="w-full py-3 border border-dashed border-gray-300 rounded-xl text-gray-500 hover:border-indigo-500 hover:text-indigo-600 hover:bg-indigo-50/50 transition-all flex items-center justify-center gap-2 text-sm font-medium mt-4"
                                        >
                                            <AddThree theme="outline" size="16" fill="#333" /> 添加
                                        </button>
                                    </div>

                                    {/* 主表 */}
                                    <div className="w-full">
                                        <div className="flex justify-between items-end mb-1">
                                            <label className="text-[14px]">依赖主体-主表
                                                <span className="text-danger text-small pl-1">*</span>
                                            </label>
                                        </div>
                                        <div className="w-full">

                                            <div className="space-y-3 w-full">
                                                {(fromData?.mainTableArr || []).map((item: any, index: number) => (
                                                    <div
                                                        key={index}
                                                        className="group relative flex flex-col md:flex-row gap-3 items-start md:items-center p-3 bg-gray-50/50 border border-gray-200 rounded-xl hover:shadow-sm hover:border-indigo-300 hover:bg-white transition-all duration-200"
                                                    >
                                                        <div className="w-8 text-center text-gray-400 font-mono text-xs select-none pr-3">
                                                            <InsertTable theme="outline" size="16" fill="#333" />
                                                        </div>

                                                        <div className="flex-1 w-full space-y-1">
                                                            <label className="text-xs text-gray-400">表名</label>
                                                            <input
                                                                readOnly
                                                                className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                                placeholder="表名"
                                                                value={item.tableName}
                                                            />
                                                        </div>

                                                        <div className="flex-1 w-full space-y-1">
                                                            <label className="text-xs text-gray-400">显示名称</label>
                                                            <input
                                                                readOnly
                                                                className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                                placeholder="显示名称"
                                                                value={item.displayName}
                                                            />
                                                        </div>
                                                    </div>
                                                ))}
                                            </div>

                                            <button
                                                type='button'
                                                onClick={onSelectTableModalOpenOfMain}
                                                className="w-full py-3 border border-dashed border-gray-300 rounded-xl text-gray-500 hover:border-indigo-500 hover:text-indigo-600 hover:bg-indigo-50/50 transition-all flex items-center justify-center gap-2 text-sm font-medium mt-4"
                                            >
                                                <AddThree theme="outline" size="16" fill="#333" /> 选择表
                                            </button>
                                        </div>
                                    </div>

                                    <Input
                                        label="主表别名"
                                        labelPlacement="outside"
                                        name="mainAlias"
                                        placeholder="输入主表别名"
                                        type="text"
                                        defaultValue={fromData ? fromData.mainAlias : 'Main'}
                                    />
                                    <Textarea
                                        isRequired
                                        label="计算公式"
                                        labelPlacement="outside"
                                        name="expression"
                                        placeholder="输入计算公式"
                                        type="text"
                                        defaultValue={fromData ? fromData.expression : ''}
                                    />
                                    {/* 连接路径 */}
                                    <div className="w-full">
                                        <div className="flex justify-between items-end mb-1">
                                            <label className="text-[14px]">连接路径</label>
                                        </div>
                                        <div className="space-y-3 w-full">
                                            {(fromData?.joinPathsArr || []).map((item: any, index: number) => (
                                                <div
                                                    key={index}
                                                    className="group relative flex flex-col md:flex-row gap-3 items-start md:items-center p-3 bg-gray-50/50 border border-gray-200 rounded-xl hover:shadow-sm hover:border-indigo-300 hover:bg-white transition-all duration-200"
                                                >
                                                    <div className="w-8 text-center text-gray-400 font-mono text-xs select-none pr-3">
                                                        {index + 1}
                                                    </div>

                                                    <div className="flex-1 w-full space-y-1">
                                                        <label className="text-xs text-gray-400">目标表ID</label>
                                                        <input
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                            placeholder="表ID"
                                                            value={item.tableId || ''}
                                                            onChange={(e) => updateJoinPath(index, 'tableId', e.target.value)}
                                                        />
                                                    </div>

                                                    <div className="flex-1 w-full space-y-1">
                                                        <label className="text-xs text-gray-400">别名</label>
                                                        <input
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                            placeholder="别名"
                                                            value={item.alias || ''}
                                                            onChange={(e) => updateJoinPath(index, 'alias', e.target.value)}
                                                        />
                                                    </div>

                                                    <div className="flex-1 w-full space-y-1">
                                                        <label className="text-xs text-gray-400">连接类型</label>
                                                        <select
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700"
                                                            value={item.joinType || 'LEFT JOIN'}
                                                            onChange={(e) => updateJoinPath(index, 'joinType', e.target.value)}
                                                        >
                                                            <option value="LEFT JOIN">LEFT JOIN</option>
                                                            <option value="INNER JOIN">INNER JOIN</option>
                                                            <option value="RIGHT JOIN">RIGHT JOIN</option>
                                                        </select>
                                                    </div>

                                                    <div className="flex-1 w-full space-y-1">
                                                        <label className="text-xs text-gray-400">连接条件</label>
                                                        <input
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                            placeholder="如: Main.id = Order.opp_id"
                                                            value={item.onCondition || ''}
                                                            onChange={(e) => updateJoinPath(index, 'onCondition', e.target.value)}
                                                        />
                                                    </div>

                                                    <div className="flex items-center gap-2 justify-end md:justify-center w-full md:w-auto mt-2 md:mt-0 pl-2 border-l border-gray-100">
                                                        <Tooltip color='foreground' content='选择表' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => openSelectJoinTable(index)}
                                                                className="w-8 h-8 flex items-center justify-center rounded-lg text-gray-300 hover:bg-blue-50 hover:text-blue-500 transition-colors"
                                                                title="选择表"
                                                            >
                                                                <Find size={16} />
                                                            </button>
                                                        </Tooltip>
                                                        <Tooltip color='danger' content='删除' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => removeJoinPath(index)}
                                                                className="w-8 h-8 flex items-center justify-center rounded-lg text-gray-300 hover:bg-red-50 hover:text-red-500 transition-colors"
                                                                title="删除"
                                                            >
                                                                <Delete size={16} />
                                                            </button>
                                                        </Tooltip>
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                        <button
                                            type='button'
                                            onClick={addJoinPath}
                                            className="w-full py-3 border border-dashed border-gray-300 rounded-xl text-gray-500 hover:border-indigo-500 hover:text-indigo-600 hover:bg-indigo-50/50 transition-all flex items-center justify-center gap-2 text-sm font-medium mt-4"
                                        >
                                            <Add theme="outline" size="16" fill="#333" /> 添加连接路径
                                        </button>
                                    </div>
                                </Form>
                            </ModalBody>
                            <ModalFooter>
                                <Button color="danger" variant="light" onPress={onClose}>
                                    Close
                                </Button>
                                <Button color="danger" variant="light" >
                                    重置
                                </Button>
                                <Button color="primary" onPress={submitform}>
                                    提交
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>

            {/* 选择数据库表-依赖主体-主表 */}
            <Modal isOpen={isSelectTableModalOpenOfMain} onClose={onSelectTableModalCloseOfMain} size='6xl' scrollBehavior='inside'>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex flex-col gap-1">选择数据表</ModalHeader>
                            <ModalBody>
                                <DbTableForm setSelectedTable={setSelectedMainTable} selectedTableKeys={[]} />
                            </ModalBody>
                            <ModalFooter>
                                <Button color="primary" onPress={confirmMainTable} >
                                    确认选择
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>


            {/* 选择数据库表-依赖路径 */}
            <Modal isOpen={isModalOpenOfJoinPath} onClose={onModalCloseOfJoinPath} size='6xl' scrollBehavior='inside'>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex flex-col gap-1">选择数据表</ModalHeader>
                            <ModalBody>
                                <DbTableForm setSelectedTable={setSelectedJoinPathTable} selectedTableKeys={[]} />
                            </ModalBody>
                            <ModalFooter>
                                <Button color="primary" onPress={confirmJoinPath} >
                                    确认选择
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>

            {/* 查看连接路径 */}
            <Modal isOpen={isShowJoinPathsOpen} onClose={onShowJoinPathsClose} size='5xl' scrollBehavior='inside'>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex flex-col gap-1">查看连接路径</ModalHeader>
                            <ModalBody>
                                <div className="space-y-3 w-full">
                                    {showJoinPathList.length === 0 ? (
                                        <div className="text-center py-8 text-gray-400 text-sm">
                                            暂无连接路径
                                        </div>
                                    ) : (
                                        showJoinPathList.map((item: any, index: number) => (
                                            <div
                                                key={index}
                                                className="group relative flex flex-col md:flex-row gap-3 items-start md:items-center p-3 bg-gray-50/50 border border-gray-200 rounded-xl hover:shadow-sm hover:border-indigo-300 hover:bg-white transition-all duration-200"
                                            >
                                                <div className="w-8 text-center text-gray-400 font-mono text-xs select-none pr-3">
                                                    {index + 1}
                                                </div>

                                                <div className="flex-1 w-full space-y-1">
                                                    <label className="text-xs text-gray-400">表ID</label>
                                                    <input
                                                        readOnly
                                                        className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                        value={item.tableId}
                                                    />
                                                </div>

                                                <div className="flex-1 w-full space-y-1">
                                                    <label className="text-xs text-gray-400">别名</label>
                                                    <input
                                                        readOnly
                                                        className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                        value={item.alias}
                                                    />
                                                </div>

                                                <div className="flex-1 w-full space-y-1">
                                                    <label className="text-xs text-gray-400">连接类型</label>
                                                    <input
                                                        readOnly
                                                        className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                        value={item.joinType}
                                                    />
                                                </div>

                                                <div className="flex-1 w-full space-y-1">
                                                    <label className="text-xs text-gray-400">连接条件</label>
                                                    <input
                                                        readOnly
                                                        className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                        value={item.onCondition}
                                                    />
                                                </div>
                                            </div>
                                        ))
                                    )}
                                </div>
                            </ModalBody>
                            <ModalFooter>
                                <Button color="danger" variant="light" onPress={onClose}>
                                    Close
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>

            {/* 删除确认 Modal */}
            <Modal isOpen={isModalDeleteOpen} onClose={onModalDeleteClose}>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader><TipsOne theme="outline" size="24" fill="#FB2C36" /></ModalHeader>
                            <ModalBody>
                                <p className="text-gray-500 text-sm">
                                    您确定要删除吗？
                                </p>
                            </ModalBody>
                            <ModalFooter>
                                <Button variant="light" onPress={onClose}>
                                    <p className='text-gray-600'>取消</p>
                                </Button>
                                <Button color="danger" onPress={submitDelete}>
                                    删除
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>
        </main >
    );
};
