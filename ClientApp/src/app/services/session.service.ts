import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription, tap } from 'rxjs';
import { environment } from 'src/enviroments/enviroment';
import { SessionEntity } from '../models/sessionModels';
import { NodeService } from './node.service';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  baseUrl = environment.server + 'session/';
  private SESSION_KEY = 'sessionId';

  private selectedChannel = new BehaviorSubject<string>('');
  private sessionListSubject = new BehaviorSubject<SessionEntity[]>([]);

  constructor(private http: HttpClient, private nodeService: NodeService) {}

  private set setChannel(name: string) {
    this.selectedChannel.next(name);
  }

  get getChannel(): Observable<string> {
    return this.selectedChannel.asObservable();
  }

  get sessionList$(): Observable<SessionEntity[]> {
    return this.sessionListSubject.asObservable();
  }

  connectToSession(session: SessionEntity): Subscription {
    this.setChannel = session.name;
    return this.http
      .post<SessionEntity>(this.baseUrl + 'connect', session)
      .subscribe({
        next: (response: SessionEntity) => {
          this.selectedChannel.next(response.name);
          sessionStorage.setItem(this.SESSION_KEY, response.sessionId);
          this.nodeService.getConfigNodes();
        },
        error: (err) => {
          console.log(err);
        },
      });
  }

  deleteSession(sessionId: string): Subscription {
    return this.http.delete(this.baseUrl + sessionId).subscribe({
      next: () => {
        this.setChannel = '';
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

  getSession(sessionId: string): Observable<SessionEntity> {
    return this.http.get<SessionEntity>(this.baseUrl + sessionId);
  }

  renewSession(sessionId: string) {
    return this.http.get<SessionEntity>(this.baseUrl + 'renew/' + sessionId);
  }
}
