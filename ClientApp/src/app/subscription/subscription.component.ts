import { Component, OnInit } from '@angular/core';
import { CommunicationService } from '../services/communication.service';
import {
  SubscriptionEntity,
  SubscriptionParameters,
} from '../models/subscriptionModels';
import { NodeReference } from '../models/nodeModels';
import { SubscriptionService } from '../services/subscription.service';
import { NodeService } from '../services/node.service';
import { MatDialog } from '@angular/material/dialog';
import { SubscriptionParametersDialogComponent } from './subscription-parameters-dialog/subscription-parameters-dialog.component';
import { SessionAccessorService } from '../shared/session-accessor.service';

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss'],
})
export class SubscriptionComponent implements OnInit {
  subscriptionsArr: SubscriptionEntity[] = [];
  newNode: NodeReference = (<Partial<NodeReference>>{}) as NodeReference;

  constructor(
    private subscriptionService: SubscriptionService,
    private sessionAccessor: SessionAccessorService,
    private nodeService: NodeService,
    public dialog: MatDialog
  ) {
    this.sessionAccessor.currentSession$.subscribe((res) => {
      this.subscriptionsArr = [];
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

        // event.stopPropagation();
        // event.preventDefault();
      },
    });
  }

  addNewSubscription() {
    if (!this.newNode.nodeId) return;

    const subscriptionDialog = this.dialog.open(
      SubscriptionParametersDialogComponent,
      {
        data: JSON.parse(JSON.stringify(this.newNode.nodeId)),
      }
    );
    subscriptionDialog.afterClosed().subscribe((result) => {
      this.newNode.nodeId = '';
    });
  }

  addToSubscription(subscriptionName: string) {
    if (!this.newNode.nodeId) return;

    this.subscriptionService.addToSubscription(
      subscriptionName,
      this.newNode.nodeId
    );
  }

  deleteMonitoringItem(
    subscription: SubscriptionEntity,
    nodeId: string,
    event: any
  ) {
    console.log('deleteMonitoringItem');

    this.subscriptionService.deleteMonitoringItem(subscription.id, nodeId);

    let nodeIndex = subscription.monitoringItems.findIndex(
      (c) => c.startNodeId == nodeId
    );

    subscription.monitoringItems.splice(nodeIndex, 1);
    this.nodeService.nodeToRemove$ = nodeId;
    event.stopPropagation();
    event.preventDefault();
  }

  removeSubscription(subscription: SubscriptionEntity, event: any) {
    console.log('removeSubscription');
    this.subscriptionService.deleteSubs(subscription.id.toString());
    this.subscriptionService.subscriptionToRemove$ = subscription.id.toString();
    let index = this.subscriptionsArr.findIndex(
      (c) => c.id === subscription.id
    );

    subscription.monitoringItems.map((item) => {
      this.nodeService.nodeToRemove$ = item.startNodeId;
    });

    this.subscriptionsArr.splice(index, 1);
  }

  modifySubscription(subscriptionId: number) {
    console.log('modifySubscription');
    if (!this.newNode.nodeId) return;

    const subscriptionDialog = this.dialog.open(
      SubscriptionParametersDialogComponent,
      {
        data: JSON.parse(JSON.stringify(this.newNode.nodeId)),
      }
    );
    subscriptionDialog.afterClosed().subscribe((result) => {
      this.newNode.nodeId = '';
    });
  }

  setPublishingMode(subscription: SubscriptionEntity, event: any) {
    console.log('setPublishingMode');
    event.stopPropagation();
    event.preventDefault();

    subscription.publishingEnabled = !subscription.publishingEnabled;
    this.subscriptionService.setPublishingMode(
      subscription.id,
      subscription.publishingEnabled
    );
  }
}
