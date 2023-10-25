import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, EventEmitter, Output } from '@angular/core';

import {
  DynamicDataSource,
  DynamicDatabase,
  DynamicFlatNode,
} from './dynamic-database.service';
import { SessionService } from 'src/app/services/session.service';

@Component({
  selector: 'app-node-tree',
  templateUrl: './node-tree.component.html',
  styleUrls: ['./node-tree.component.scss'],
})
export class NodeTreeComponent {
  treeControl: FlatTreeControl<DynamicFlatNode>;
  dataSource: DynamicDataSource;
  @Output() newNode: EventEmitter<any> = new EventEmitter();

  constructor(
    private database: DynamicDatabase,
    private session: SessionService
  ) {
    this.treeControl = new FlatTreeControl<DynamicFlatNode>(
      this.getLevel,
      this.isExpandable
    );
    this.dataSource = new DynamicDataSource(this.treeControl, database);

    this.session.getSession.subscribe((res) => {
      if (res !== '') {
        this.dataSource.data = database.initialData();
      }
    });
  }

  populateNode(node: any) {
    this.newNode.emit(node);
  }

  getLevel = (node: DynamicFlatNode) => node.level;

  isExpandable = (node: DynamicFlatNode) => node.expandable;

  hasChild = (_: number, _nodeData: DynamicFlatNode) => true;
}
