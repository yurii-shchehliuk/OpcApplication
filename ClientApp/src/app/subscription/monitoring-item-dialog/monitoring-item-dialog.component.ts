import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MonitoredItemConfig } from '../../models/monitoredItem';
import { MonitoredItemService } from '../../services/monitored-item.service';

@Component({
  selector: 'app-monitoring-item-dialog',
  templateUrl: './monitoring-item-dialog.component.html',
  styleUrls: ['./monitoring-item-dialog.component.scss'],
})
export class MonitoringItemDialogComponent implements OnInit {
  monitoredItemConfiguration: MonitoredItemConfig;
  isAddMode: boolean = true;

  monitoredItemForm = this.fb.group({
    displayName: ['', Validators.required],
    samplingInterval: [0, Validators.required],
    queueSize: [1, Validators.required],
    discardOldest: [true],
    startNodeId: [''],
    indexRange: [''],
    monitoringMode: [''],
    attributeId: [''],
    nodeClass: [''],
    relativePath: [''],
  });

  constructor(
    private fb: FormBuilder,
    private monitoredItemService: MonitoredItemService,
    public dialogRef: MatDialogRef<MonitoringItemDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public nodeId: string
  ) {}

  ngOnInit(): void {
    // this.monitoredItemService.getMonitoredItem().subscribe();
  }

  onSubmit() {
    // stop here if form is invalid
    if (this.monitoredItemForm.invalid) {
      return;
    }

    this.modifySubscription();
  }

  private modifySubscription() {}
}
