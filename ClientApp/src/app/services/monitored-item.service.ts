import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { MonitoredItemConfig } from '../models/monitoredItem';
import { SessionEntity } from '../models/sessionModels';
import { SessionAccessorService } from '../shared/session-accessor.service';
import {
  SubscriptionConfig,
  SubscriptionValue,
} from '../models/subscriptionModels';
import { RequestObject } from '../models/requestObject';

@Injectable({
  providedIn: 'root',
})
export class MonitoredItemService {
  baseUrl = environment.server + 'monitoredItem/';
  currentSession: SessionEntity;

  constructor(
    private http: HttpClient,
    private sessionAccessor: SessionAccessorService
  ) {
    this.sessionAccessor.currentSession$.subscribe({
      next: (value) => {
        this.currentSession = value;
      },
    });
  }

  addToSubscription(subscription: SubscriptionValue, nodeId: string) {
    const url = `${this.baseUrl}addToSubscription`;
    let request = <RequestObject>{
      sessionGuid: this.currentSession.guid,
      sessionNodeId: this.currentSession.sessionNodeId,
      subscriptionGuid: subscription.guid,
      opcUaId: subscription.opcUaId,
      nodeId: nodeId,
    };  
    return this.http.post(url, request).subscribe();
  }

  getMonitoredItem(subscriptionId: number, nodeId: string) {
    const queryParams = new URLSearchParams({
      nodeId: encodeURIComponent(nodeId),
      sessionNodeId: this.currentSession.sessionNodeId,
      subscriptionId: subscriptionId.toString(),
    }).toString();

    let url = `${this.baseUrl}get?${queryParams}`;
    return this.http.get(url);
  }

  deleteMonitoredItem(subscriptionId: number, nodeId: string) {
    const queryParams = new URLSearchParams({
      nodeId: encodeURIComponent(nodeId),
      sessionNodeId: this.currentSession.sessionNodeId,
      subscriptionId: subscriptionId.toString(),
    }).toString();

    let url = `${this.baseUrl}delete?${queryParams}`;
    return this.http.delete(url).subscribe();
  }

  updateMonitoredItem(
    subscriptionId: number,
    nodeId: string,
    updatedItem: MonitoredItemConfig
  ) {
    const queryParams = new URLSearchParams({
      sessionNodeId: this.currentSession.sessionNodeId,
      subscriptionId: subscriptionId.toString(),
    }).toString();

    let url = `${this.baseUrl}update?${queryParams}`;
    return this.http.put(url, updatedItem).subscribe();
  }
}
