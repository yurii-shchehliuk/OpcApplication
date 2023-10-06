import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { environment } from 'src/enviroments/enviroment';
import { NodeReference } from '../models/nodeModels';

@Injectable({
  providedIn: 'root',
})
export class NodeService {
  private configNodesSubj = new BehaviorSubject<NodeReference[]>([]);

  baseUrl = environment.server + 'node/';
  constructor(private http: HttpClient) {}

  get configNodes$() {
    return this.configNodesSubj.asObservable();
  }

  addConfigNode(node: NodeReference) {
    return this.http.post(this.baseUrl + 'create', node).subscribe(() => {
      this.getConfigNodes();
    });
  }

  deleteConfigNode(nodeId: string): Subscription {
    return this.http.delete(this.baseUrl + nodeId).subscribe(() => {
      this.getConfigNodes();
    });
  }

  getConfigNodes() {
    this.http.get<NodeReference[]>(this.baseUrl + 'config/list').subscribe((res) => {
      this.configNodesSubj.next(res);
    });
  }
}
