/** SSE 事件类型 */
export type SSEEventType = 'delta' | 'block' | 'done' | 'error';

/** 基础 SSE 消息 */
export interface SSEMessage {
  type: SSEEventType;
}

/** 增量文本消息（流式文本输出） */
export interface DeltaMessage extends SSEMessage {
  type: 'delta';
  delta: string;
}

/** 内容块消息（SQL、数据、图表等特殊内容） */
export interface BlockMessage extends SSEMessage {
  type: 'block';
  block: ContentBlock;
}

/** 完成消息 */
export interface DoneMessage extends SSEMessage {
  type: 'done';
  elapsedMs: number;
}

/** 错误消息 */
export interface ErrorMessage extends SSEMessage {
  type: 'error';
  code: string;
  message: string;
  details?: string;
}


/** 内容块类型 */
export type ContentBlockType = 'sql' | 'table' | 'echarts' | 'error';

/** 内容块基础接口 */
export interface ContentBlock {
  id: string;
  type: ContentBlockType;
}


/** 内容项类型 - 包含所有可能的内容类型 */
export type ContentItemType = 'text' | 'sql' | 'table' | 'echarts' | 'error';

/** 内容项基础接口 */
export interface ContentItem {
  id: string;
  type: ContentItemType;
}

/** 文本内容项 */
export interface TextContentItem extends ContentItem {
  type: 'text';
  content: string;
}

/** SQL 内容项 */
export interface SqlContentItem extends ContentItem {
  type: 'sql';
  sql: string;
  tables: string[];
  dialect?: string;
}

/** 数据表格内容项 */
export interface TableContentItem extends ContentItem {
  type: 'table';
  columns: string[];
  rows: any[][];
  totalRows: number;
}

/** 图表内容项 */
export interface ChartContentItem extends ContentItem {
  type: 'echarts';
  chartType: string;
  echartsOption?: string;
  config: ChartConfig;
  data: any;
}

/** 错误内容项 */
export interface ErrorContentItem extends ContentItem {
  type: 'error';
  code: string;
  message: string;
  details?: string;
}

/** SQL 代码块 */
export interface SqlBlock extends ContentBlock {
  type: 'sql';
  sql: string;
  tables: string[];
  dialect?: string;
}

/** 数据表格块 */
export interface TableBlock extends ContentBlock {
  type: 'table';
  columns: string[];
  rows: any[][];
  totalRows: number;
}

/** 图表块 */
export interface EchartsBlock extends ContentBlock {
  type: 'echarts';
  chartType: string;
  echartsOption?: string; // ECharts option 配置 JSON 字符串
  config: ChartConfig;
  data: any;
}

export interface ChartConfig {
  xAxis?: string;
  yAxis?: string[];
  title?: string;
  showLegend: boolean;
}

/** 错误块 */
export interface ErrorBlock extends ContentBlock {
  type: 'error';
  code: string;
  message: string;
  details?: string;
}