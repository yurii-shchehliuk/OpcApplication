import { Injectable } from '@angular/core';
import { SessionEntity, SessionState } from '../models/sessionModels';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { SessionService } from '../services/session.service';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root',
})
export class SessionAccessorService {
  constructor(
    private sessionService: SessionService,
    private notifyService: NotificationService
  ) {}

  get currentSession$(): Observable<SessionEntity> {
    return this.sessionService.selectedSession$.pipe(
      tap((session) => {
        if (
          session.state == undefined ||
          session.state !== SessionState.connected
        ) {
          this.notifyService.showWarning('Session is not connected', '');
        }
      })
    );
  }
}
