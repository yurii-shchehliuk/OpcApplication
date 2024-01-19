import { Injectable } from '@angular/core';
import { SubscriptionConfig } from '../models/subscriptionModels';
import { Subject, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SharedService {
  chartValueBoundaries$ = new BehaviorSubject<SubscriptionConfig>(
    (<Partial<SubscriptionConfig>>{}) as SubscriptionConfig
  );

  updateSessionList$ = new Subject<any>();

  //update chart by subscription dialog request
  updateChart(subscriptionConfig: SubscriptionConfig) {
    this.chartValueBoundaries$.next(subscriptionConfig);
  }

  //update session by session dialog request
  updateSessionList() {
    this.updateSessionList$.next(undefined);
  }
}
