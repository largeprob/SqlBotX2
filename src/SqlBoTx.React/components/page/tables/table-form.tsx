import React, { useState, useMemo, useEffect, useRef, useCallback } from 'react';
import { Alert, Button, ButtonGroup, Select, SelectItem, Tooltip, useDisclosure, type SortDescriptor } from "@heroui/react";
import { Modal, ModalContent, ModalHeader, ModalBody, ModalFooter } from "@heroui/react";
import { addToast, ToastProvider } from "@heroui/react";
import { Form, Input } from "@heroui/react";
import { Textarea } from "@heroui/react";
import { Add, AddMode, AddOne, AddThree, BatteryEmpty, Data, Delete, DropDownList, Edit, Find, InsertTable, Key, MoreOne, Server, TipsOne, Link, InvalidFiles } from '@icon-park/react';
import {
    Table,
    TableHeader,
    TableColumn,
    TableBody,
    TableRow,
    TableCell,
    DropdownTrigger,
    Dropdown,
    DropdownMenu,
    DropdownItem,
    Chip,
    User,
    Pagination,
} from "@heroui/react";
import { apiService } from '@/lib/api';



export default function Index({ connectionId, setSelectedTable, selectedTableKeys }: { connectionId: any, setSelectedTable?: any, selectedTableKeys?: Array<string> }) {

    const columns = [
        { name: "表名", uid: "tableName", sortable: true },
        { name: "表ID", uid: "tableId", sortable: true },
        { name: "连接ID", uid: "connectionId", sortable: true },
        { name: "显示名称", uid: "displayName" },
        { name: "字段概览", uid: "fieldCount", sortable: true },
        { name: "描述", uid: "description" },
        { name: "操作", uid: "actions" },
    ];

    const capitalize = (s: any) => {
        return s ? s.charAt(0).toUpperCase() + s.slice(1).toLowerCase() : "";
    }

    const INITIAL_VISIBLE_COLUMNS = ["tableId", "connectionId", "tableName", "displayName", "fieldCount", "description", "actions"];

    const api = {
        getTables: (dbId: any, query: any) => apiService.get(`/TableStructure/list-by-connection/${dbId}`),
        deleteTable: (id: any) => apiService.delete(`/TableStructure/delete/${id}`),
        addTable: (data: any) => apiService.post('/TableStructure/add', data),
        updateTable: (data: any) => apiService.post('/TableStructure/update', data),
    };

    const [tables, setTables] = useState<any[]>([]);
    const [page, setPage] = React.useState(1);
    const [limit, setLimit] = React.useState(100);
    const [total, setTotal] = React.useState(0);

    const refreshTables = async (connectionId: any) => {
        const res = await api.getTables(connectionId, {
            page: page,
            limit: limit
        });
        setTables(res.data);
        setTotal(res.data.length);
    }

    React.useEffect(() => {
        if (!connectionId) return;
        refreshTables(connectionId)
    }, [connectionId, page, total]);

    // Table 

    // --- Table Modal Handlers ---
    const { isOpen: isTableModalOpen, onOpen: onTableModalOpen, onClose: onTableModalClose } = useDisclosure();
    const { isOpen: isTableDelModalOpen, onOpen: onTableDelModalOpen, onClose: onTableDelModalClose } = useDisclosure();

    // Relationship Modal Handlers
    const { isOpen: isRelationshipModalOpen, onOpen: onRelationshipModalOpen, onClose: onRelationshipModalClose } = useDisclosure();

    // Table Form State
    const [delTable, setDelTable] = useState<any>();
    const [tableForm, setTableForm] = useState<any>();
    const formTableRef = useRef<HTMLFormElement>(null);
    const [formTableFieldErrors, setFormTableFieldErrors] = useState<any>(null);

    const tableModalHandle = (type: 'add' | 'edit' | 'delete', table?: any) => {
        if (type === 'add') {
            if (connectionId == null) {
                addToast({
                    title: "提示",
                    description: '请先选择数据连接',
                    radius: 'md',
                    color: 'danger',
                });
                return
            }
            setTableForm({
                connectionId: null,
                tableId: null,
                tableName: '',
                displayName: '',
                description: '',
                tableFields: []
            })
            onTableModalOpen();
        }

        if (type === 'edit') {
            console.log("Edit Table:", table);
            setTableForm(table);
            onTableModalOpen();
        }

        if (type === 'delete') {
            onTableDelModalOpen();
            setDelTable(table);
        }
    }

    const submitformTable = async () => {
        const form = formTableRef.current!;
        if (form.checkValidity() == false) {
            form.reportValidity()
            return
        }
        const dataJson: any = Object.fromEntries(
            new FormData(form)
        );

        if (tableForm.tableFields.length == 0) {
            setFormTableFieldErrors('请至少添加一个字段');
            return;
        } else {
            setFormTableFieldErrors(null);
        }
        dataJson.connectionId = connectionId
        dataJson.tableFields = tableForm.tableFields;
        if (!!tableForm.tableId) {
            dataJson.tableId = tableForm.tableId;
            await editTableApi(dataJson);
        } else {
            await addTableApi(dataJson);
        }
    }

    const submitDelTable = async () => {
        await delTableApi(delTable.tableId);
    }

    const restformTable = async () => {
        formTableRef.current!.reset();
    }

    const addTableApi = async (dataJson: any) => {
        const response = await apiService.post('/TableStructure/add', dataJson);
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
            onTableModalClose();
            refreshTables(connectionId)
        }
    }

    const editTableApi = async (dataJson: any) => {
        const response = await apiService.post('/TableStructure/update', dataJson);
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
            onTableModalClose();
            refreshTables(connectionId)
        }
    }

    const delTableApi = async (tableId: any) => {
        const response = await apiService.delete('/TableStructure/delete/' + tableId);
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
            setDelTable(null)
            onTableDelModalClose();
            refreshTables(connectionId)
        }
    }

    // --- Form Helpers ---
    const addColumn = () => {
        const newField = {
            fieldName: '',
            dataType: '',
            defaultValue: '',
            isPrimaryKey: false,
            isNullable: false,
            isIdentity: false,
            fieldDescription: '',
            isAvailable: true,
        };
        setTableForm((prev: any) => ({
            ...prev,
            tableFields: [...prev.tableFields, newField]
        }));
    };
    const removeColumn = (index: number) => {
        const newCols = [...tableForm.tableFields];
        newCols.splice(index, 1);
        setTableForm((prev: any) => ({ ...prev, tableFields: newCols }));
    };
    const updateColumn = (index: number, field: string, value: any) => {
        setTableForm((prev: any) => {
            const newFields = [...prev.tableFields];
            newFields[index] = { ...newFields[index], [field]: value };
            return { ...prev, tableFields: newFields };
        });
    };

    const [filterValue, setFilterValue] = React.useState("");
    const [selectedKeys, setSelectedKeys] = React.useState<any>(new Set(selectedTableKeys || []));
    const [visibleColumns, setVisibleColumns] = React.useState<any>(new Set(INITIAL_VISIBLE_COLUMNS));
    const [rowsPerPage, setRowsPerPage] = React.useState(5);
    const [sortDescriptor, setSortDescriptor] = React.useState<SortDescriptor>({
        column: "tableId",
        direction: "ascending",
    });
    const hasSearchFilter = Boolean(filterValue);

    // 显示/隐藏列
    const headerColumns = React.useMemo(() => {
        //@ts-ignore
        if (visibleColumns === "all") return columns;
        return columns.filter((column) => Array.from(visibleColumns).includes(column.uid));
    }, [visibleColumns]);


    const filteredItems = React.useMemo(() => {
        let filteredUsers = [...tables];

        if (hasSearchFilter) {
            filteredUsers = filteredUsers.filter((table) =>
                table.tableName.toLowerCase().includes(filterValue.toLowerCase()),
            );
        }

        return filteredUsers;
    }, [tables, filterValue]);

    const pages = Math.ceil(filteredItems.length / page) || 1;

    const items = React.useMemo(() => {
        const start = (page - 1) * limit;
        const end = start + limit;

        return filteredItems.slice(start, end);
    }, [page, filteredItems, page]);

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
            case "tableName":
                return (
                    <div className="flex items-center gap-3">
                        <div className="p-2 bg-indigo-50 text-indigo-600 rounded-lg">
                            <InsertTable theme="outline" size="16" fill="#333" />
                        </div>
                        <span className="font-mono font-medium text-gray-700 text-sm">{cellValue}</span>
                    </div>
                );
            case "fieldCount":
                return (
                    <div className="flex items-center gap-2">
                        <span className="px-2.5 py-0.5 bg-gray-100 text-gray-600 rounded-md text-xs font-medium border border-gray-200">
                            {item.tableFields.length} 字段
                        </span>
                        {item.tableFields.find((c: any) => c.isPrimaryKey) && (
                            <span className="flex items-center gap-1 text-[10px] text-gray-400 bg-gray-50 px-2 py-0.5 rounded-full border border-gray-100">
                                <Key size={10} />
                                {item.tableFields.find((c: any) => c.isPrimaryKey).fieldName}
                            </span>
                        )}
                    </div>
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
                            <DropdownItem key="view">查看</DropdownItem>
                            <DropdownItem key="edit" onPress={() => tableModalHandle('edit', item)}>编辑</DropdownItem>
                            <DropdownItem key="delete" onPress={() => tableModalHandle('delete', item)}>删除</DropdownItem>
                        </DropdownMenu>
                    </Dropdown>
                );
            default:
                return cellValue;
        }
    }, []);

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

    const onLimitChange = React.useCallback((e: React.ChangeEvent<HTMLSelectElement>) => {
        setLimit(parseInt(e.target.value))
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
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">轻量级语义建模</h1>
                </div>

                <Alert color='primary' title='解释'
                    description='非侵入式’的语义增强建模技术。在不改变客户原有数据库物理结构的前提下，
                        加装一个‘AI 认知层’，将机器字段翻译为业务语言。'
                />

                <div className="flex justify-between gap-3 items-end">
                    <Input
                        isClearable
                        className="w-full sm:max-w-[44%]"
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

                        <Button color='secondary' startContent={<AddThree theme="outline" size="24" fill="#FFFFFF" />} onPress={() => tableModalHandle('add')}>
                            新建表
                        </Button>
                    </div>
                </div>
                <div className="flex justify-between items-center">
                    <span className="text-default-400 text-small">Total {tables.length} users</span>
                    <label className="flex items-center text-default-400 text-small">
                        Rows per page:
                        <select
                            className="bg-transparent outline-solid outline-transparent text-default-400 text-small"
                            onChange={onLimitChange}
                        >
                            <option value="1">1</option>
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
        visibleColumns,
        onLimitChange,
        tables.length,
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
                    total={total}
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



    const tableOnSelectionChange = useCallback((keys: any) => {
        setSelectedKeys(keys)
        !!setSelectedTable && setSelectedTable(tables.filter(x => keys.has(x.tableId.toString())));
    }, [tables]);


    return (
        <>
            {/* 数据表格卡片 */}
            <Table
                isHeaderSticky
                aria-label="Example table with custom cells, pagination and sorting"
                bottomContent={bottomContent}
                bottomContentPlacement="outside"
                classNames={{
                    wrapper: "max-h-[382px]",
                }}
                selectedKeys={selectedKeys}
                selectionMode="multiple"
                sortDescriptor={sortDescriptor}
                topContent={topContent}
                topContentPlacement="outside"
                onSelectionChange={tableOnSelectionChange}
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
                <TableBody emptyContent={"No users found"} items={sortedItems}>
                    {(item) => (
                        <TableRow key={item.tableId}>
                            {(columnKey) => <TableCell>{renderCell(item, columnKey)}</TableCell>}
                        </TableRow>
                    )}
                </TableBody>
            </Table>

            {/*  新增、编辑表 Modal */}
            <Modal isOpen={isTableModalOpen} onClose={onTableModalClose} size='4xl' scrollBehavior='inside'>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader className="flex flex-col gap-1">  {!!tableForm && tableForm.tableId ? '编辑表 ' : '新建表 '}</ModalHeader>
                            <ModalBody>
                                <Form
                                    ref={formTableRef}
                                    className="w-full  flex flex-col gap-4"
                                >
                                    <Input
                                        isRequired
                                        label="表名"
                                        labelPlacement="outside"
                                        name="TableName"
                                        placeholder="输入表名"
                                        type="text"
                                        defaultValue={tableForm ? tableForm.tableName : ''}
                                    />
                                    <Input
                                        isRequired
                                        label="显示名称"
                                        labelPlacement="outside"
                                        name="DisplayName"
                                        placeholder="输入显示名称"
                                        type="text"
                                        defaultValue={tableForm ? tableForm.displayName : ''}
                                    />
                                    <Textarea
                                        label="表描述"
                                        labelPlacement="outside"
                                        name="Description"
                                        placeholder="输入表描述"
                                        type="text"
                                        defaultValue={tableForm ? tableForm.description : ''}
                                    />
                                    <div className='w-full'>
                                        {/* 字段列表标题 */}
                                        <div className="flex justify-between items-end">
                                            <div>
                                                <h3 className="text-lg font-bold text-gray-900">字段定义
                                                    <span className="text-danger text-small pl-2">
                                                        *
                                                    </span>
                                                </h3>
                                            </div>
                                        </div>

                                        {/* 字段列表 */}
                                        <div className="space-y-3 w-full">
                                            {tableForm.tableFields.map((col: any, index: number) => (
                                                <div
                                                    key={index}
                                                    className="group relative flex flex-col md:flex-row gap-3 items-start md:items-center p-3 bg-gray-50/50 border border-gray-200 rounded-xl hover:shadow-sm hover:border-indigo-300 hover:bg-white transition-all duration-200"
                                                >
                                                    <div className="hidden md:block w-8 text-center text-gray-400 font-mono text-xs select-none">
                                                        {index + 1}
                                                    </div>
                                                    <div className="flex-1 w-full space-y-1">
                                                        <label className="md:hidden text-xs text-gray-400">字段名</label>
                                                        <input
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                            placeholder="字段名"
                                                            value={col.fieldName}
                                                            onChange={(e) => updateColumn(index, 'fieldName', e.target.value)}
                                                        />
                                                    </div>
                                                    <div className="w-full md:w-40 space-y-1">
                                                        <label className="md:hidden text-xs text-gray-400">类型</label>
                                                        <input
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm font-mono transition-colors text-gray-700 placeholder-gray-400"
                                                            placeholder="字段类型"
                                                            value={col.dataType}
                                                            onChange={(e) => updateColumn(index, 'dataType', e.target.value)}
                                                        />
                                                    </div>

                                                    <div className="w-full md:w-24 space-y-1">
                                                        <label className="md:hidden text-xs text-gray-400">默认值</label>
                                                        <input
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm text-center text-gray-700 placeholder-gray-400"
                                                            placeholder="默认值"
                                                            value={col.defaultValue}
                                                            onChange={(e) => updateColumn(index, 'defaultValue', e.target.value)}
                                                        />
                                                    </div>
                                                    <div className="flex-[1.5] w-full space-y-1">
                                                        <label className="md:hidden text-xs text-gray-400">备注</label>
                                                        <input
                                                            className="w-full px-0 py-1.5 bg-transparent border-b border-gray-200 focus:border-indigo-500 focus:outline-none text-sm text-gray-700 placeholder-gray-400"
                                                            placeholder="备注说明"
                                                            value={col.fieldDescription}
                                                            onChange={(e) => updateColumn(index, 'fieldDescription', e.target.value)}
                                                        />
                                                    </div>
                                                    <div className="flex items-center gap-2 justify-end md:justify-center w-full md:w-auto mt-2 md:mt-0 pl-2 border-l border-gray-100">
                                                        <Tooltip color="foreground" content='主键' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => updateColumn(index, 'isPrimaryKey', !col.isPrimaryKey)}
                                                                className={`w-8 h-8 flex items-center justify-center rounded-lg transition-all ${col.isPrimaryKey ? 'bg-orange-100 text-orange-500 ring-1 ring-orange-200' : 'text-gray-300 hover:text-gray-500 hover:bg-gray-100'}`}
                                                            >
                                                                <Key theme="outline" size="16" fill="#333" />
                                                            </button>
                                                        </Tooltip>

                                                        <Tooltip color="foreground" content='自增' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => updateColumn(index, 'isIdentity', !col.isIdentity)}
                                                                className={`w-8 h-8 flex items-center justify-center rounded-lg transition-all ${col.isIdentity ? 'bg-orange-100 text-orange-500 ring-1 ring-orange-200' : 'text-gray-300 hover:text-gray-500 hover:bg-gray-100'}`}
                                                            >
                                                                <AddMode theme="outline" size="16" fill="#333" />
                                                            </button>
                                                        </Tooltip>

                                                        <Tooltip color="foreground" content='允许为空' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => updateColumn(index, 'isNullable', !col.isNullable)}
                                                                className={`w-8 h-8 flex items-center justify-center rounded-lg transition-all ${col.isNullable ? 'bg-orange-100 text-orange-500 ring-1 ring-orange-200' : 'text-gray-300 hover:text-gray-500 hover:bg-gray-100'}`}
                                                            >
                                                                <BatteryEmpty theme="outline" size="16" fill="#333" />
                                                            </button>
                                                        </Tooltip>

                                                        <Tooltip color="foreground" content='是否有效' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => updateColumn(index, 'isAvailable', !col.isAvailable)}
                                                                className={`w-8 h-8 flex items-center justify-center rounded-lg transition-all ${col.isAvailable ? 'bg-orange-100 text-orange-500 ring-1 ring-orange-200' : 'text-gray-300 hover:text-gray-500 hover:bg-gray-100'}`}
                                                            >
                                                                <InvalidFiles theme="outline" size="16" fill="#333" />
                                                            </button>
                                                        </Tooltip>

                                                        <Tooltip color='danger' content='删除' placement="bottom">
                                                            <button
                                                                type='button'
                                                                onClick={() => removeColumn(index)}
                                                                className="w-8 h-8 flex items-center justify-center rounded-lg text-gray-300 hover:bg-red-50 hover:text-red-500 transition-colors"
                                                                title="删除字段"
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
                                            onClick={addColumn}
                                            className="w-full py-3 border border-dashed border-gray-300 rounded-xl text-gray-500 hover:border-indigo-500 hover:text-indigo-600 hover:bg-indigo-50/50 transition-all flex items-center justify-center gap-2 text-sm font-medium mt-4"
                                        >
                                            <AddThree theme="outline" size="16" fill="#333" /> 添加新字段
                                        </button>
                                        {
                                            !!formTableFieldErrors &&
                                            <span className="text-danger text-small pl-2">
                                                {formTableFieldErrors}
                                            </span>
                                        }
                                    </div>
                                </Form>
                            </ModalBody>
                            <ModalFooter>
                                <Button color="danger" variant="light" onPress={onClose}>
                                    Close
                                </Button>
                                <Button color="danger" variant="light" onPress={restformTable}>
                                    重置
                                </Button>
                                <Button color="primary" onPress={submitformTable}>
                                    提交
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>

            {/* 删除表确认 Modal */}
            <Modal isOpen={isTableDelModalOpen} onClose={onTableDelModalClose}>
                <ModalContent>
                    {(onClose) => (
                        <>
                            <ModalHeader><TipsOne theme="outline" size="24" fill="#FB2C36" /></ModalHeader>
                            <ModalBody>
                                <p className="text-gray-500 text-sm">
                                    您确定要删除这张表吗？<br />
                                    <span className="text-red-500 font-medium">警告：该操作将同时删除其下的所有字段结构信息！</span>
                                </p>
                            </ModalBody>
                            <ModalFooter>
                                <Button variant="light" onPress={onClose}>
                                    <p className='text-gray-600'>取消</p>
                                </Button>
                                <Button color="danger" onPress={submitDelTable}>
                                    删除
                                </Button>
                            </ModalFooter>
                        </>
                    )}
                </ModalContent>
            </Modal>


        </>
    );
};