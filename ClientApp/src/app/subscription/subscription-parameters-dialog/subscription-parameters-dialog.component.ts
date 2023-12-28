import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SubscriptionConfig } from 'src/app/models/subscriptionModels';
import { SubscriptionService } from 'src/app/services/subscription.service';

export interface income {
  nodeId: string;
  subscription: any | null;
}
@Component({
  selector: 'app-subscription-parameters-dialog',
  templateUrl: './subscription-parameters-dialog.component.html',
  styleUrls: ['./subscription-parameters-dialog.component.scss'],
})
export class SubscriptionParametersDialogComponent implements OnInit {
  subscriptionParametersForm: FormGroup;
  isAddMode: boolean = true;

  constructor(
    private fb: FormBuilder,
    private subscriptionService: SubscriptionService,
    public dialogRef: MatDialogRef<SubscriptionParametersDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: income
  ) {
    this.subscriptionParametersForm = this.fb.group({
      opcUaId: [0],
      subscriptionGuidId: [''],
      displayName: ['', Validators.required],
      publishingInterval: [1000, Validators.required],
      keepAliveCount: [10, Validators.required],
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
              subscriptionGuidId: [
                this.data.subscription.subscriptionGuidId ??
                  paramaters?.subscriptionGuidId,
              ],
              displayName: [paramaters?.displayName ?? '', Validators.required],
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
  }

  private modifySubscription() {
    this.subscriptionService.modifySubs(
      this.subscriptionParametersForm.value as SubscriptionConfig
    );
  }
}
