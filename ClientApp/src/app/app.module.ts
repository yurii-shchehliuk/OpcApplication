import { ErrorHandler, NgModule } from '@angular/core';
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

import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { BehaviorSubject } from 'rxjs';
import { OpcSettingsComponent } from './opc-settings/opc-settings.component';
import { NavbarComponent } from './core/navbar/navbar.component';
import { ErrorInterceptor } from './application/ErrorInterceptor';
import { GlobalErrorHandler } from './application/GlobalErrorHandler';
import { SessionComponent } from './session/session.component';
import { SessionDialogComponent } from './session/session-dialog/session-dialog.component';
import { NodeTreeComponent } from './node-chart/node-tree/node-tree.component';
import { FooterComponent } from './core/footer/footer.component';
import { NodeChartComponent } from './node-chart/node-chart.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { ToastrModule } from 'ngx-toastr';

@NgModule({
  declarations: [
    AppComponent,
    NodeTreeComponent,
    OpcSettingsComponent,
    NavbarComponent,
    SessionComponent,
    SessionDialogComponent,
    NodeChartComponent,
    FooterComponent,
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
    MatDialogModule,
    MatExpansionModule,
    MatProgressBarModule,
    //
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
      closeButton: true,
      timeOut: 3000,
    }),
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    // { provide: ErrorHandler, useClass: GlobalErrorHandler },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
