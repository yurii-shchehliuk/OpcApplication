import { MonitoredItemValue } from './monitoredItem';

export interface SubscriptionValue {
  subscriptionGuidId: string;
  opcUaId: number;
  displayName: string;
  publishingInterval: number;
  itemsCount: number;
  sequenceNumber: number;
  publishingEnabled: boolean;
  monitoredItems: MonitoredItemValue[];
}

export interface SubscriptionConfig {
  subscriptionGuidId: string;
  opcUaId: number;
  displayName: string;
  publishingInterval: number;
  keepAliveCount: number;
  lifetimeCount: number;
  maxNotificationsPerPublish: number;
  priority: number;
  publishingEnabled: boolean;
}
