import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { environment } from 'src/enviroments/enviroment';

@Injectable({
  providedIn: 'root',
})
export class SubscriptionService {
  baseUrl = environment.server + 'subscription/';
  constructor(private http: HttpClient) {}

  createSubs(subscription: any): Subscription {
    return this.http.post(this.baseUrl + 'create', subscription).subscribe();
  }

  deleteSubs(subscriptionId: string): Observable<any> {
    return this.http.delete(this.baseUrl + subscriptionId);
  }

  getSubsList() {
    return this.http.get(this.baseUrl + 'list');
  }
}
