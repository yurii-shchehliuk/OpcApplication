import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  catchError,
  finalize,
  of,
  switchMap,
  throwError,
} from 'rxjs';
import { NotificationService } from '../shared/notification.service';
import { SessionAccessorService } from '../shared/session-accessor.service';
import { LoadingService } from '../shared/loading.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private loadingService: LoadingService,
    private notifyService: NotificationService,
    private sessionAccessorService: SessionAccessorService
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (request.url.includes('session')) {
      this.loadingService.show();
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMsg = '';

        if (error.error instanceof ErrorEvent) {
          errorMsg = `Error: ${error.error.message}`;
        } else {
          errorMsg = `${error.error?.message ?? error.message}, Status: ${
            error.status
          }`;
        }
        console.error(error);
        if (error.status === 0 && error.url?.includes('session')) {
          this.notifyService.showError('', 'Server is not accessible');
        }
        return throwError(error);
      }),
      finalize(() => this.loadingService.hide())
    );
  }
}
