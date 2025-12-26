import { type RouteConfig, index, layout, route } from "@react-router/dev/routes";
import { redirect } from "react-router";

export default [
    layout("layout.tsx", [
        index("home/index.tsx"),
        route("chat/:sessionId?", "chat/index.tsx"),
    ]),
] satisfies RouteConfig;
