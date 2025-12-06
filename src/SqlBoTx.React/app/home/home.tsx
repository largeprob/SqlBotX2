import { NavLink } from "react-router";
import { ExternalLink, Code, Database, Github, Smartphone, Globe, Bot } from 'lucide-react';
import { useState } from "react";
import { useLoaderData } from "react-router";

//服务端加载数据
export async function loader(): Promise<any[]> {
  return [
    {
      id: 1,
      title: "Sk-Chat",
      image: "https://images.unsplash.com/photo-1563013544-824ae1b704d3?w=400&h=200&fit=crop",
      description: "基于SK基础的AI对话应用，支持多轮对话、上下文记忆、Function calling调用、MCP功能。",
      techStack: ["Semantic-kernel", "DeepSeek"],
      route: "/chat",
      category: "人工智能",
    },
    {
      id: 2,
      title: "Sk-RAG知识库",
      image: "https://images.unsplash.com/photo-1611224923853-80b023f02d71?w=400&h=200&fit=crop",
      description: "基于Skematic-kernel的RAG知识库应用，支持文档上传、检索和问答功能。",
      techStack: ["Semantic-kernel", "DeepSeek"],
      route: "/projects/task-manager",
      category: "Web应用"
    },
    {
      id: 3,
      title: "数据可视化仪表板",
      image: "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400&h=200&fit=crop",
      description: "实时数据分析和可视化平台，支持多种图表类型，自定义仪表板布局，数据导入导出功能。",
      techStack: ["D3.js", "React", "FastAPI", "Redis"],
      route: "/projects/dashboard",
      category: "数据分析"
    },
    {
      id: 4,
      title: "移动端社交应用",
      image: "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=400&h=200&fit=crop",
      description: "基于位置的社交应用，用户可以发现附近的人和事件，分享动态，实时聊天。支持图片和视频分享。",
      techStack: ["React Native", "Firebase", "Node.js", "Socket.io"],
      route: "/projects/social-app",
      category: "移动开发"
    },
    {
      id: 5,
      title: "AI智能问答系统",
      image: "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=400&h=200&fit=crop",
      description: "基于自然语言处理的智能问答系统，支持多轮对话，知识库检索，自动回复等功能。",
      techStack: ["Python", "TensorFlow", "BERT", "Flask"],
      route: "/projects/ai-qa",
      category: "人工智能"
    },
    {
      id: 6,
      title: "区块链投票系统",
      image: "https://images.unsplash.com/photo-1639762681485-074b7f938ba0?w=400&h=200&fit=crop",
      description: "基于区块链技术的透明投票系统，确保投票的安全性和不可篡改性，支持实时统计和结果公示。",
      techStack: ["Solidity", "Web3.js", "Ethereum", "React"],
      route: "/projects/blockchain-voting",
      category: "区块链"
    }
  ]
}

