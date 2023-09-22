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
  private selectedChannel = new BehaviorSubject<string>('');
  private graphDataSubject = new Subject<any>();
  private appSettingsSubject = new Subject<any>();
  private channelsSubject = new Subject<any>();

  private hubConnection: signalR.HubConnection;
  private nodeSubject: Subject<NodeData> = new Subject<NodeData>();
  private nodeConfigurations: Subject<any[]> = new Subject<any[]>();

  get getChannel(): Observable<string> {
    return this.selectedChannel.asObservable();
  }

  get getNodeObservable(): Observable<NodeData> {
    return this.nodeSubject.asObservable();
  }

  get getNodeConfigurationsObservable(): Observable<any[]> {
    return this.nodeConfigurations.asObservable();
  }

  get getGraphData(): Observable<any> {
    return this.graphDataSubject.asObservable();
  }

  get getAppSettings(): Observable<any> {
    return this.appSettingsSubject.asObservable();
  }

  get getChannels() {
    return this.channelsSubject.asObservable();
  }

  private set setChannel(name: string) {
    this.selectedChannel.next(name);
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
        this.listenGroups();
      });

      return 'Connected';
    } catch (err) {
      console.error('Error while connecting to SignalR hub:', err);
      return undefined;
    }
  }

  //#region actions
  joinNewGroup(group: string) {
    this.signalrInit().then(() => {
      this.setChannel = group;
      this.hubConnection.invoke('JoinGroup', group);
      //listeners
      this.listenNodesSubscription();
      this.listenLoadGraph();
      this.listenConfigNodes();
      //listener-actions
      this.listenGetSettings();
      this.getNodesWeb();
    });
  }

  getGraphWeb(isFull: boolean) {
    this.hubConnection.invoke(
      'GetGraphWeb',
      this.selectedChannel.value,
      isFull
    );
  }

  getSettingsWeb() {
    this.hubConnection.invoke('GetSettingsWeb', this.selectedChannel.value);
  }

  getChannelsWeb() {
    this.hubConnection.invoke('GetGroupsWeb');
  }

  nodeMonitorWeb(node: NewNode) {
    this.hubConnection.invoke('NodeMonitorWeb', node);
    this.getNodesWeb();
  }

  getNodesWeb() {
    this.hubConnection.invoke('GetNodesWeb', this.selectedChannel.value);
  }

  saveSettings(appSettings: any) {
    if (!this.selectedChannel.value) return;
    this.hubConnection.invoke(
      'SaveSettingsWeb',
      this.selectedChannel.value,
      appSettings
    );
  }

  addChannelWeb(name: string) {
    this.hubConnection.invoke('AddGroupWeb', name);
  }

  removeChannelWeb(name: string) {
    this.hubConnection.invoke('RemoveGroupWeb', name);
    this.setChannel = '';
  }
  //#endregion

  //#region listeners
  listenGroups(method: string = 'GroupsReceived') {
    this.hubConnection.on(method, (channels: any) => {
      let data = JSON.parse(channels);
      this.channelsSubject.next(data);
    });

    this.getChannelsWeb();
  }

  private listenNodesSubscription(method: string = 'NodeReceived') {
    this.hubConnection.on(method, (data: any) => {
      let result = data as NodeData;
      result.storeTime = new Date(data.storeTime).toLocaleString();
      this.nodeSubject.next(result);
    });
  }

  private listenConfigNodes(method: string = 'ConfigNodesReceived') {
    this.hubConnection.on(method, (data: any) => {
      console.log(data);
      let nodeConfigs = JSON.parse(data);
      this.nodeConfigurations.next(nodeConfigs);
    });
  }

  private listenLoadGraph(method: string = 'GraphReceived') {
    this.hubConnection.on(method, (groupName: any, graphTree: any) => {
      let data = JSON.parse(graphTree);
      this.graphDataSubject.next(data);
    });
  }

  private listenGetSettings(method: string = 'SettingsReceived') {
    this.hubConnection.on(method, (settings: any) => {
      let data = JSON.parse(settings);
      if (!data) {
        data = settings;
      }
      this.appSettingsSubject.next(data);
    });

    this.getSettingsWeb();
    // this.getGraphWeb(this.appSettingsSubject.createFullTree);
  }

  //#endregion
}
