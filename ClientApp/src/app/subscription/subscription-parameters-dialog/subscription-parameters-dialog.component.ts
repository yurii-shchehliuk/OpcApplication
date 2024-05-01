import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SubscriptionConfig } from 'src/app/models/subscriptionModels';
import { SharedService } from 'src/app/shared/shared.service';
import { SubscriptionService } from 'src/app/services/subscription.service';
import { BaseComponent } from 'src/app/shared/components/base/base.component';
import { takeUntil } from 'rxjs';

export interface income {
  nodeId: string;
  subscription: any | null;
}
@Component({
  selector: 'app-subscription-parameters-dialog',
  templateUrl: './subscription-parameters-dialog.component.html',
  styleUrls: ['./subscription-parameters-dialog.component.scss'],
})
export class SubscriptionParametersDialogComponent
  extends BaseComponent
  implements OnInit
{
  subscriptionParametersForm: FormGroup;
  isAddMode: boolean = true;

  constructor(
    private fb: FormBuilder,
    private subscriptionService: SubscriptionService,
    private sharedService: SharedService,
    public dialogRef: MatDialogRef<SubscriptionParametersDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: income
  ) {
    super();
    this.subscriptionParametersForm = this.fb.group({
      opcUaId: [0],
      guid: [''],
      displayName: ['', Validators.required],
      maxValue: [0, Validators.required],
      minValue: [0, Validators.required],
      keepAliveCount: [10, Validators.required],
      publishingInterval: [1000, Validators.required],
      lifetimeCount: [1000, Validators.required],
      maxNotificationsPerPublish: [0, Validators.required],
      priority: [255, Validators.required],
      publishingEnabled: [true],
    });
  }

  ngOnInit(): void {
    if (this.data.nodeId === '') {
      this.isAddMode = false;

      this.subscriptionService
        .getSubscriptionConfig(this.data.subscription)
        .subscribe({
          next: (paramaters: SubscriptionConfig) => {
            this.subscriptionParametersForm = this.fb.group({
              opcUaId: [this.data.subscription.opcUaId],
              guid: [this.data.subscription.guid ?? paramaters?.guid],
              displayName: [paramaters?.displayName ?? '', Validators.required],
              maxValue: [paramaters?.maxValue ?? 0, Validators.required],
              minValue: [paramaters?.minValue ?? 0, Validators.required],
              publishingInterval: [
                paramaters?.publishingInterval ?? 1000,
                Validators.required,
              ],
              keepAliveCount: [
                paramaters?.keepAliveCount ?? 10,
                Validators.required,
              ],
              lifetimeCount: [
                paramaters?.lifetimeCount ?? 1000,
                Validators.required,
              ],
              maxNotificationsPerPublish: [
                paramaters?.maxNotificationsPerPublish ?? 0,
                Validators.required,
              ],
              priority: [paramaters?.priority ?? 255, Validators.required],
              publishingEnabled: [paramaters?.publishingEnabled ?? true],
            });
          },
          error: (err) => {
            console.error(err);
          },
        });
    }
  }

  onSubmit() {
    if (this.subscriptionParametersForm.invalid) {
      return;
    }

    if (this.isAddMode) {
      this.createSubscription();
    } else {
      this.modifySubscription();
    }
  }

  private createSubscription() {
    this.subscriptionService.createSubs(
      this.subscriptionParametersForm.value as SubscriptionConfig,
      this.data.nodeId
    );
    this.updateChart();
  }

  private modifySubscription() {
    this.subscriptionService
      .modifySubs(this.subscriptionParametersForm.value as SubscriptionConfig)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (value) => {},
        error: (err) => {
          console.error(err);
        },
      });
    this.updateChart();
  }

  private updateChart() {
    this.sharedService.updateChart(
      this.subscriptionParametersForm.value as SubscriptionConfig
    );
  }
}
