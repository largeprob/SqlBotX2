
import { AddThree, Delete, DropDownList, Edit, Find, InsertTable, MoreOne, PreviewOpen, TipsOne } from "@icon-park/react";
import { Modal, ModalContent, ModalHeader, ModalBody, ModalFooter, Tooltip, Form, Textarea, useDisclosure } from "@heroui/react";
import { addToast, ToastProvider } from "@heroui/react";
import React from "react";
import { Alert } from "@heroui/react";
import {
    Table,
    TableHeader,
    TableColumn,
    TableBody,
    TableRow,
    TableCell,
    Input,
    Button,
    DropdownTrigger,
    Dropdown,
    DropdownMenu,
    DropdownItem,
    Chip,
    User,
    Pagination,
} from "@heroui/react";
import { apiService } from "@/lib/api";
import { useLoaderData } from "react-router";
import DbTableForm from '@/components/page/tables/db-form';


export const columns = [
    { name: "ID", uid: "id", sortable: true },
    { name: "业务名称", uid: "businessName", sortable: true },
    { name: "近义词", uid: "synonyms", sortable: true },
    { name: "依赖表", uid: "dependencyTables", sortable: true },
    { name: "业务解释", uid: "description" },
    { name: "创建时间", uid: "createdDate", sortable: true },
    { name: "更新时间", uid: "updatedDate", sortable: true },
    { name: "ACTIONS", uid: "actions" },
];

export const statusOptions = [
    { name: "Active", uid: "active" },
    { name: "Paused", uid: "paused" },
    { name: "Vacation", uid: "vacation" },
];


export function capitalize(s: any) {
    return s ? s.charAt(0).toUpperCase() + s.slice(1).toLowerCase() : "";
}


export const ChevronDownIcon = ({ strokeWidth = 1.5, ...otherProps }) => {
    return (
        <svg
            aria-hidden="true"
            fill="none"
            focusable="false"
            height="1em"
            role="presentation"
            viewBox="0 0 24 24"
            width="1em"
            {...otherProps}
        >
            <path
                d="m19.92 8.95-6.52 6.52c-.77.77-2.03.77-2.8 0L4.08 8.95"
                stroke="currentColor"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeMiterlimit={10}
                strokeWidth={strokeWidth}
            />
        </svg>
    );
};

const statusColorMap = {
    active: "success",
    paused: "danger",
    vacation: "warning",
};

const INITIAL_VISIBLE_COLUMNS = ["businessName", "synonyms", "dependencyTables", "description", "createdDate", "updatedDate", "actions"];


