import { MonitoringMode, NodeClass } from './enums';

export interface MonitoredItemConfig {
  displayName: string;
  startNodeId: string;
  samplingInterval: number;
  queueSize: number;
  discardOldest: boolean;
  attributeId: string;
  indexRange: string;
  relativePath: string;
  nodeClass: NodeClass;
  monitoringMode: MonitoringMode;
}

export interface MonitoredItemValue {
  subscriptionOpcId: number;
  displayName: string;
  value: number;
  samplingInterval: number;
  queueSize: number;
  startNodeId: string;
  createdAt: string;
}
