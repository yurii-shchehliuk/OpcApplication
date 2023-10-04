import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { environment } from 'src/enviroments/enviroment';

@Injectable({
  providedIn: 'root',
})
export class NodeService {
  private sessionListSubject = new BehaviorSubject<any[]>([]);

  baseUrl = environment.server + 'node/';
  constructor(private http: HttpClient) {}

  get configNodes$() {
    return this.sessionListSubject.asObservable();
  }

  deleteConfigNode(nodeId: string): Subscription {
    return this.http.delete(this.baseUrl + nodeId).subscribe(() => {
      this.getConfigNodes();
    });
  }

  getConfigNodes() {
    this.http.get<any[]>(this.baseUrl + 'config/list').subscribe((res) => {
      this.sessionListSubject.next(res);
    });
  }
}
