import { Component, ElementRef, ViewChild } from '@angular/core';
import { CommunicationService } from '../services/communication.service';
import { SessionService } from '../services/session.service';
import { MatDialog } from '@angular/material/dialog';
import { SessionDialogComponent } from './session-dialog/session-dialog.component';
import { SessionEntity } from '../models/sessionModels';
import { SessionState } from '../models/enums';

@Component({
  selector: 'app-session',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.scss'],
})
export class SessionComponent {
  sessionState = SessionState;
  sessionArr: SessionEntity[] = [];
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
    //1 get all sessions
    this.sessionService.sessionList$.subscribe((res) => {
      res.map((session) => {
        session.isSelected = false;
        const index = this.sessionArr.findIndex(
          (sessionItem) => sessionItem.sessionGuidId === session.sessionGuidId
        );
        if (index !== -1) {
          this.sessionArr[index] = session;
        } else {
          this.sessionArr.push(session);
        }
      });
    });

    //2 on connected replace session entity
    this.sessionService.selectedSession$.subscribe((res) => {
      // update session array with connected session data
      if (!res.sessionNodeId) return;
      const index = this.sessionArr.findIndex(
        (session) => session.name === res.name
      );
      res.isSelected = true;
      if (index !== -1) {
        this.sessionArr[index] = res;
      } else {
        this.sessionArr.push(res);
      }
      // flat for css
      this.sessionArr.map((session) => {
        if (session.sessionGuidId != res.sessionGuidId) {
          session.isSelected = false;
        }
      });

      this.communicationService.joinNewGroup(res.sessionNodeId);
    });

    this.sessionService.getSessionList();
  }

  selectSession(selectedSession: SessionEntity) {
    this.sessionArr.map((session) => {
      if (session.sessionGuidId === selectedSession.sessionGuidId) {
        this.sessionService.connectToSession(session);
      } else {
        this.communicationService.leaveGroup(session.sessionNodeId);
      }
    });
  }

  disconnectSession(event: any, session: SessionEntity) {
    this.sessionService.disconnect(session).subscribe({
      next: () => {
        const index = this.sessionArr.findIndex(
          (sessionItem) => sessionItem.sessionGuidId === session.sessionGuidId
        );
        if (index !== -1) {
          this.sessionArr[index].state = SessionState.disconnected;
          this.sessionArr[index].sessionNodeId = '';
        }
      },
    });
    event.stopPropagation();
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
      },
    });
    event.stopPropagation();
  }
}
