import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { TreeNode } from '../models/treeNodeModel';
import { NotificationService } from '../shared/notification.service';
import { MonitoredItemConfig } from '../models/monitoredItem';
import { SessionAccessorService } from '../shared/session-accessor.service';
import { SessionEntity } from '../models/sessionModels';

@Injectable({
  providedIn: 'root',
})
export class TreeService {
  baseUrl = environment.server + 'tree/';
  currentSession: SessionEntity;

  constructor(
    private http: HttpClient,
    private sessionAccessor: SessionAccessorService
  ) {
    this.sessionAccessor.currentSession$.subscribe({
      next: (value) => {
        this.currentSession = value;
      },
    });
  }

  getTreeChildrens(treeNode: TreeNode): Observable<TreeNode[]> {
    let url = `${this.baseUrl}childrens?sessionNodeId=${this.currentSession.sessionNodeId}`;
    //todo: not array
    return this.http.patch<TreeNode[]>(url, treeNode);
  }

  downloadFullTree() {
    let url = `${this.baseUrl}full?sessionNodeId=${this.currentSession.sessionNodeId}`;
    return this.http.get(url, {
      responseType: 'blob',
      reportProgress: true,
    });
  }
}
