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
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MonitoredItemService {
  baseUrl = environment.server + 'monitoredItem/';
  currentSession: SessionEntity;

  private nodeToRemoveSubj = new Subject<string>();

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

  get nodeToRemove$(): Observable<string> {
    return this.nodeToRemoveSubj.asObservable();
  }

  set nodeToRemove$(node: string) {
    this.nodeToRemoveSubj.next(node);
  }

  addToSubscription(subscription: SubscriptionValue, nodeId: string) {
    const queryParams = new URLSearchParams({
      subscriptionGuid: subscription.guid,
      subscriptionId: subscription.opcUaId.toString(),
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();

    let url = `${this.baseUrl}addToSubscription?${queryParams}`;

    let request = nodeId;

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

  deleteMonitoredItem(
    subscriptionId: number,
    subscriptionGuid: string,
    nodeId: string
  ) {
    const queryParams = new URLSearchParams({
      nodeId: encodeURIComponent(nodeId),
      sessionNodeId: this.currentSession.sessionNodeId,
      subscriptionId: subscriptionId.toString(),
      subscriptionGuid: subscriptionGuid,
    }).toString();

    let url = `${this.baseUrl}delete?${queryParams}`;
    return this.http.delete(url).subscribe();
  }
}