const api = {
    getList: () => apiService.get('/BusinessObjective/list'),
    delete: (id: any) => apiService.delete(`/BusinessObjective/delete/${id}`),
    add: (data: any) => apiService.post('/BusinessObjective/add', data),
    update: (data: any) => apiService.post('/BusinessObjective/update', data),
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

    const { isOpen: isShowDependencyTablesOpen, onOpen: onShowDependencyTablesOpen, onClose: onShowDependencyTablesClose } = useDisclosure();
    const [showDependencyTableList, setShowDependencyTableList] = React.useState<any[]>([]);

    const [fromData, setFormData] = React.useState<any>(null);
    const { isOpen: isSelectTableModalOpen, onOpen: onSelectTableModalOpen, onClose: onSelectTableModalClose } = useDisclosure();
    const { isOpen: isModalOpen, onOpen: onModalOpen, onClose: onModalClose } = useDisclosure();
    const { isOpen: isModalDeleteOpen, onOpen: onModalDeleteOpen, onClose: onModalDeleteClose } = useDisclosure();

    const [filterValue, setFilterValue] = React.useState("");
    const [selectedKeys, setSelectedKeys] = React.useState(new Set([]));
    const [visibleColumns, setVisibleColumns] = React.useState(new Set(INITIAL_VISIBLE_COLUMNS));
    const [statusFilter, setStatusFilter] = React.useState("all");
    const [rowsPerPage, setRowsPerPage] = React.useState(5);
    const [sortDescriptor, setSortDescriptor] = React.useState({
        column: "age",
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
                user.name.toLowerCase().includes(filterValue.toLowerCase()),
            );
        }
        if (statusFilter !== "all" && Array.from(statusFilter).length !== statusOptions.length) {
            filteredUsers = filteredUsers.filter((user) =>
                Array.from(statusFilter).includes(user.status),
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

            case "dependencyTables":
                return (
                    <Tooltip color="foreground" content='查看' placement="bottom">
                        <Button isIconOnly onPress={() => showDependencyTables(item)} className=" bg-white  hover:bg-gray-100 rounded-md text-gray-500 transition-colors">
                            <PreviewOpen theme="outline" size="16" fill="#333" />
                        </Button>
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

    const showDependencyTables = React.useCallback((data: any) => {
        setShowDependencyTableList(data.dependencyTables);
        onShowDependencyTablesOpen();
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
                    description='业务目标旨在统一团队对系统业务范畴的认知，确保所有人聚焦于解决实际业务问题，而非单纯关注数据本身。换言之，每一个业务目标都对应一个核心业务模块，并指向你日常负责的具体工作领域。业务模块是企业数据的‘物理疆域’与‘上下文边界’。它将散落在数据库中的成百上千张物理表，按照业务职能（如销售、供应链、财务）进行逻辑划界。'
                />
                <div className="flex justify-between gap-3 items-end">
                    <Input
                        isClearable
                        className="w-full sm:max-w-[44%]"
                        placeholder="业务目标搜索"
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
                                        {capitalize(column.name)}
                                    </DropdownItem>
                                ))}
                            </DropdownMenu>
                        </Dropdown>
                        <Button onPress={() => openModal('add')} color="secondary" startContent={<AddThree theme="outline" size="16" fill="#FFFFFF" />}>
                            添加业务目标
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
                        : `${selectedKeys.size} of ${filteredItems.length} selected`}
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
    const [dependencyTableErrors, setDependencyTableErrors] = React.useState<any>(null);

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

        if (selectedTable.length <= 0) {
            setDependencyTableErrors('依赖表不能为空')
            return
        }

        const dataJson: any = Object.fromEntries(
            new FormData(form)
        );
        dataJson.synonyms = fromData.synonymsArr?.join(',');
        dataJson.DependencyTables = selectedTable;
        console.log(dataJson)

        console.log('表单',fromData)
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
            setFormData({
                synonymsArr: [],
            });
            onModalOpen();
        }

        if (type == 'edit') {
            setFormData(
                {
                    ...data,
                    synonymsArr: data.synonyms?.split(',')
                }
            )
            setSelectedKeys(data.dependencyTables.map((x: any) => x.tableId.toString()));
            setSelectedTable(data.dependencyTables);
            setFormData({
                ...data,
                synonymsArr: data.synonyms?.split(','),
            });
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
    const addSynonyms = () => {
        setFormData((prev: any) => ({
            ...prev,
            synonymsArr: [...prev.synonymsArr, '']
        }));
    }

    const updateSynonyms = (index: number, value: any) => {
        setFormData((prev: any) => ({
            ...prev,
            synonymsArr: prev.synonymsArr.map((item: any, i: number) => i === index ? value : item)
        }));
    }
    const removeSynonyms = (index: number) => {
        setFormData((prev: any) => ({
            ...prev,
            synonymsArr: prev.synonymsArr.filter((_: any, i: number) => i !== index)
        }));
    }

    // 依赖表
    const [selectedTable, setSelectedTable] = React.useState<any[]>([]);
    const selectedTableKeys = React.useMemo(() => selectedTable.map(t => t.tableId.toString()), [selectedTable]);
    const addDependencyTable = async () => {
        onSelectTableModalOpen();
    }
    const removeDependencyTable = React.useCallback((index: number) => {
        setSelectedTable(selectedTable.filter((x, i) => i !== index))
    }, [])



    return (
        <main className="w-full">
            <header className="h-16 px-8 border-b border-gray-200 bg-white flex items-center justify-between sticky top-0 z-10">
                <div className="flex items-center gap-3">
                    <div>
                        <h1 className="text-2xl font-bold text-gray-900">业务目标管理</h1>
                        <p className="text-gray-500 text-sm mt-1">分类项目业务目标</p>

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
                                    <Input
                                        isRequired
                                        label="业务名称"
                                        labelPlacement="outside"
                                        name="businessName"
                                        placeholder="输入业务名称"
                                        type="text"
                                        defaultValue={fromData ? fromData.businessName : ''}
                                    />

                                    <Textarea
                                        label="业务解释"
                                        labelPlacement="outside"
                                        name="description"
                                        placeholder="输入业务解释"
                                        type="text"
                                        defaultValue={fromData ? fromData.description : ''}
                                    />


                                    {/* 依赖表 */}
                                    <div className="w-full">
                                        <div className="flex justify-between items-end mb-1">
                                            <label className="text-[14px]">依赖表
                                                <span className="text-danger text-small pl-1">
                                                    *
                                                </span>
                                            </label>
                                        </div>
                                        <div className="space-y-3 w-full">
                                            {selectedTable.map((item: any, index: number) => (
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

                                                    <div className="flex items-center gap-2 justify-end md:justify-center w-full md:w-auto mt-2 md:mt-0 pl-2 border-l border-gray-100">
                                                        <Tooltip color='danger' content='删除' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => removeDependencyTable(index)}
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
                                            onClick={addDependencyTable}
                                            className="w-full py-3 border border-dashed border-gray-300 rounded-xl text-gray-500 hover:border-indigo-500 hover:text-indigo-600 hover:bg-indigo-50/50 transition-all flex items-center justify-center gap-2 text-sm font-medium mt-4"
                                        >
                                            <AddThree theme="outline" size="16" fill="#333" /> 添加
                                        </button>
                                        {
                                            !!dependencyTableErrors &&
                                            <span className="text-danger text-small pl-2">
                                                {dependencyTableErrors}
                                            </span>
                                        }
                                    </div>

                                    {/* 近义词 */}
                                    <div className="w-full">
                                        {/* 字段列表标题 */}
                                        <div className="flex justify-between items-end mb-1">
                                            <label className="text-[14px]">近义词</label>
                                        </div>
                                        <div className="space-y-3 w-full">
                                            {fromData.synonymsArr?.map((item: any, index: number) => (
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
                                                            onChange={(e) => updateSynonyms(index, e.target.value)}
                                                        />
                                                    </div>

                                                    <div className="flex items-center gap-2 justify-end md:justify-center w-full md:w-auto mt-2 md:mt-0 pl-2 border-l border-gray-100">
                                                        <Tooltip color='danger' content='删除' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => removeSynonyms(index)}
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
                                            onClick={addSynonyms}
                                            className="w-full py-3 border border-dashed border-gray-300 rounded-xl text-gray-500 hover:border-indigo-500 hover:text-indigo-600 hover:bg-indigo-50/50 transition-all flex items-center justify-center gap-2 text-sm font-medium mt-4"
                                        >
                                            <AddThree theme="outline" size="16" fill="#333" /> 添加
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

            {/* 选择数据库表 */}
            <Modal isOpen={isSelectTableModalOpen} onClose={onSelectTableModalClose} size='6xl' scrollBehavior='inside'>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex flex-col gap-1">选择依赖表</ModalHeader>
                            <ModalBody>
                                <DbTableForm setSelectedTable={setSelectedTable} selectedTableKeys={selectedTableKeys} />
                            </ModalBody>
                            <ModalFooter>
                                <Button color="danger" variant="light" onPress={onClose}>
                                    Close
                                </Button>
                                <Button color="primary" onPress={onSelectTableModalClose} >
                                    {
                                        !!selectedTable && selectedTable.length > 0 && (< span > {selectedTable?.length}条</span>)
                                    } 确认选择
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>

            {/* 查看依赖表 */}
            <Modal isOpen={isShowDependencyTablesOpen} onClose={onShowDependencyTablesClose} size='5xl' scrollBehavior='inside'>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex flex-col gap-1">查看依赖表</ModalHeader>
                            <ModalBody>
                                <div className="space-y-3 w-full">
                                    {showDependencyTableList.map((item: any, index: number) => (
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
