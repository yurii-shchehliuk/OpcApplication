import { Component, HostListener } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { SidenavService } from '../shared/sidenav-service.service';

@Component({
  selector: 'app-opc-settings',
  templateUrl: './opc-settings.component.html',
  styleUrls: ['./opc-settings.component.scss'],
})
export class OpcSettingsComponent {
  appSettings: any = {
    saveToAzure: false,
    saveToDb: false,
    createFullTree: false,
  };

  appSettingsForm = this.initAppSettings();

  constructor(
    private fb: FormBuilder,
    public sidenavService: SidenavService
  ) {
    // this.appService.getAppSettings.subscribe((data) => {
    //   if (!data.appSettings) {
    //     this.appSettingsForm = this.initAppSettings();
    //     return;
    //   }
    //   console.log(data);

    //   this.appSettings = JSON.parse(JSON.stringify(data.appSettings));
    //   console.log(this.appSettings);

    //   this.appSettingsForm = this.fb.group({
    //     saveToAzure: [this.appSettings.saveToAzure],
    //     saveToDb: [this.appSettings.saveToDb],
    //     targetTable: [this.appSettings.targetTable],
    //     createFullTree: [this.appSettings.createFullTree],
    //     opcUrl: [this.appSettings.opcUrl],
    //     signalR: this.fb.group({
    //       hubUrl: [this.appSettings.signalR.hubUrl],
    //     }),
    //     dbConnectionString: [this.appSettings.dbConnectionString],
    //     azureEventHub: this.fb.group({
    //       endpointUrl: [this.appSettings.azureEventHub.endpointUrl],
    //       hubName: [this.appSettings.azureEventHub.hubName],
    //       eventHubSender: this.fb.group({
    //         policyName: [
    //           this.appSettings.azureEventHub.eventHubSender.policyName,
    //         ],
    //         primaryKey: [
    //           this.appSettings.azureEventHub.eventHubSender.primaryKey,
    //         ],
    //       }),
    //       // eventHubConsumer: this.fb.group({
    //       //   conntainerName: this.appSettings.azureEventHub.ConntainerName,
    //       //   blobConnString: this.appSettings.azureEventHub.BlobConnString,
    //       //   consumerGroup: this.appSettings.azureEventHub.ConsumerGroup,
    //       //   policyName: this.appSettings.azureEventHub.PolicyName,
    //       //   primaryKey: this.appSettings.azureEventHub.PrimaryKey,
    //       // }),
    //     }),
    //   });
    // });
  }

  saveSettings() {
    if (!this.appSettingsForm.valid) {
      return;
    }
    // this.appService.saveSettings(this.appSettingsForm.value);
  }

  initAppSettings() {
    return this.fb.group({
      saveToAzure: [this.appSettings.saveToAzure],
      saveToDb: [this.appSettings.saveToDb],
      targetTable: ['', Validators.required],
      createFullTree: [this.appSettings.createFullTree],
      opcUrl: ['', Validators.required],
      signalR: this.fb.group({
        hubUrl: ['', Validators.required],
      }),
      dbConnectionString: ['', Validators.required],
      azureEventHub: this.fb.group({
        endpointUrl: ['', Validators.required],
        hubName: ['', Validators.required],
        eventHubSender: this.fb.group({
          policyName: ['', Validators.required],
          primaryKey: ['', Validators.required],
        }),
      }),
    });
  }
}
