import { LogCategory } from "./enums";

export interface EventData {
  message: string;
  title: string;
  logCategory: LogCategory;
}
