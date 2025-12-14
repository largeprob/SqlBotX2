import { type RouteConfig, index, layout, route } from "@react-router/dev/routes";

export default [
    layout("layout.tsx", [
        index("chat/index.tsx"),
        route("home", "home/index.tsx"),
    ]),

] satisfies RouteConfig;
