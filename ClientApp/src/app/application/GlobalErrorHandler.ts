import { Injectable, ErrorHandler } from "@angular/core";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  handleError(error: Error) {
    // Log the error (e.g., to an external service)
    console.error('Error: ', error);
    
    // Optionally, display a user-friendly error message
  }
}
