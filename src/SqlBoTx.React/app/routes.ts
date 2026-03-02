import { type RouteConfig, index, layout, route, prefix } from "@react-router/dev/routes";
import { redirect } from "react-router";

export default [
    layout("layout.tsx", [
        index("home/index.tsx"),
        route("table", "table/index.tsx"),
        route("chat/:sessionId?", "chat/index.tsx"),


        // route("baseSetting/", "baseSetting/index.tsx"),


        ...prefix("baseSetting", [
            index("baseSetting/index.tsx"),
            layout("baseSetting/layout.tsx", [
                route("businessObjective", "baseSetting/businessObjective/index.tsx"),
                route("businessMetric", "baseSetting/businessMetric/index.tsx"),
            ]),
        ]),

    ]),
] satisfies RouteConfig;
