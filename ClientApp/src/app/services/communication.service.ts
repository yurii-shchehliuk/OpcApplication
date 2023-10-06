import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/enviroments/enviroment';
import { NodeValue } from '../models/nodeModels';
import * as signalR from '@microsoft/signalr';

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

  async signalReset() {
    if (this.hubConnection) await this.hubConnection.stop();
    console.log('SignalR disconnected.');
  }

  async signalrInit() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalrHub)
      .build();

    try {
      await this.hubConnection.start().then(() => {
        console.log('Connected to SignalR hub.');
        this.listenNodesSubscription();
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

  joinNewGroup(group: string): void {
    this.hubConnection.invoke('JoinGroup', group);
  }

  leaveGroup(group: string): void {
    this.hubConnection.invoke('LeaveGroup', group);
  }
}
