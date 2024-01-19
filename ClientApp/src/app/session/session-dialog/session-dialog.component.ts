import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SessionEntity } from 'src/app/models/sessionModels';
import { CommunicationService } from 'src/app/services/communication.service';
import { SessionService } from 'src/app/services/session.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-session-dialog',
  templateUrl: './session-dialog.component.html',
  styleUrls: ['./session-dialog.component.scss'],
})
export class SessionDialogComponent {
  addNew: boolean = true;
  constructor(
    public dialogRef: MatDialogRef<SessionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public sessionData: SessionEntity,
    private sessionService: SessionService,
    private communicationService: CommunicationService,
    private sharedService: SharedService
  ) {
    if (sessionData.name) {
      this.addNew = false;
    }
  }

  addSession() {
    if (this.addNew) {
      this.sessionData.guid = '';
      this.sessionService.createEndpoint(this.sessionData);
    } else {
      this.sessionService.updateSession(this.sessionData);
    }

    if (this.sessionData.name != undefined) {
      this.updateSessionList();
    }
  }

  deleteSession() {
    this.communicationService.leaveGroup(this.sessionData.sessionNodeId);
    this.sessionService.deleteSession(this.sessionData);
    this.updateSessionList();
  }

  private updateSessionList() {
    this.sharedService.updateSessionList();
  }
}
