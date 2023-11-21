import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import {
  DynamicDataSource,
  DynamicDatabase,
  DynamicFlatNode,
} from './dynamic-database.service';
import { SessionEntity, SessionState } from 'src/app/models/sessionModels';
import { SessionAccessorService } from 'src/app/shared/session-accessor.service';
import { SubscriptionParametersDialogComponent } from 'src/app/subscription/subscription-parameters-dialog/subscription-parameters-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { TreeService } from 'src/app/services/tree.service';
import { SubscriptionEntity } from 'src/app/models/subscriptionModels';
import { SubscriptionService } from 'src/app/services/subscription.service';
import { NodeReference } from 'src/app/models/nodeModels';

@Component({
  selector: 'app-node-tree',
  templateUrl: './node-tree.component.html',
  styleUrls: ['./node-tree.component.scss'],
})
export class NodeTreeComponent implements OnInit {
  treeControl: FlatTreeControl<DynamicFlatNode>;
  dataSource: DynamicDataSource;
  sessionSource: SessionEntity;
  sessionState = SessionState;
  subscriptionsArr: SubscriptionEntity[] = [];

  constructor(
    private database: DynamicDatabase,
    private sessionAccessor: SessionAccessorService,
    private treeService: TreeService,
    public dialog: MatDialog,
    private subscriptionService: SubscriptionService
  ) {
    this.treeControl = new FlatTreeControl<DynamicFlatNode>(
      this.getLevel,
      this.isExpandable
    );
    this.dataSource = new DynamicDataSource(this.treeControl, database);

    this.sessionAccessor.currentSession$.subscribe((res) => {
      this.sessionSource = res;
      if (this.sessionSource.state === SessionState.connected) {
        this.dataSource.data = database.initialData();
        this.subscriptionsArr = [];
      }
    });
  }

  ngOnInit(): void {
    this.subscriptionService.newSubscription$.subscribe({
      next: (value: SubscriptionEntity) => {
        const index = this.subscriptionsArr.findIndex(
          (sub) => sub.id === value.id
        );

        if (index !== -1) {
          this.subscriptionsArr[index] = value;
        } else {
          this.subscriptionsArr.push(value);
        }
      },
    });

    this.subscriptionService.subscriptionToRemove$.subscribe({
      next: (subscriptionId: string) => {
        const index = this.subscriptionsArr.findIndex(
          (sub) => sub.id.toString() === subscriptionId
        );
        this.subscriptionsArr.splice(index, 1);
      },
    });
  }

  downloadTree() {
    this.treeService.downloadFullTree().subscribe((blob: Blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = this.sessionSource.name + '.txt';
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  addNewSubscription(node: NodeReference) {
    if (!node.nodeId) return;
    this.dialog.open(SubscriptionParametersDialogComponent, {
      data: JSON.parse(JSON.stringify(node.nodeId)),
    });
  }

  addToSubscription(subscriptionName: string, node: NodeReference) {
    if (!node) return;
    this.subscriptionService.addToSubscription(subscriptionName, node.nodeId);
  }

  getLevel = (node: DynamicFlatNode) => node.level;

  isExpandable = (node: DynamicFlatNode) => node.expandable;

  hasChild = (_: number, _nodeData: DynamicFlatNode) => true;
}