export default function Home() {

  // 示例项目数据
  const projects: any[] = useLoaderData<typeof loader>();

  const handleCardClick = (route: string) => {
    // 这里可以使用 React Router 进行路由跳转
    console.log('跳转到:', route);
    // 实际项目中应该使用: navigate(route);
    alert(`将跳转到: ${route}`);
  };

  const getTechIcon = (tech: any) => {
    const iconMap = {
      'React': <Code className="w-4 h-4" />,
      'Vue.js': <Code className="w-4 h-4" />,
      'Node.js': <Database className="w-4 h-4" />,
      'Python': <Code className="w-4 h-4" />,
      'React Native': <Smartphone className="w-4 h-4" />,
      'MongoDB': <Database className="w-4 h-4" />,
      'PostgreSQL': <Database className="w-4 h-4" />,
      'Web3.js': <Globe className="w-4 h-4" />,
      'DeepSeek': <Bot className="w-4 h-4" />,
    };
    return iconMap[tech] || <Code className="w-4 h-4" />;
  };

  const getCategoryColor = (category: any) => {
    const colorMap = {
      '全栈开发': 'bg-blue-100 text-blue-800',
      'Web应用': 'bg-green-100 text-green-800',
      '数据分析': 'bg-purple-100 text-purple-800',
      '移动开发': 'bg-orange-100 text-orange-800',
      '人工智能': 'bg-red-100 text-red-800',
      '区块链': 'bg-yellow-100 text-yellow-800'
    };
    return colorMap[category] || 'bg-gray-100 text-gray-800';
  };


  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-indigo-100">
      {/* 页面头部 */}
      <div className="relative overflow-hidden bg-gradient-to-r from-blue-600 via-purple-600 to-indigo-700 text-white">
        <div className="absolute inset-0 bg-black opacity-20"></div>
        <div className="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24">
          <div className="text-center">
            <h1 className="text-4xl md:text-6xl font-bold mb-6 bg-clip-text text-transparent bg-gradient-to-r from-white to-blue-200">
              我的AI项目作品集
            </h1>
            <p className="text-xl md:text-2xl text-blue-100 max-w-3xl mx-auto leading-relaxed">
              本页面展示了我基于.NET Semantic-kernel、Python LangChain/LangGraph等技术栈开发的多个AI项目，包括Ai对话、RAG知识库、语音克隆、模型训练和微调等能力。
            </p>
            <div className="mt-8 flex justify-center">
              <div className="w-24 h-1 bg-gradient-to-r from-blue-400 to-purple-400 rounded-full"></div>
            </div>
          </div>
        </div>
        {/* 装饰元素 */}
        <div className="absolute top-0 left-0 w-full h-full">
          <div className="absolute top-20 left-10 w-20 h-20 bg-white opacity-10 rounded-full animate-pulse"></div>
          <div className="absolute top-40 right-20 w-16 h-16 bg-blue-300 opacity-20 rounded-full animate-bounce"></div>
          <div className="absolute bottom-20 left-1/4 w-12 h-12 bg-purple-300 opacity-15 rounded-full animate-pulse"></div>
        </div>
      </div>

      {/* 项目卡片网格 */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {projects.map((project) => (
            <NavLink to={project.route} target="_blank" key={project.id}>
              <div
                // key={project.id}
                onClick={() => handleCardClick(project.route)}
                className="group relative bg-white rounded-2xl shadow-lg hover:shadow-2xl transform hover:-translate-y-2 transition-all duration-300 cursor-pointer overflow-hidden border border-gray-100"
              >
                {/* 项目图片 */}
                <div className="relative overflow-hidden">
                  <img
                    src={project.image}
                    alt={project.title}
                    className="w-full h-48 object-cover group-hover:scale-110 transition-transform duration-500"
                  />
                  <div className="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                  <div className="absolute top-4 right-4">
                    <span className={`px-3 py-1 rounded-full text-xs font-semibold ${getCategoryColor(project.category)}`}>
                      {project.category}
                    </span>
                  </div>
                  <div className="absolute top-4 left-4 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                    <ExternalLink className="w-6 h-6 text-white" />
                  </div>
                </div>

                {/* 项目内容 */}
                <div className="p-6">
                  <h3 className="text-xl font-bold text-gray-900 mb-3 group-hover:text-blue-600 transition-colors">
                    {project.title}
                  </h3>
                  <p className="text-gray-600 text-sm leading-relaxed mb-4 line-clamp-3">
                    {project.description}
                  </p>

                  {/* 技术栈 */}
                  <div className="mb-4">
                    <h4 className="text-sm font-semibold text-gray-700 mb-2">技术栈:</h4>
                    <div className="flex flex-wrap gap-2">
                      {project.techStack.map((tech, index) => (
                        <span
                          key={index}
                          className="inline-flex items-center gap-1 px-3 py-1 bg-gray-100 text-gray-700 text-xs rounded-full hover:bg-blue-100 hover:text-blue-700 transition-colors"
                        >
                          {getTechIcon(tech)}
                          {tech}
                        </span>
                      ))}
                    </div>
                  </div>

                  {/* 查看详情按钮 */}
                  <div className="flex items-center justify-between pt-4 border-t border-gray-100">
                    <span className="text-sm text-gray-500">点击查看详情</span>
                    <div className="flex items-center text-blue-600 group-hover:text-blue-700">
                      <span className="text-sm font-medium mr-1">了解更多</span>
                      <ExternalLink className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
                    </div>
                  </div>
                </div>

                {/* 悬浮效果装饰 */}
                <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-blue-600/5 to-purple-600/5 opacity-0 group-hover:opacity-100 transition-opacity duration-300 pointer-events-none"></div>
              </div>
            </NavLink>

          ))}
        </div>
      </div>

      {/* 页脚 */}
      <footer className="bg-gray-900 text-white py-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <h3 className="text-2xl font-bold mb-4">联系我</h3>
            <p className="text-gray-300 mb-6">有任何项目合作想法？欢迎与我联系</p>
            <div className="flex justify-center space-x-6">
              <a href="#" className="text-gray-300 hover:text-white transition-colors">
                <Github className="w-6 h-6" />
              </a>
              <a href="#" className="text-gray-300 hover:text-white transition-colors">
                <Globe className="w-6 h-6" />
              </a>
            </div>
          </div>
        </div>
      </footer>


    </div>
  );
};

