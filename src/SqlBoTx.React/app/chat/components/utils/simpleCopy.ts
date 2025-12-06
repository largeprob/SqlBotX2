/**
 * 简化的复制工具，主要使用 execCommand
 * 避免使用可能被策略阻止的 Clipboard API
 */

export function copyTextLegacy(text: string): boolean {
  try {
    // 创建临时的textarea元素
    const textarea = document.createElement('textarea');
    textarea.value = text;
    
    // 设置样式，使其不可见但可以被选中
    textarea.style.position = 'fixed';
    textarea.style.top = '0';
    textarea.style.left = '0';
    textarea.style.width = '2em';
    textarea.style.height = '2em';
    textarea.style.padding = '0';
    textarea.style.border = 'none';
    textarea.style.outline = 'none';
    textarea.style.boxShadow = 'none';
    textarea.style.background = 'transparent';
    
    // 添加到DOM
    document.body.appendChild(textarea);
    
    // 选择文本
    textarea.focus();
    textarea.select();
    
    let successful = false;
    try {
      // 尝试复制
      successful = document.execCommand('copy');
    } catch (err) {
      console.warn('execCommand copy failed:', err);
      successful = false;
    }
    
    // 清理
    document.body.removeChild(textarea);
    
    return successful;
  } catch (error) {
    console.error('Legacy copy failed:', error);
    return false;
  }
}

/**
 * 检查是否支持 execCommand copy
 */
export function supportsLegacyCopy(): boolean {
  try {
    return document.queryCommandSupported && document.queryCommandSupported('copy');
  } catch {
    return false;
  }
}

/**
 * 显示复制对话框
 */
export function showCopyDialog(content: string): void {
  const overlay = document.createElement('div');
  overlay.className = 'copy-dialog-overlay';
  overlay.style.cssText = `
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.6);
    backdrop-filter: blur(4px);
    z-index: 50000;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 16px;
    animation: fadeIn 0.2s ease-out;
  `;

  const dialog = document.createElement('div');
  dialog.style.cssText = `
    background: #ffffff;
    border-radius: 12px;
    box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
    max-width: 600px;
    width: 100%;
    max-height: 80vh;
    display: flex;
    flex-direction: column;
    overflow: hidden;
  `;

  // 添加CSS动画
  const style = document.createElement('style');
  style.textContent = `
    @keyframes fadeIn {
      from { opacity: 0; }
      to { opacity: 1; }
    }
    .copy-dialog-overlay * {
      box-sizing: border-box;
    }
  `;
  document.head.appendChild(style);

  dialog.innerHTML = `
    <div style="
      padding: 20px 24px;
      border-bottom: 1px solid #e5e7eb;
      display: flex;
      align-items: center;
      justify-content: space-between;
    ">
      <h3 style="
        margin: 0;
        font-size: 18px;
        font-weight: 600;
        color: #111827;
      ">复制文本内容</h3>
      <button class="close-btn" style="
        background: none;
        border: none;
        font-size: 20px;
        cursor: pointer;
        color: #6b7280;
        padding: 4px;
        border-radius: 4px;
        display: flex;
        align-items: center;
        justify-content: center;
        width: 28px;
        height: 28px;
      " onmouseover="this.style.background='#f3f4f6'" onmouseout="this.style.background='none'">✕</button>
    </div>
    
    <div style="padding: 20px 24px; flex: 1; overflow: hidden;">
      <p style="
        margin: 0 0 16px 0;
        color: #6b7280;
        font-size: 14px;
        line-height: 1.5;
      ">由于浏览器限制，无法自动复制。请手动选择下面的文本并复制（Ctrl+C 或 Cmd+C）：</p>
      
      <textarea readonly style="
        width: 100%;
        min-height: 200px;
        max-height: 300px;
        padding: 12px;
        border: 2px solid #10a37f;
        border-radius: 8px;
        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        font-size: 14px;
        line-height: 1.5;
        resize: vertical;
        outline: none;
        background: #f9fafb;
        color: #111827;
      " onfocus="this.select()">${content.replace(/</g, '<').replace(/>/g, '>')}</textarea>
    </div>
    
    <div style="
      padding: 16px 24px;
      background: #f9fafb;
      border-top: 1px solid #e5e7eb;
      display: flex;
      justify-content: flex-end;
      gap: 12px;
    ">
      <button class="select-btn" style="
        background: #10a37f;
        color: white;
        border: none;
        padding: 8px 16px;
        border-radius: 6px;
        font-size: 14px;
        font-weight: 500;
        cursor: pointer;
      " onmouseover="this.style.background='#0d9568'" onmouseout="this.style.background='#10a37f'">全选文本</button>
      
      <button class="close-btn" style="
        background: #6b7280;
        color: white;
        border: none;
        padding: 8px 16px;
        border-radius: 6px;
        font-size: 14px;
        font-weight: 500;
        cursor: pointer;
      " onmouseover="this.style.background='#4b5563'" onmouseout="this.style.background='#6b7280'">关闭</button>
    </div>
  `;

  overlay.appendChild(dialog);

  const textarea = dialog.querySelector('textarea') as HTMLTextAreaElement;
  const closeButtons = dialog.querySelectorAll('.close-btn');
  const selectButton = dialog.querySelector('.select-btn') as HTMLButtonElement;

  const cleanup = () => {
    document.body.removeChild(overlay);
    document.head.removeChild(style);
  };

  // 事件监听
  closeButtons.forEach(btn => btn.addEventListener('click', cleanup));
  selectButton.addEventListener('click', () => {
    textarea.focus();
    textarea.select();
  });

  overlay.addEventListener('click', (e) => {
    if (e.target === overlay) cleanup();
  });

  // ESC键关闭
  const handleEsc = (e: KeyboardEvent) => {
    if (e.key === 'Escape') {
      cleanup();
      document.removeEventListener('keydown', handleEsc);
    }
  };
  document.addEventListener('keydown', handleEsc);

  // 添加到页面
  document.body.appendChild(overlay);
  
  // 自动选择文本
  setTimeout(() => {
    textarea.focus();
    textarea.select();
  }, 100);
}