import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription, tap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { SessionEntity, SessionState } from '../models/sessionModels';
import { NodeService } from './node.service';
import { NotificationService } from '../shared/notification.service';
import { SubscriptionService } from './subscription.service';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  baseUrl = environment.server + 'session/';
  private SESSION_KEY = 'sessionId_';

  private sessionListSubject = new BehaviorSubject<SessionEntity[]>([]);
  private selectedSession = new BehaviorSubject<SessionEntity>(
    (<Partial<SessionEntity>>{}) as SessionEntity
  );

  constructor(
    private http: HttpClient,
    private nodeService: NodeService,
    private notifyService: NotificationService,
    private subscriptionService: SubscriptionService
  ) {}

  get sessionList$(): Observable<SessionEntity[]> {
    return this.sessionListSubject.asObservable();
  }

  get selectedSession$(): Observable<SessionEntity> {
    return this.selectedSession.asObservable();
  }

  createEndpoint(session: SessionEntity) {
    return this.http
      .post<SessionEntity>(this.baseUrl + 'create', session)
      .subscribe({
        next: (response: SessionEntity) => {
          this.notifyService.showSuccess('Added successfully', '');
        },
        error: (err) => {
          this.notifyService.showError(err.message, 'Cannot add session');
          console.log(err);
        },
      });
  }

  connectToSession(session: SessionEntity): Subscription {
    return this.http
      .post<SessionEntity>(this.baseUrl + 'connect', session)
      .subscribe({
        next: (response: SessionEntity) => {
          this.selectedSession.next(response);
          sessionStorage.clear();
          sessionStorage.setItem(
            this.SESSION_KEY + response.name,
            response.sessionId
          );

          setTimeout(() => {
            this.subscriptionService.getActiveSubscriptions();
          }, 500);

          this.notifyService.showSuccess('Connected successfully', '');
        },
        error: (err) => {
          this.nodeService.getConfigNodes();
          session.state = SessionState.disconnected;
          this.selectedSession.next(session);
          console.log(err);
        },
        complete: () => {
          this.nodeService.getConfigNodes();
        },
      });
  }

  updateSession(session: SessionEntity): Subscription {
    return this.http
      .put<SessionEntity>(this.baseUrl + 'update', session)
      .subscribe({
        next: (response: SessionEntity) => {
          this.notifyService.showSuccess('Updated successfully', '');
        },
        error: (err) => {
          this.notifyService.showError('Session update falied', '');
          console.log(err);
        },
      });
  }

  disconnect(session: SessionEntity) {
    return this.http
      .post<SessionEntity>(this.baseUrl + 'disconnect', session)
      .subscribe();
  }

  deleteSession(sessionId: string): Subscription {
    return this.http.delete(this.baseUrl + sessionId).subscribe({
      next: () => {
        this.selectedSession.next(
          (<Partial<SessionEntity>>{}) as SessionEntity
        );
        sessionStorage.removeItem(this.SESSION_KEY);
      },
    });
  }

  getActiveSessions() {
    this.http
      .get<SessionEntity[]>(this.baseUrl + 'activeSessions')
      .subscribe((data: SessionEntity[]) => {
        this.sessionListSubject.next(data);
      });
  }

  getSessionList() {
    this.http
      .get<SessionEntity[]>(this.baseUrl + 'savedSessions')
      .subscribe((data: SessionEntity[]) => {
        this.sessionListSubject.next(data);
        this.getActiveSessions();
      });
  }
}
