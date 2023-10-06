import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/enviroments/enviroment';
import { NodeReference, TreeNode } from '../models/nodeModels';

@Injectable({
  providedIn: 'root',
})
export class TreeService {
  baseUrl = environment.server + 'tree/';
  constructor(private http: HttpClient) {}

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
