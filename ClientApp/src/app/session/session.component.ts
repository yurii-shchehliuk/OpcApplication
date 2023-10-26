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

  @ViewChild('sessionList') sessionArrElem: ElementRef;

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

  selectSession(channel: SessionEntity) {
    let childrens = this.sessionArrElem.nativeElement.children;
    for (let item of childrens) {
      let value = item.getElementsByClassName('channel-text')[0].innerText;

      if (value === channel.name) {
        item.classList.add('channel-selected');
        this.communicationService.joinNewGroup(channel.name);
        this.sessionService.connectToSession(channel);
      } else {
        item.classList.remove('channel-selected');
        this.communicationService.leaveGroup(value);
      }
    }
  }

  openDialog(
    event: any,
    sessionData: SessionEntity = (<Partial<SessionEntity>>{}) as SessionEntity
  ): void {
    const sessionDialog = this.dialog.open(SessionDialogComponent, {
      data: JSON.parse(JSON.stringify(sessionData)),
    });

    sessionDialog.afterClosed().subscribe({
      next: (value) => {
        this.sessionService.getSessionList();
      },
      complete: () => {
        this.sessionService.getSessionList();
      },
    });
    event.stopPropagation();
  }
}
