import { NavLink, redirect, useLocation } from "react-router";
import { useState } from "react";
import { useLoaderData } from "react-router";


export function loader() {
  return redirect("/baseSetting/businessObjective");
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

