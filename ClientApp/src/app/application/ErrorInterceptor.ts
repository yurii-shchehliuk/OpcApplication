import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { NotificationService } from '../shared/notification.service';
import { SessionService } from '../services/session.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private notifyService: NotificationService,
    private session: SessionService
  ) {}
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // const sessionId = sessionStorage.getItem('sessionId');
    // if (!sessionId && !request.url.includes('session')) {
    //   console.error('Session ID is not found in sessionStorage!');
    //   return throwError('Session ID is not found!');
    // }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMsg = '';

        if (error.error instanceof ErrorEvent) {
          errorMsg = `Error: ${error.error.message}`;
        } else {
          errorMsg = `API Error: ${
            error.error.message || error.message
          }, Status: ${error.status}`;
        }

        // Optionally log to an external logger
        console.error(error);

        // Show a user-friendly message
        this.notifyService.showError(errorMsg, '');

        return throwError(error);
      })
    );
  }
}
