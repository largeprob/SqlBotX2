import { HeroUIProvider } from "@heroui/react";
import { ToastProvider } from "@heroui/react";
export function HerouiProviders({ children }: { children: React.ReactNode }) {
    return (
        <HeroUIProvider>
            <ToastProvider placement='top-center' />
            {children}
        </HeroUIProvider>
    );
}
