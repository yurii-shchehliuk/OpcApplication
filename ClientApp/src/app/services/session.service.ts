import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription, tap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { SessionEntity } from '../models/sessionModels';
import { NotificationService } from '../shared/notification.service';
import { SessionState } from '../models/enums';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  baseUrl = environment.server + 'session/';
  private SESSION_KEY = 'sessionId_';
  currentSession: SessionEntity;

  private sessionListSubject = new BehaviorSubject<SessionEntity[]>([]);
  private selectedSessionSubject = new BehaviorSubject<SessionEntity>(
    (<Partial<SessionEntity>>{}) as SessionEntity
  );

  constructor(
    private http: HttpClient,
    private notifyService: NotificationService
  ) {}

  get sessionList$(): Observable<SessionEntity[]> {
    return this.sessionListSubject.asObservable();
  }

  get selectedSession$(): Observable<SessionEntity> {
    return this.selectedSessionSubject.asObservable();
  }

  getSessionList() {
    let url = `${this.baseUrl}list`;
    this.http.get<SessionEntity[]>(url).subscribe((data: SessionEntity[]) => {
      this.sessionListSubject.next(data);
    });
  }

  createEndpoint(session: SessionEntity) {
    let url = `${this.baseUrl}create`;
    return this.http.post<SessionEntity>(url, session).subscribe({
      next: (response: SessionEntity) => {
        this.notifyService.showSuccess('Added successfully', '');
        //todo
        // this.sessionListSubject.next(...response);
      },
      error: (err) => {
        this.notifyService.showError(err.message, 'Cannot add session');
        console.log(err);
      },
    });
  }

  connectToSession(session: SessionEntity): Subscription {
    let url = `${this.baseUrl}connect`;

    return this.http.post<SessionEntity>(url, session).subscribe({
      next: (response: SessionEntity) => {
        this.selectedSessionSubject.next(response);
        this.currentSession = response;
        sessionStorage.clear();
        sessionStorage.setItem(
          this.SESSION_KEY + response.name,
          response.sessionNodeId
        );
        this.notifyService.showSuccess('Connected successfully', '');
      },
      error: (err) => {
        session.state = SessionState.disconnected;
        this.selectedSessionSubject.next(session);
        console.error(err);
      },
    });
  }

  disconnect(session: SessionEntity) {
    let url = `${this.baseUrl}disconnect?sessionNodeId=${session.sessionNodeId ?? ''}`;
    return this.http.delete<SessionEntity>(url);
  }

  updateSession(session: SessionEntity): Subscription {
    let url = `${this.baseUrl}update`;

    return this.http.put<SessionEntity>(url, session).subscribe({
      next: (response: SessionEntity) => {
        this.notifyService.showSuccess('Updated successfully', '');
      },
      error: (err) => {
        this.notifyService.showError('Session update falied', '');
        console.log(err);
      },
    });
  }

  deleteSession(session: SessionEntity): Subscription {
    let url = `${this.baseUrl}delete?guid=${
      session.guid ?? ''
    }`;
    return this.http.delete(url).subscribe({
      next: () => {
        // this.selectedSession.next(
        //   (<Partial<SessionEntity>>{}) as SessionEntity
        // );
        sessionStorage.removeItem(this.SESSION_KEY + session.name);
      },
    });
  }
}
