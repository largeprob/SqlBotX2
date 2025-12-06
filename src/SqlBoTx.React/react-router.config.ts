import type { Config } from "@react-router/dev/config";

export default {
  //启用中间件
  future: {
    v8_middleware: true,
  },
  // ssr: true,
} satisfies Config;
