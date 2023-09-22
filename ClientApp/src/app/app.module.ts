import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgChartsModule } from 'ng2-charts';
// Material
import { MatCardModule } from '@angular/material/card';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatInputModule } from '@angular/material/input';
import { MatTreeModule } from '@angular/material/tree';

import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ChannelComponent } from './channel/channel.component';
import { ChannelsManagerComponent } from './channels-manager/channels-manager.component';
import { NodeTreeComponent } from './node-tree/node-tree.component';

import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, Injectable } from '@angular/core';
import {
  MatTreeFlatDataSource,
  MatTreeFlattener,
} from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { BehaviorSubject } from 'rxjs';
import { OpcSettingsComponent } from './opc-settings/opc-settings.component';
import { NavbarComponent } from './core/navbar/navbar.component';
@NgModule({
  declarations: [
    AppComponent,
    ChannelComponent,
    ChannelsManagerComponent,
    NodeTreeComponent,
    OpcSettingsComponent,
    NavbarComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    NgChartsModule,
    //material
    MatTreeModule,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatCardModule,
    MatSidenavModule,
    MatButtonModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
