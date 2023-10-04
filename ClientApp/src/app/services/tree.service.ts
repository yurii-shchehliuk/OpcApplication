import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/enviroments/enviroment';

@Injectable({
  providedIn: 'root',
})
export class TreeService {
  baseUrl = environment.server + 'tree/';
  constructor(private http: HttpClient) {}

  getTreeChildrens(treeNode: any): Observable<any> {
    return this.http.patch(this.baseUrl + 'childrens', treeNode);
  }

  getTreeFull() {
    return this.http.get(this.baseUrl + 'full');
  }

  getTreeActive(nodesToFind: any) {
    return this.http.post(this.baseUrl + 'active', nodesToFind);
  }
}
