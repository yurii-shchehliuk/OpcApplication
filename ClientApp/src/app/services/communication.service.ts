import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment.development';
import { NotificationService } from '../shared/notification.service';
import { MonitoredItemValue } from '../models/monitoredItem';
import { SubscriptionValue } from '../models/subscriptionModels';
import { NotificationData } from '../models/notificationModel';

@Injectable({
  providedIn: 'root',
})
export class CommunicationService {
  private hubConnection: signalR.HubConnection;
  private nodeSubject = new Subject<MonitoredItemValue>();
  private subscriptionSubject = new Subject<SubscriptionValue>();

  get getNodeObservable(): Observable<MonitoredItemValue> {
    return this.nodeSubject.asObservable();
  }

  get subscriptionObservable(): Observable<SubscriptionValue> {
    return this.subscriptionSubject.asObservable();
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
        this.listenSubscription();
        this.listenNotification();
      });

      return 'Connected';
    } catch (err) {
      let message = 'Error while connecting to SignalR hub';
      this.notificationService.showError(message, '');
      console.error(message, err);
      return undefined;
    }
  }

  // joining the group when session is selected and connected
  joinNewGroup(group: string): void {
    if(!group) return;
    
    if (!this.hubConnection.connectionId) {
      this.signalrInit().then(() => {
        this.hubConnection.invoke('JoinGroup', group);
      });
    } else {
      this.hubConnection.invoke('JoinGroup', group);
    }
  }

  leaveGroup(group: string): void {
    if (group) {
      this.hubConnection.invoke('LeaveGroup', group);
    }
  }

  private listenNodesSubscription(method: string = 'SendNodeAction') {
    this.hubConnection.on(method, (data: MonitoredItemValue) => {
      let result = data as MonitoredItemValue;
      result.createdAt = new Date(data.createdAt).toLocaleString();
      this.nodeSubject.next(result);
    });
  }

  private listenSubscription(method: string = 'SendSubscriptionAction') {
    // get subscription with monitored items
    this.hubConnection.on(method, (data: SubscriptionValue) => {
      this.subscriptionSubject.next(data);
    });
  }

  private listenNotification(method: string = 'SendNotificationAction') {
    this.hubConnection.on(method, (data: NotificationData) => {
      this.notificationService.show(data);
    });
  }
}
