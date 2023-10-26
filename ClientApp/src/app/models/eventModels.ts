export interface EventData {
  message: string;
  title: string;
  logCategory: LogCategory;
}
export enum LogCategory {
  info,
  success,
  warning,
  error,
  critical,
}
