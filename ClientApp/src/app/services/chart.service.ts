import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, Subscription } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { NotificationService } from '../shared/notification.service';
import { SessionAccessorService } from '../shared/session-accessor.service';
import { CommunicationService } from './communication.service';
import { MonitoredItemConfig, MonitoredItemValue } from '../models/monitoredItem';

@Injectable({
  providedIn: 'root',
})
export class ChartService {
  private nodeToRemoveSubj = new Subject<string>();

  baseUrl = environment.server + 'node/';
  constructor(
    private communicationService: CommunicationService
  ) {}

  get getNodeValueObservable$(): Observable<MonitoredItemValue> {
    return this.communicationService.getNodeObservable;
  }

  get nodeToRemove$(): Observable<string> {
    return this.nodeToRemoveSubj.asObservable();
  }

  set nodeToRemove$(node: string) {
    this.nodeToRemoveSubj.next(node);
  }
}
