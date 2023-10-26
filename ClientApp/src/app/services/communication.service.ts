import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment.development';
import { NotificationService } from '../shared/notification.service';
import { NodeValue } from '../models/nodeModels';
import { EventData, LogCategory } from '../models/eventModels';

@Injectable({
  providedIn: 'root',
})
export class CommunicationService {
  baseUrl = environment.server;

  private hubConnection: signalR.HubConnection;
  private nodeSubject: Subject<NodeValue> = new Subject<NodeValue>();

  get getNodeObservable(): Observable<NodeValue> {
    return this.nodeSubject.asObservable();
  }

  constructor(private notificationService: NotificationService) {}

  async signalReset() {
    if (this.hubConnection) await this.hubConnection.stop();
    console.log('SignalR disconnected.');
  }

  async signalrInit() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalrHub)
      .withAutomaticReconnect()
      .build();

    try {
      await this.hubConnection.start().then(() => {
        console.log('Connected to SignalR hub.');
        this.listenNodesSubscription();
        this.listenEventMessage();
      });

      return 'Connected';
    } catch (err) {
      console.error('Error while connecting to SignalR hub:', err);
      return undefined;
    }
  }

  private listenNodesSubscription(method: string = 'SendNodeAction') {
    this.hubConnection.on(method, (data: NodeValue) => {
      let result = data as NodeValue;
      result.storeTime = new Date(data.storeTime).toLocaleString();
      this.nodeSubject.next(result);
    });
  }

  private listenEventMessage(method: string = 'SendEventMessageAction') {
    this.hubConnection.on(method, (data: EventData) => {
      switch (data.logCategory) {
        case LogCategory.info:
          this.notificationService.showInfo(data.message, data.title);
          break;
        case LogCategory.warning:
          this.notificationService.showWarning(data.message, data.title);
          break;
        case LogCategory.error:
          this.notificationService.showError(data.message, data.title);
          break;
        case LogCategory.success:
          this.notificationService.showSuccess(data.message, data.title);
          break;
      }
    });
  }

  joinNewGroup(group: string): void {
    if (!this.hubConnection.connectionId) {
      this.signalrInit().then(() => {
        this.hubConnection.invoke('JoinGroup', group);
      });
    } else {
      this.hubConnection.invoke('JoinGroup', group);
    }
  }

  leaveGroup(group: string): void {
    this.hubConnection.invoke('LeaveGroup', group);
  }
}
