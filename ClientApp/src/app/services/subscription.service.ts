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
    private communicationService: CommunicationService,
    private sessionAccessor: SessionAccessorService,
    private notification: NotificationService
  ) {
    this.sessionAccessor.currentSession$.subscribe({
      next: (value) => {
        this.currentSession = value;
        if (value.state === SessionState.connected)
          this.getActiveSubscriptions();
      },
    });
  }

  get newSubscription$(): Observable<SubscriptionValue> {
    return this.communicationService.subscriptionObservable;
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
      next(value) {},
      error: (err) => {
        this.notification.showError(err.error, '');
        console.error(err);
      },
    });
  }

  deleteSubs(subscription: SubscriptionValue) {
    const queryParams = new URLSearchParams({
      subscriptionGuidId: subscription.subscriptionGuidId,
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();

    let url = `${this.baseUrl}delete/${subscription.opcUaId}?${queryParams}`;
    return this.http.delete(url);
  }

  getSubscriptionConfig(subscription: any) {
    const queryParams = new URLSearchParams({
      sessionNodeId: this.currentSession.sessionNodeId,
      subscriptionGuidId: subscription.subscriptionGuidId,
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
    return this.http.put(url, subscription).subscribe();
  }

  getActiveSubscriptions() {
    const queryParams = new URLSearchParams({
      sessionNodeId: this.currentSession.sessionNodeId,
    }).toString();
    
    let url = `${this.baseUrl}list?${queryParams}`;
    return this.http.get(url).subscribe();
  }
}
