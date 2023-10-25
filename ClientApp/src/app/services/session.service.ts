import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription, tap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { SessionEntity } from '../models/sessionModels';
import { NodeService } from './node.service';
import { NotificationService } from '../shared/notification.service';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  baseUrl = environment.server + 'session/';
  private SESSION_KEY = 'sessionId_';

  private selectedSession = new BehaviorSubject<string>('');
  private sessionListSubject = new BehaviorSubject<SessionEntity[]>([]);

  constructor(
    private http: HttpClient,
    private nodeService: NodeService,
    private notifyService: NotificationService
  ) {}

  private set setSession(name: string) {
    this.selectedSession.next(name);
  }

  get getSession(): Observable<string> {
    return this.selectedSession.asObservable();
  }

  get sessionList$(): Observable<SessionEntity[]> {
    return this.sessionListSubject.asObservable();
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
          this.selectedSession.next(response.name);
          sessionStorage.setItem(
            this.SESSION_KEY + response.name,
            response.sessionId
          );
          this.setSession = session.name;
          this.nodeService.getConfigNodes();
          this.notifyService.showSuccess('Connected successfully', '');
        },
        error: (err) => {
          this.notifyService.showError(
            'Connection to the provided url failed',
            ''
          );
          console.log(err);
        },
      });
  }

  deleteSession(sessionId: string): Subscription {
    return this.http.delete(this.baseUrl + sessionId).subscribe({
      next: () => {
        this.setSession = '';
        sessionStorage.removeItem(this.SESSION_KEY);
      },
    });
  }

  getSessionList() {
    this.http
      .get<SessionEntity[]>(this.baseUrl + 'list')
      .subscribe((data: SessionEntity[]) => {
        this.sessionListSubject.next(data);
      });
  }
}
