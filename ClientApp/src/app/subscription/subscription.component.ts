import { Component, OnInit } from '@angular/core';
import { CommunicationService } from '../services/communication.service';
import { SubscriptionService } from '../services/subscription.service';
import { MatDialog } from '@angular/material/dialog';
import { SubscriptionParametersDialogComponent } from './subscription-parameters-dialog/subscription-parameters-dialog.component';
import { SessionAccessorService } from '../shared/session-accessor.service';
import {
  MonitoredItemConfig,
  MonitoredItemValue,
} from '../models/monitoredItem';
import {
  SubscriptionConfig,
  SubscriptionValue,
} from '../models/subscriptionModels';
import { MonitoredItemService } from '../services/monitored-item.service';
import { NotificationService } from '../shared/notification.service';
import { BaseComponent } from '../shared/components/base/base.component';
import { takeUntil } from 'rxjs';
import { SharedService } from '../shared/shared.service';

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss'],
})
export class SubscriptionComponent extends BaseComponent implements OnInit {
  subscriptionsArr: SubscriptionValue[] = [];
  newNode = (<Partial<MonitoredItemConfig>>{}) as MonitoredItemConfig;

  constructor(
    private sessionAccessor: SessionAccessorService,
    private subscriptionService: SubscriptionService,
    private communicationService: CommunicationService,
    private monitoredItemService: MonitoredItemService,
    private monitoredItemSvs: MonitoredItemService,
    private notificationService: NotificationService,
    private sharedService: SharedService,
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit(): void {
    this.sessionAccessor.currentSession$
      .pipe(takeUntil(this.destroy$))
      .subscribe((res) => {
        this.getSubscriptions();
      });

    //get subscription on monitoring
    this.communicationService.subscriptionObservable
      .pipe(takeUntil(this.destroy$))
      .subscribe({
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

  addToSubscription(subscription: SubscriptionValue) {
    if (!this.newNode.startNodeId) return;
    //TODO: add when not active
    if (!subscription.publishingEnabled) {
      this.notificationService.showWarning('Subscription is not active', '');
      return;
    }
    this.monitoredItemSvs.addToSubscription(
      subscription,
      this.newNode.startNodeId
    );
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

    subscriptionDialog
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((result) => {
        if (!result) return;

        this.newNode.startNodeId = '';
        this.getSubscriptions();
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
    subscriptionDialog
      .afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe((result) => {
        if (!result) return;

        this.newNode.startNodeId = '';
        this.getSubscriptions();
      });
  }

  stopAllSubscriptions() {
    this.subscriptionService
      .stopAllSubscriptions()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.subscriptionsArr.map((item) => {
            item.publishingEnabled = false;
          });
        },
      });
  }

  deleteMonitoredItem(
    subscription: SubscriptionValue,
    monitoredItem: MonitoredItemValue,
    event: any
  ) {
    console.log('deleteMonitoringItem');

    this.monitoredItemSvs.deleteMonitoredItem(
      subscription.opcUaId,
      subscription.guid,
      monitoredItem.startNodeId
    );

    let nodeIndex = subscription.monitoredItems.findIndex(
      (c) => c.startNodeId == monitoredItem.startNodeId
    );

    subscription.monitoredItems.splice(nodeIndex, 1);
    this.monitoredItemService.nodeToRemove$ = monitoredItem.startNodeId;
    event.stopPropagation();
    event.preventDefault();
  }

  removeSubscription(subscription: SubscriptionValue, event: any) {
    console.log('removeSubscription');
    this.subscriptionService
      .deleteSubs(subscription)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {},
      });
    this.subscriptionService.subscriptionToRemove$ =
      subscription.opcUaId.toString();
    let index = this.subscriptionsArr.findIndex(
      (c) => c.opcUaId === subscription.opcUaId
    );

    subscription.monitoredItems.map((item) => {
      this.monitoredItemService.nodeToRemove$ = item.startNodeId;
    });

    this.subscriptionsArr.splice(index, 1);
  }

  setPublishingMode(subscription: SubscriptionValue, event: any) {
    event.stopPropagation();
    event.preventDefault();

    // update model
    subscription.publishingEnabled = !subscription.publishingEnabled;
    this.subscriptionService
      .setPublishingMode(subscription)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (value: SubscriptionValue) => {
          
          if (value.guid == null) {
            console.error('subscription guid is null');
            return;
          }
          this.sharedService.updateChart(value as any);
          const index = this.subscriptionsArr.findIndex(
            (sub) => sub.guid === value.guid
          );
          if (index !== -1) {
            this.subscriptionsArr[index] = value;
          } else {
            this.subscriptionsArr.push(value);
          }
        },
        error:(err)=> {
          console.error(err);
        },
      });
  }

  private getSubscriptions() {
    this.subscriptionsArr = [];
    //get subscriptions on session connected
    this.subscriptionService
      .getActiveSubscriptions()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
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
  }
}
