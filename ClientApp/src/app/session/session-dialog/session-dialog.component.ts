import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SessionEntity } from 'src/app/models/sessionModels';
import { SessionService } from 'src/app/services/session.service';

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
    private sessionService: SessionService
  ) {
    if (sessionData.name) {
      this.addNew = false;
    }
  }

  addGroup() {
    this.sessionService.connectToSession(this.sessionData);
  }

  removeGroup() {
    this.sessionService.deleteSession(this.sessionData.name);
  }
}
