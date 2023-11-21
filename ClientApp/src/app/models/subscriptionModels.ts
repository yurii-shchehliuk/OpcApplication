export interface SubscriptionEntity {
  id: number;
  displayName: string;
  publishInterval: number;
  itemsCount: number;
  sequenceNumber: number;
  publishingEnabled: boolean;
  monitoringItems: MonitoringItemEntity[];
}

export interface MonitoringItemEntity {
  id: number;
  displayName: string;
  samplingInterval: number;
  queueSize: number;
  startNodeId: string;
  value: string;
  sourceTime: string;
}

export interface SubscriptionParameters {
  displayName: string;
  publishingInterval: number;
  keepAliveCount: number;
  lifetimeCount: number;
  maxNotificationsPerPublish: number;
  priority: number;
  publishingEnabled: boolean;
}
