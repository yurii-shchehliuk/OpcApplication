import { MonitoredItemValue } from './monitoredItem';

export interface SubscriptionValue {
  guid: string;
  opcUaId: number;
  displayName: string;
  publishingInterval: number;
  itemsCount: number;
  sequenceNumber: number;
  publishingEnabled: boolean;
  monitoredItems: MonitoredItemValue[];
}

export interface SubscriptionConfig {
  guid: string;
  opcUaId: number;
  displayName: string;
  minValue: number;
  maxValue: number;
  publishingInterval: number;
  keepAliveCount: number;
  lifetimeCount: number;
  maxNotificationsPerPublish: number;
  priority: number;
  publishingEnabled: boolean;
}
