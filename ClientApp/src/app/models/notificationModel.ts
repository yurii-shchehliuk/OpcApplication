import { LogCategory } from "./enums";

export interface NotificationData {
  message: string;
  title: string;
  logCategory: LogCategory;
}
