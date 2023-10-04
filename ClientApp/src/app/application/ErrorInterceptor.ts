import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, catchError, throwError } from "rxjs";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMsg = '';

        if (error.error instanceof ErrorEvent) {
          errorMsg = `Error: ${error.error.message}`;
        } else {
          errorMsg = `API Error: ${error.error.message || error.message}, Status: ${error.status}`;
        }
        
        // Optionally log to an external logger
        console.error(errorMsg);
        
        // Show a user-friendly message
        this.showToast(errorMsg);
        
        return throwError(error);
      })
    );
  }

  private showToast(message: string): void {
    // Implement a toast notification, Snackbar, or a modal to inform the user.
  }
}
