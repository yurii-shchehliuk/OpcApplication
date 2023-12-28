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
export class NodeService {
  private configNodesSubj = new BehaviorSubject<MonitoredItemValue[]>([]);
  private nodeToRemoveSubj = new Subject<string>();

  baseUrl = environment.server + 'node/';
  constructor(
    private http: HttpClient,
    private communicationService: CommunicationService
  ) {}

  get configNodes$() {
    return this.configNodesSubj.asObservable();
  }

  get getNodeValueObservable$(): Observable<MonitoredItemValue> {
    return this.communicationService.getNodeObservable;
  }

  get nodeToRemove$(): Observable<string> {
    return this.nodeToRemoveSubj.asObservable();
  }

  set nodeToRemove$(node: string) {
    this.nodeToRemoveSubj.next(node);
  }

  addConfigNode(node: MonitoredItemConfig) {
    return this.http.post(this.baseUrl + 'create', node).subscribe(() => {
      this.getConfigNodes();
    });
  }

  deleteConfigNode(nodeId: string): Subscription {
    return this.http.delete(this.baseUrl + nodeId).subscribe(() => {
      this.getConfigNodes();
    });
  }

  getConfigNodes() {
    this.http.get<MonitoredItemValue[]>(this.baseUrl + 'config/list').subscribe({
      next: (res) => {
        this.configNodesSubj.next(res);
      },
      error: (err) => {
        this.configNodesSubj.next([]);
      },
    });
  }
}
