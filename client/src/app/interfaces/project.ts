import { ITimeRegistration } from "./timeregistration";

export interface IProject {
    id: number;
    name: string;
    status: number;
    deadline: Date;
    timeRegistrations: ITimeRegistration[];
}