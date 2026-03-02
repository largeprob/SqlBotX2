// .NET 标准的 ProblemDetails 结构 RFC 7807
interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;         // 业务异常信息通常在这里
  errors?: Record<string, string[]>; // 模型验证错误在这里
  traceId?: string;
}