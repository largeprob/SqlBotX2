import { apiService } from '@/lib/api';
import DbForm from '@/components/page/tables/db-form';
import type { Route } from '../+types/root';

//服务端加载数据
export async function loader({ params }: Route.LoaderArgs): Promise<any> {
    console.log("loader Params:", params);
    return []
}

// 2. 客户端加载器
export async function clientLoader({
    serverLoader,
    params,
}: Route.ClientLoaderArgs): Promise<any> {
    console.log("clientLoader Params:", params);
    const serverData = await serverLoader();
    // 合并两边的数据
    return serverData;
}

export default function Index({
    loaderData,
}: Route.ComponentProps) {
    return (
        <>
            <DbForm />
        </>
    );
};