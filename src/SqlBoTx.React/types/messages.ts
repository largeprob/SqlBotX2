// 对话角色类型
export type Role = 'user' | 'assistant' | 'system'

//数据块类型
export type BlockType = 'text' | 'thought' | 'sql' | 'echarts' | 'table' | 'file';



//块级状态
export type BlockStatus = 'streaming' | 'done' | 'error';


//=======================消息体
export interface BaseContentBlock {
  id?: string | null;
  type: BlockType;
  status: BlockStatus;
  createdAt: number;
}

// 1思考模式
export interface ContentBlockThought extends BaseContentBlock {
  type: 'thought';
  content: string;
  isCollapsed?: boolean;
  duration?: number;
}

// 2正文文本块
export interface ContentBlockText extends BaseContentBlock {
  type: 'text';

  text: string;
}

// 3SQL块
export interface ContentBlockSql extends BaseContentBlock {
  blockType: 'sql';
  sql: string;
}

// 4图表块
export interface ContentBlockEcharts extends BaseContentBlock {
  type: 'echarts';
  options: string;
}

// 5表格块
export interface ContentBlockTable extends BaseContentBlock {
  type: 'table';
  id: string;
  columns: Array<{ title: string; dataIndex: string; key: string }>;
  item: Array<Record<string, any>>;
  pagination?: {
    current: number;
    pageSize: number;
    total: number;
  };
  summary?: string;
}

// 6文件
export interface ContentBlockFile extends BaseContentBlock {
  blockType: 'file';
  name: string;
  url: string;
  fileType?: string;
}


// 联合类型
export type ContentBlock =
  | ContentBlockThought
  | ContentBlockText
  | ContentBlockSql
  | ContentBlockEcharts
  | ContentBlockTable;


// 消息整体状态 
export type MessageStatus = 'pending' | 'streaming' | 'done' | 'error';
// 基础信息
export interface ChatMessage {
  sessionId?: string | null;
  id?: string | null;
  role: Role;
  content: ContentBlock[];
  status: MessageStatus;
  statusStr?: string
  createdAt: number;
  errorMsg?: string;
}
