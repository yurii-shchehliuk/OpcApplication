import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { CommunicationService } from './communication.service';
import { SessionAccessorService } from '../shared/session-accessor.service';
import { SessionEntity } from '../models/sessionModels';
import {
  SubscriptionConfig,
  SubscriptionValue,
} from '../models/subscriptionModels';
import { MonitoredItemConfig } from '../models/monitoredItem';
import { NotificationService } from '../shared/notification.service';
import { SessionState } from '../models/enums';
import { SharedService } from '../shared/shared.service';

@Injectable({
  providedIn: 'root',
})
/**
 * in this service method subscribe is empty because everything is based on events of the OPC UA library
 */
export class SubscriptionService {
  baseUrl = environment.server + 'subscription/';
  currentSession: SessionEntity;
  private subscriptionToRemoveSubj = new Subject<string>();

  constructor(
    private http: HttpClient,
    private sessionAccessor: SessionAccessorService,
    private notification: NotificationService
  ) {
    this.sessionAccessor.currentSession$.subscribe({
      next: (value) => {
        this.currentSession = value;
      },
    });
  }

  get subscriptionToRemove$(): Observable<string> {
    return this.subscriptionToRemoveSubj.asObservable();
  }

  set subscriptionToRemove$(node: string) {
    this.subscriptionToRemoveSubj.next(node);
  }

  createSubs(subsParams: SubscriptionConfig, nodeId: string) {
    const queryParams = new URLSearchParams({
      nodeId: encodeURIComponent(nodeId),
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();

    let url = `${this.baseUrl}create?${queryParams}`;
    return this.http.post<MonitoredItemConfig>(url, subsParams).subscribe({
      next: (value) => {},
      error: (err) => {
        this.notification.showError(err.error, '');
        console.error(err);
      },
    });
  }

  getActiveSubscriptions() {
    const queryParams = new URLSearchParams({
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();

    let url = `${this.baseUrl}list?${queryParams}`;
    return this.http.get<SubscriptionValue[]>(url);
  }

  //todo:
  getSubscriptionConfig(subscription: any) {
    const queryParams = new URLSearchParams({
      sessionNodeId: this.currentSession.sessionNodeId,
      subscriptionGuid: subscription.guid,
    }).toString();

    let url = `${this.baseUrl}${subscription.opcUaId}?${queryParams}`;
    return this.http.get<SubscriptionConfig>(url);
  }

  modifySubs(subscriptionParams: SubscriptionConfig) {
    const queryParams = new URLSearchParams({
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();

    let url = `${this.baseUrl}modify?${queryParams}`;
    return this.http.put(url, subscriptionParams).subscribe();
  }

  setPublishingMode(subscription: SubscriptionValue) {
    const queryParams = new URLSearchParams({
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();

    let url = `${this.baseUrl}setPublishingMode/${subscription.opcUaId}?${queryParams}`;
    return this.http.put<SubscriptionValue>(url, subscription);
  }

  stopAllSubscriptions() {
    const queryParams = new URLSearchParams({
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();

    let url = `${this.baseUrl}stopAll?${queryParams}`;
    return this.http.put(url, null);
  }

  deleteSubs(subscription: SubscriptionValue) {
    const queryParams = new URLSearchParams({
      subscriptionGuid: subscription.guid,
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();

    let url = `${this.baseUrl}delete/${subscription.opcUaId}?${queryParams}`;
    return this.http.delete(url);
  }
}
