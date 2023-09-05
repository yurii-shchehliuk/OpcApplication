import { Injectable, NgZone, OnInit, signal } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/enviroments/enviroment';
import * as signalR from '@microsoft/signalr';
import { NewNode, NodeData } from './models/models';

@Injectable({
  providedIn: 'root',
})
export class AppService {
  baseUrl = environment.server;
  signalrHub = environment.signalrHub;
  private selectedChannel = new BehaviorSubject<string>('All');
  private graphDataSubject = new Subject<any>();

  private hubConnection: signalR.HubConnection;
  private messageSubject: Subject<NodeData> = new Subject<NodeData>();

  getChannel(): Observable<string> {
    return this.selectedChannel.asObservable();
  }

  getMessageObservable(): Observable<NodeData> {
    return this.messageSubject.asObservable();
  }

  getGraphData(): Observable<any> {
    return this.graphDataSubject.asObservable();
  }

  joinPublic() {
    this.signalrInit(environment.signalrHub).then(() => {
      this.listenMethod('MessageReceived');
      this.askForGraph();
      this.selectedChannel.next('All');
    });
  }

  joinNewGroup(group: string) {
    this.signalrInit(environment.signalrHub).then(() => {
      this.hubConnection.invoke('JoinGroup', group);
      this.listenMethod('MessageReceivedGroup');
      this.askForGraph();
      this.selectedChannel.next(group);
    });
  }

  nodeMonitor(node: NewNode) {
    this.hubConnection.invoke('NodeMonitorAction', node);
  }

  async signalReset() {
    if (this.hubConnection) await this.hubConnection.stop();
  }

  private askForGraph(group: string = '') {
    this.hubConnection.on(
      'LoadGraph',
      (groupName: any, graphName: any, graphTree: any) => {
        let data = JSON.parse(graphTree);
        this.graphDataSubject.next(data);
      }
    );

    this.loadGraphAction(group, 'graph-new.json');
    this.loadGraphAction(group, 'graph-full.json');
  }

  loadGraphAction(group: string, graphName: string) {
    this.hubConnection.invoke('GetGraphAction', group, graphName);
  }

  private listenMethod(method: string) {
    this.hubConnection.on(method, (data: any) => {
      let entity = JSON.parse(data);
      let result = entity as NodeData;
      result.StoreTime = new Date(entity.StoreTime).toLocaleString();
      this.messageSubject.next(result);
    });
  }

  private async signalrInit(hubUrl: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .build();

    try {
      await this.hubConnection.start();
      console.log('Connected to SignalR hub.');
      return 'Connected';
    } catch (err) {
      console.error('Error while connecting to SignalR hub:', err);
      return undefined;
    }
  }
}
