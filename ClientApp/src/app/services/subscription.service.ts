import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { NodeReference } from '../models/nodeModels';

@Injectable({
  providedIn: 'root',
})
export class SubscriptionService {
  baseUrl = environment.server + 'subscription/';
  constructor(private http: HttpClient) {}

  createSubs(subscription: NodeReference): Observable<NodeReference> {
    return this.http.post<NodeReference>(this.baseUrl + 'create', subscription);
  }

  deleteSubs(subscriptionId: string, nodeId: string) {
    return this.http.delete(this.baseUrl + subscriptionId + '/' + nodeId);
  }

  getSubsList() {
    return this.http.get(this.baseUrl + 'list');
  }
}
