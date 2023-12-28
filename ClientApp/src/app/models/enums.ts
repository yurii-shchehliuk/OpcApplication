export enum SessionState {
  connected,
  disconnected,
  connecting,
}

export enum LogCategory {
  info,
  success,
  warning,
  error,
  critical,
}

export enum MonitoringMode {
  Disabled,
  Sampling,
  Reporting,
}

export enum NodeClass {
  Unspecified = 0,
  Object = 1,
  Variable = 2,
  Method = 4,
  ObjectType = 8,
  VariableType = 16,
  ReferenceType = 32,
  DataType = 64,
  View = 128,
}
