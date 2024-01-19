import { Component, OnInit } from '@angular/core';
import { CommunicationService } from '../services/communication.service';
import { SubscriptionService } from '../services/subscription.service';
import { NodeService } from '../services/node.service';
import { MatDialog } from '@angular/material/dialog';
import { SubscriptionParametersDialogComponent } from './subscription-parameters-dialog/subscription-parameters-dialog.component';
import { SessionAccessorService } from '../shared/session-accessor.service';
import { MonitoredItemConfig } from '../models/monitoredItem';
import {
  SubscriptionConfig,
  SubscriptionValue,
} from '../models/subscriptionModels';
import { MonitoredItemService } from '../services/monitored-item.service';
import { NotificationService } from '../shared/notification.service';

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss'],
})
export class SubscriptionComponent implements OnInit {
  subscriptionsArr: SubscriptionValue[] = [];
  newNode: MonitoredItemConfig = (<
    Partial<MonitoredItemConfig>
  >{}) as MonitoredItemConfig;

  constructor(
    private sessionAccessor: SessionAccessorService,
    private subscriptionService: SubscriptionService,
    private communicationService: CommunicationService,
    private nodeService: NodeService,
    private monitoredItemSvs: MonitoredItemService,
    private notificationService: NotificationService,
    public dialog: MatDialog
  ) {
    this.sessionAccessor.currentSession$.subscribe((res) => {
      this.subscriptionsArr = [];
      //get subscriptions on session connected
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
    });
  }

  ngOnInit(): void {
    //get subscription on monitoring
    this.communicationService.subscriptionObservable.subscribe({
      next: (value: SubscriptionValue) => {
        const index = this.subscriptionsArr.findIndex(
          (sub) => sub.guid === value.guid
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
    if (!this.newNode.startNodeId) return;

    const subscriptionDialog = this.dialog.open(
      SubscriptionParametersDialogComponent,
      {
        data: {
          nodeId: JSON.parse(JSON.stringify(this.newNode.startNodeId)),
          subscriptionParameters: (<
            Partial<SubscriptionConfig>
          >{}) as SubscriptionConfig,
        },
      }
    );
    subscriptionDialog.afterClosed().subscribe((result) => {
      this.newNode.startNodeId = '';
    });
  }

  modifySubscription(subscription: SubscriptionValue) {
    const subscriptionDialog = this.dialog.open(
      SubscriptionParametersDialogComponent,
      {
        data: {
          nodeId: '',
          subscription: subscription,
        },
      }
    );
    subscriptionDialog.afterClosed().subscribe((result) => {
      this.newNode.startNodeId = '';
    });
  }

  stopAllSubscriptions() {
    this.subscriptionService.stopAllSubscriptions().subscribe({
      next: () => {
        this.subscriptionsArr.map((item) => {
          item.publishingEnabled = false;
        });
      },
    });
  }

  addToSubscription(subscription: SubscriptionValue) {
    if (!this.newNode.startNodeId) return;
    if (!subscription.publishingEnabled) {
      this.notificationService.showWarning('Subscription is not active', '');
      return;
    }
    this.monitoredItemSvs.addToSubscription(
      subscription,
      this.newNode.startNodeId
    );
  }

  deleteMonitoringItem(
    subscription: SubscriptionValue,
    nodeId: string,
    event: any
  ) {
    console.log('deleteMonitoringItem');

    this.monitoredItemSvs.deleteMonitoredItem(subscription.opcUaId, nodeId);

    let nodeIndex = subscription.monitoredItems.findIndex(
      (c) => c.startNodeId == nodeId
    );

    subscription.monitoredItems.splice(nodeIndex, 1);
    this.nodeService.nodeToRemove$ = nodeId;
    event.stopPropagation();
    event.preventDefault();
  }

  removeSubscription(subscription: SubscriptionValue, event: any) {
    console.log('removeSubscription');
    this.subscriptionService.deleteSubs(subscription).subscribe({
      next: () => {},
    });
    this.subscriptionService.subscriptionToRemove$ =
      subscription.opcUaId.toString();
    let index = this.subscriptionsArr.findIndex(
      (c) => c.opcUaId === subscription.opcUaId
    );

    subscription.monitoredItems.map((item) => {
      this.nodeService.nodeToRemove$ = item.startNodeId;
    });

    this.subscriptionsArr.splice(index, 1);
  }

  setPublishingMode(subscription: SubscriptionValue, event: any) {
    console.log('setPublishingMode');
    event.stopPropagation();
    event.preventDefault();

    // update model
    subscription.publishingEnabled = !subscription.publishingEnabled;
    this.subscriptionService.setPublishingMode(subscription).subscribe({
      next: (value: SubscriptionValue) => {
        if (value.guid == null) {
          console.error('subscription guid is null');
          this.subscriptionsArr.push(value);
          return;
        }

        const index = this.subscriptionsArr.findIndex(
          (sub) => sub.guid === value.guid
        );
        if (index !== -1) {
          this.subscriptionsArr[index] = value;
        } else {
          this.subscriptionsArr.push(value);
        }
      },
    });
  }
}
