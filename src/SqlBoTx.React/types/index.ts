
//当前用户
export type User = {
    id: number;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    age?: number; 
    createdAt: Date;
    isActive: boolean;
}
