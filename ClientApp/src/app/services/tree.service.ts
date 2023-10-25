import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { NodeReference, TreeNode } from '../models/nodeModels';
import { NotificationService } from '../shared/notification.service';
import { SessionService } from './session.service';

@Injectable({
  providedIn: 'root',
})
export class TreeService {
  baseUrl = environment.server + 'tree/';
  constructor(
    private http: HttpClient,
    private notifyService: NotificationService,
    private session: SessionService
  ) {}

  
  getTreeChildrens(treeNode: TreeNode): Observable<TreeNode[]> {
    return this.http.patch<TreeNode[]>(this.baseUrl + 'childrens', treeNode);
  }

  getTreeFull() {
    return this.http.get(this.baseUrl + 'full');
  }

  getTreeActive(nodesToFind: NodeReference) {
    return this.http.post(this.baseUrl + 'active', nodesToFind);
  }
}
