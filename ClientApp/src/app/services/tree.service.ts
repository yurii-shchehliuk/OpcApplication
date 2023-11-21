import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { NodeReference, TreeNode } from '../models/nodeModels';
import { NotificationService } from '../shared/notification.service';

@Injectable({
  providedIn: 'root',
})
export class TreeService {
  baseUrl = environment.server + 'tree/';
  constructor(private http: HttpClient) {}

  getTreeChildrens(treeNode: TreeNode): Observable<TreeNode[]> {
    return this.http.patch<TreeNode[]>(this.baseUrl + 'childrens', treeNode);
  }

  downloadFullTree() {
    return this.http.get(this.baseUrl + 'full', {
      responseType: 'blob',
      reportProgress: true,
    });
  }

  getTreeActive(nodesToFind: NodeReference) {
    return this.http.post(this.baseUrl + 'active', nodesToFind);
  }
}
