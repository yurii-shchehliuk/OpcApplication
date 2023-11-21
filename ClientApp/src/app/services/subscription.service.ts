import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, Subscription } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { NodeReference } from '../models/nodeModels';
import {
  SubscriptionEntity,
  SubscriptionParameters,
} from '../models/subscriptionModels';
import { CommunicationService } from './communication.service';

@Injectable({
  providedIn: 'root',
})
export class SubscriptionService {
  baseUrl = environment.server + 'subscription/';
  private subscriptionToRemoveSubj = new Subject<string>();

  constructor(
    private http: HttpClient,
    private communicationService: CommunicationService
  ) {}

  get newSubscription$(): Observable<SubscriptionEntity> {
    return this.communicationService.subscriptionObservable;
  }

  get subscriptionToRemove$(): Observable<string> {
    return this.subscriptionToRemoveSubj.asObservable();
  }

  set subscriptionToRemove$(node: string) {
    this.subscriptionToRemoveSubj.next(node);
  }

  modifySubs(
    subscriptionParams: SubscriptionParameters,
    subscriptionId: number
  ) {
    return this.http
      .put(this.baseUrl + 'modify/' + subscriptionId, subscriptionParams)
      .subscribe();
  }

  getActiveSubscriptions() {
    return this.http.get(this.baseUrl + 'list/').subscribe();
  }

  createSubs(subscription: SubscriptionParameters, nodeId: string) {
    return this.http
      .post<NodeReference>(this.baseUrl + 'create/' + nodeId, subscription)
      .subscribe();
  }

  addToSubscription(subscriptionName: string, nodeId: string) {
    return this.http
      .put(
        this.baseUrl + 'addToSubscription/' + subscriptionName + '/' + nodeId,
        null
      )
      .subscribe();
  }

  setPublishingMode(subscriptionId: any, enable: boolean) {
    return this.http
      .put(
        this.baseUrl + 'setPublishingMode/' + subscriptionId + '/' + enable,
        null
      )
      .subscribe();
  }

  deleteSubs(subscriptionId: string) {
    return this.http.delete(this.baseUrl + subscriptionId).subscribe();
  }

  deleteMonitoringItem(subscriptionId: number, nodeId: string) {
    return this.http
      .delete(
        this.baseUrl + 'deleteMonitoringItem/' + subscriptionId + '/' + nodeId
      )
      .subscribe();
  }
}
