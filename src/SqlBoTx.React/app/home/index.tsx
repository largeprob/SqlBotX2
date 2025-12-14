import { NavLink, useLocation } from "react-router";
import { useState } from "react";
import { useLoaderData } from "react-router";
import { Tooltip, Button } from "@heroui/react";
import { Tabs, Tab, Card, CardBody, Switch } from "@heroui/react";
import { Communication, Github, SeoFolder, TableFile } from "@icon-park/react";

import { Bot } from "@/components/icons";

//服务端加载数据
export async function loader(): Promise<any[]> {
  return [

  ]
}

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

