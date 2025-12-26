import type { User } from "@/types"
import { fetchEventSource, type EventSourceMessage } from '@microsoft/fetch-event-source';


interface ApiResponse<T = any> {
  code: number
  msg: any
  data?: T | null
}

interface ApiRequestConfig {
  headers?: Record<string, string>
  timeout?: number
  credentials?: RequestCredentials
}

class ApiService {
  private defaultHeaders: Record<string, string>
  private defaultTimeout: number
  private abortController?: AbortController;

  constructor() {
    this.defaultTimeout = 15000
    this.defaultHeaders = {
      'Content-Type': 'application/json',
    }
    if (import.meta.env.MODE === 'development') {
      console.group('🔧 API Configuration')
      console.log('Environment:', import.meta.env.MODE)
      console.log('defaultTimeout:', this.defaultTimeout)
      console.log('defaultHeaders:', this.defaultHeaders)
      console.groupEnd()
    }


  }

  // 设置默认请求头
  setDefaultHeaders(headers: Record<string, string>) {
    this.defaultHeaders = { ...this.defaultHeaders, ...headers }
  }


  // 通用请求方法
  private async request<T>(
    endpoint: string,
    options: RequestInit & ApiRequestConfig = {}
  ): Promise<ApiResponse<T>> {

    const url = import.meta.env.VITE_APP_BASEURL + endpoint;
    const { timeout = this.defaultTimeout, ...fetchOptions } = options

    if (import.meta.env.MODE === 'development') {
      console.log(`🌐 API Request: ${options.method || 'GET'} ${url}`)
    }

    // 合并请求头
    const headers: Record<string, string> = {
      ...this.defaultHeaders,
      ...(options.headers as Record<string, string> || {}),
    }

    if (options.body instanceof FormData) {
      delete headers['Content-Type'];
    }

    try {

      //发送请求
      const response = await fetch(url, {
        ...fetchOptions,
        headers,
        credentials: options.credentials || 'include',
      })

      const result = await response.json();
      return {
        code: response.status,
        msg: result,
        data: result,
      }
    }
    //异常处理
    catch (error: any) {

      // 在开发环境下输出错误信息
      if (import.meta.env.MODE === 'development') {
        console.error(`❌ API Error:`, error)
      }

      if (error instanceof TypeError) {
        return {
          code: 500,
          msg: '服务异常，请稍后重试',
          data: null as any,
        }
      }

      return {
        code: 500,
        msg: error?.message || '网络请求失败',
        data: null as any,
      }

    }
  }

  // GET请求
  async get<T>(endpoint: string, config?: ApiRequestConfig): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'GET',
      ...config,
    })
  }

  // POST请求
  async post<T>(
    endpoint: string,
    data?: any,
    config?: ApiRequestConfig
  ): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
      ...config,
    })
  }

  // 通用请求方法
  async postStream<T>(
    endpoint: string,
    data?: any,
    callOk?: () => void,
    callChunk?: (msg: any) => void,
  ): Promise<any> {

    const url = import.meta.env.VITE_APP_BASEURL + endpoint;

    // 合并请求头
    const headers = {
      ...this.defaultHeaders,
    }
    if (data instanceof FormData) {
      delete headers['Content-Type'];
    }

    try {

      const response = await fetch(url, {
        method: 'POST',
        body: data ? JSON.stringify(data) : undefined,
        headers,
        credentials: 'include',
      })

      !!callOk ? callOk() : null;

      // 获取可读流
      //@ts-ignore
      const reader = response.body.getReader();
      const decoder = new TextDecoder();
      while (true) {
        const { done, value } = await reader.read();
        if (done) break; // 如果流结束，退出循环

        // 解码并处理数据
        const chunk = decoder.decode(value);

        const lines = chunk.split('\n');

        // 保留最后一行（可能不完整）
        lines.pop() || '';

        for (const line of lines) {
          if (line.startsWith('data: ')) {
            const data = line.slice(6);
            try {
              console.log('SSE Raw Data:', data);
              const message = JSON.parse(data) as any;
              !!callChunk ? callChunk(message) : null;
            } catch (e) {
              console.error('Failed to parse SSE message:', e, data);
            }
          }
        }
      }

      const result = await response.json();
      return {
        code: response.status,
        msg: result,
        data: result,
      }

    } catch (error) {

      if (error instanceof Error && error.name === 'AbortError') {

        return {
          code: 500,
          msg: '请求超时',
          data: null as any,
        }

      } else {

        return {
          code: 500,
          msg: error,
          data: null as any,
        }

      }
    }
  }


  // 验证用户登录
  async checkUser(extra?: any): Promise<User | null> {
    try {
      const res = await this.get<User>('/checkUser', {
        credentials: "include",
        ...extra,
      });
      if (res.code !== 200) {
        return null;
      }
      return res.data as User;
    } catch (error) {
      return null;
    }
  }

  // 登录方法
  async login(account: string, password: string): Promise<ApiResponse<User>> {
    const res = await this.get<any>('/login?account=' + account + '&password=' + password, {
      credentials: "include",
    });
    console.log('登录响应:', res)
    if (res.code == 200) {
      window.location.reload();
    }
    return res;
  }


  // 开始 sse
  async sseEventSource<T>(
    endpoint: string,
    data: any,
    open: () => void,
    message: (msg: EventSourceMessage) => void,
    colse: () => void,
  ): Promise<any> {

    const url = import.meta.env.VITE_APP_BASEURL + endpoint;

    if (this.abortController) {
      this.abortController.abort();
    }
    this.abortController = new AbortController();
    const ctr = this.abortController;




    await fetchEventSource(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: data ? JSON.stringify(data) : undefined,
      openWhenHidden: true,
      signal: ctr.signal,
      async onopen(response) {
        console.log(response)
        open();
      },
      onmessage(ev) {
        console.log(ev);
        message(ev)
      },
      onclose() {
        console.log('onclose');
        colse();
      },
      onerror(err) {
        console.log(err);
        throw err;
      }
    },
    );
  }

  // 停止 sse
  sseEventStop(close: () => void) {
    if (this.abortController) {
      // 发送中断信号
      this.abortController.abort();
      // 重新初始化以便下次使用（或者在 start 时初始化也可以）
      this.abortController = new AbortController();

      console.log('SSE 请求已中止');
      !!close && close();
    }
  }
}




// 创建API服务实例
export const apiService = new ApiService()

// 导出类型
export type { ApiResponse, ApiRequestConfig }

// 便捷的方法导出
export const {
  get,
  post,
  checkUser,
  login
} = apiService