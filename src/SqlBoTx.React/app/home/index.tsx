import { NavLink, redirect, useLocation } from "react-router";
import { useState } from "react";
import { useLoaderData } from "react-router";
import { Tooltip, Button } from "@heroui/react";
import { Tabs, Tab, Card, CardBody, Switch } from "@heroui/react";
import { Communication, Github, SeoFolder, TableFile } from "@icon-park/react";

import { Bot } from "@/components/icons";

export function loader() {
  // -------------------------------------------------
  // 选项 A：简单跳转 (最常用)
  // 跳转到 /chat，让 Chat 组件自己决定是否生成新 ID
  // -------------------------------------------------
  return redirect("/chat");


  // -------------------------------------------------
  // 选项 B：生成 UUID 并跳转 (更主动)
  // 如果你希望 URL 一开始就带上 ID，比如 /chat/550e84...
  // -------------------------------------------------
  // const newId = crypto.randomUUID();
  // return redirect(`/chat/${newId}`);
}

//服务端加载数据
// export async function loader(): Promise<any[]> {

//   return redirect("/chat");

//   return [

//   ]
// }

export default function Home() {

  // 示例项目数据
  const projects: any[] = useLoaderData<typeof loader>();

  const { pathname } = useLocation();
  const [isVertical, setIsVertical] = useState(true);

  console.log("当前路径:", pathname);

 

  return (
    <div>
      home
    </div>
  );
};

