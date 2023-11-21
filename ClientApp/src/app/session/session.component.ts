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
  sessionArr: SessionEntity[] = [];
  panelOpenState = false;

  @ViewChild('sessionList') sessionArrElem: ElementRef;

  constructor(
    private sessionService: SessionService,
    private communicationService: CommunicationService,
    public dialog: MatDialog,
    private subscriptionService: SubscriptionService
  ) {
    this.communicationService.signalrInit();
  }

  ngOnInit(): void {
    //1 get all sessions
    this.sessionService.sessionList$.subscribe((res) => {
      res.map((session) => {
        const index = this.sessionArr.findIndex(
          (sessionItem) => sessionItem.name === session.name
        );
        if (index !== -1) {
          // if we have connected session with the same name, disconnect
          if (
            session.sessionId != null &&
            session.sessionId != this.sessionArr[index].sessionId
          ) {
            this.sessionService.disconnect(this.sessionArr[index]);
            this.sessionArr[index] = session;
          }
        } else {
          this.sessionArr.push(session);
        }
      });
    });

    //2 on connected replace session entity
    this.sessionService.selectedSession$.subscribe((res) => {
      if (res.sessionNodeId) {
        const index = this.sessionArr.findIndex(
          (session) => session.name === res.name
        );
        if (index !== -1) {
          this.sessionArr[index] = res;
        } else {
          this.sessionArr.push(res);
        }
        this.communicationService.joinNewGroup(res.sessionNodeId);
      }
    });

    this.sessionService.getSessionList();
  }

  selectSession(selectedSession: SessionEntity) {
    this.sessionArr.map((session) => {
      if (
        session.sessionId === selectedSession.sessionId &&
        session.name == selectedSession.name
      ) {
        this.sessionService.connectToSession(session);
      } else {
        this.communicationService.leaveGroup(session.sessionNodeId);
      }
    });

    let childrens = this.sessionArrElem.nativeElement.children;
    for (let item of childrens) {
      let value = item.getElementsByClassName('channel-text')[0].innerText;
      if (value === selectedSession.name) {
        item.classList.add('channel-selected');
      } else {
        item.classList.remove('channel-selected');
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
      next: () => {
        this.sessionArr = [];
        this.sessionService.getSessionList();
        this.sessionService.getActiveSessions();
      },
    });
    event.stopPropagation();
  }
}
