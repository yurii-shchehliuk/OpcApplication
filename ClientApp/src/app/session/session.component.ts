import { Component, ElementRef, ViewChild } from '@angular/core';
import { Chart, ChartDataset, ChartConfiguration } from 'chart.js';
import { Subscription } from 'rxjs';
import { NodeValue } from '../models/nodeModels';
import { CommunicationService } from '../services/communication.service';
import { NodeService } from '../services/node.service';
import { SessionService } from '../services/session.service';
import { SubscriptionService } from '../services/subscription.service';
import { MatDialog } from '@angular/material/dialog';
import { SessionDialogComponent } from './session-dialog/session-dialog.component';
import { SessionEntity } from '../models/sessionModels';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.scss'],
})
export class SessionComponent {
  sessionArr: SessionEntity[];
  panelOpenState = false;

  @ViewChild('channelList') channelArrElem: ElementRef;

  constructor(
    private sessionService: SessionService,
    private communicationService: CommunicationService,
    public dialog: MatDialog
  ) {
    this.communicationService.signalrInit();
  }

  ngOnInit(): void {
    this.sessionService.sessionList$.subscribe((res) => {
      this.sessionArr = res;
    });
    this.sessionService.getSessionList();
  }

  selectChannel(channel: SessionEntity) {
    let childrens = this.channelArrElem.nativeElement.children;
    for (let item of childrens) {
      item.classList.remove('channel-selected');
      let value = item.getElementsByClassName('channel-text')[0].innerText;
      this.communicationService.leaveGroup(channel.name);

      if (value === channel.name) {
        item.classList.add('channel-selected');
        this.communicationService.joinNewGroup(channel.name);
        this.sessionService.connectToSession(channel);
      }
    }
  }

  openDialog(
    event: any,
    sessionData: SessionEntity = (<Partial<SessionEntity>>{}) as SessionEntity
  ): void {
    const sessionDialog = this.dialog.open(SessionDialogComponent, {
      data: sessionData,
    });

    event.stopPropagation();

    sessionDialog.afterClosed().subscribe(() => {
      this.sessionService.getSessionList();
    });
  }
}
