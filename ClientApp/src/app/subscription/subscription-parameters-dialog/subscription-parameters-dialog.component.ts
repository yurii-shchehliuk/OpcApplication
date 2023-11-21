import { Component, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SubscriptionParameters } from 'src/app/models/subscriptionModels';
import { SubscriptionService } from 'src/app/services/subscription.service';

@Component({
  selector: 'app-subscription-parameters-dialog',
  templateUrl: './subscription-parameters-dialog.component.html',
  styleUrls: ['./subscription-parameters-dialog.component.scss'],
})
export class SubscriptionParametersDialogComponent {
  subscriptionParameters: SubscriptionParameters;
  isAddMode: boolean = true;

  subscriptionParametersForm = this.fb.group({
    displayName: ['', Validators.required],
    publishingInterval: [1000, Validators.required],
    keepAliveCount: [10, Validators.required],
    lifetimeCount: [1000, Validators.required],
    maxNotificationsPerPublish: [0, Validators.required],
    priority: [255, Validators.required],
    publishingEnabled: [true],
  });

  constructor(
    private fb: FormBuilder,
    private subscriptionService: SubscriptionService,
    public dialogRef: MatDialogRef<SubscriptionParametersDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public nodeId: string
  ) {}

  onSubmit() {
    // stop here if form is invalid
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
      this.subscriptionParametersForm.value as SubscriptionParameters,
      this.nodeId
    );
  }

  private modifySubscription() {
    this.subscriptionService.modifySubs(
      this.subscriptionParametersForm.value as SubscriptionParameters,
      Number(this.nodeId)
    );
  }
}
