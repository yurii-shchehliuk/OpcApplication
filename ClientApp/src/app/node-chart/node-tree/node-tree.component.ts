import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import {
  DynamicDataSource,
  DynamicDatabase,
  DynamicFlatNode,
} from './dynamic-database.service';
import { SessionEntity } from 'src/app/models/sessionModels';
import { SessionAccessorService } from 'src/app/shared/session-accessor.service';
import { SubscriptionParametersDialogComponent } from 'src/app/subscription/subscription-parameters-dialog/subscription-parameters-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { TreeService } from 'src/app/services/tree.service';
import { SubscriptionService } from 'src/app/services/subscription.service';
import { SessionState } from 'src/app/models/enums';
import {
  SubscriptionConfig,
  SubscriptionValue,
} from 'src/app/models/subscriptionModels';
import { MonitoredItemValue } from 'src/app/models/monitoredItem';
import { MonitoredItemService } from 'src/app/services/monitored-item.service';

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
  subscriptionsArr: SubscriptionValue[] = [];

  constructor(
    private database: DynamicDatabase,
    private sessionAccessor: SessionAccessorService,
    private treeService: TreeService,
    public dialog: MatDialog,
    private subscriptionService: SubscriptionService,
    private monitoredItemSvs: MonitoredItemService
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
    this.subscriptionService.getActiveSubscriptions().subscribe({
      next: (subsValArr: SubscriptionValue[]) => {
        subsValArr.map((value) => {
          const index = this.subscriptionsArr.findIndex(
            (sub) => sub.guid === value.guid
          );
          if (index !== -1) {
            this.subscriptionsArr[index] = value;
          } else {
            this.subscriptionsArr.push(value);
          }
        });
        // event.stopPropagation();
        // event.preventDefault();
      },
    });

    this.subscriptionService.subscriptionToRemove$.subscribe({
      next: (subscriptionId: string) => {
        const index = this.subscriptionsArr.findIndex(
          (sub) => sub.opcUaId.toString() === subscriptionId
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

  addNewSubscription(node: MonitoredItemValue) {
    if (!node.startNodeId) return;
    this.dialog.open(SubscriptionParametersDialogComponent, {
      data: {
        nodeId: JSON.parse(JSON.stringify(node.startNodeId)),
        opcUaId: null,
      },
    });
  }

  addToSubscription(subscription: SubscriptionValue, node: MonitoredItemValue) {
    if (!node) return;
    this.monitoredItemSvs.addToSubscription(subscription, node.startNodeId);
  }

  getLevel = (node: DynamicFlatNode) => node.level;

  isExpandable = (node: DynamicFlatNode) => node.expandable;

  hasChild = (_: number, _nodeData: DynamicFlatNode) => true;
}
