/**
 * 复制文本到剪贴板的工具函数
 * 提供多种fallback方法以确保在不同环境下都能工作
 */
export async function copyToClipboard(text: string): Promise<boolean> {
  // 方法1: 尝试现代 Clipboard API
  try {
    if (navigator.clipboard && window.isSecureContext) {
      await navigator.clipboard.writeText(text);
      return true;
    }
  } catch (error) {
    // Clipboard API被阻止，继续尝试fallback方法
    console.warn('Clipboard API 不可用，尝试fallback方法');
  }

  // 方法2: 使用 execCommand fallback
  try {
    const textArea = document.createElement('textarea');
    textArea.value = text;
    
    // 设置样式使其不可见但仍可选择
    textArea.style.position = 'fixed';
    textArea.style.left = '-9999px';
    textArea.style.top = '-9999px';
    textArea.style.width = '1px';
    textArea.style.height = '1px';
    textArea.style.padding = '0';
    textArea.style.border = 'none';
    textArea.style.outline = 'none';
    textArea.style.boxShadow = 'none';
    textArea.style.background = 'transparent';
    textArea.setAttribute('readonly', '');
    
    document.body.appendChild(textArea);
    
    // 选择文本
    textArea.focus();
    textArea.select();
    textArea.setSelectionRange(0, text.length);
    
    // 执行复制命令
    const successful = document.execCommand('copy');
    document.body.removeChild(textArea);
    
    if (successful) {
      return true;
    }
  } catch (error) {
    console.warn('execCommand 复制失败:', error);
  }

  // 如果所有方法都失败，返回false
  return false;
}

/**
 * 检查是否支持复制功能
 * 更保守的检查，主要检查execCommand支持
 */
export function isCopySupported(): boolean {
  // 优先检查execCommand支持，因为它更可靠
  if (document.queryCommandSupported && document.queryCommandSupported('copy')) {
    return true;
  }
  
  // 作为backup检查Clipboard API（但实际使用时可能仍会失败）
  if (navigator.clipboard && window.isSecureContext) {
    return true;
  }
  
  return false;
}