import { Injectable } from '@angular/core';

import { ToastrService } from 'ngx-toastr';
import { NotificationData } from '../models/notificationModel';
import { LogCategory } from '../models/enums';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private toastr: ToastrService) {}

  show(data: NotificationData) {
    switch (data.logCategory) {
      case LogCategory.info:
        this.toastr.info(data.message, data.title);
        break;

      case LogCategory.success:
        this.toastr.success(data.message, data.title);
        break;

      case LogCategory.warning:
        this.toastr.warning(data.message, data.title);
        break;

      case LogCategory.error:
        this.toastr.error(data.message, data.title);
        break;
    }
  }

  showInfo(message: string, title: string) {
    this.toastr.info(message, title);
  }

  showSuccess(message: string, title: string) {
    this.toastr.success(message, title);
  }

  showWarning(message: string, title: string) {
    this.toastr.warning(message, title);
  }

  showError(message: string, title: string) {
    this.toastr.error(message, title);
  }
}
