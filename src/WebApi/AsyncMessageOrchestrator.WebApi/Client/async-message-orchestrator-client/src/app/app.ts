import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SignalRService } from './services/signalR.service';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
   messageForm: FormGroup;
  isSending = false;
  successMessage = '';
  errorMessage = '';

  // ⚡ Replace with your Azure Function URL

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private signalRService: SignalRService) {
    this.messageForm = this.fb.group({
      message: ['', [Validators.required, Validators.minLength(5)]]
    });
    this.signalRService.startConnection();
  }

  ngOnDestroy(): void {
    this.signalRService.stopConnection();
  }

  onSubmit() {
    if (this.messageForm.invalid) return;

    this.isSending = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.http.post(environment.AzureFunctionUrl, { message: this.messageForm.value.message })
      .subscribe({
        next: (response: any) => {
          this.successMessage = response.message;
          alert(this.successMessage);
          this.isSending = false;
          this.messageForm.reset();
        },
        error: () => {
          this.errorMessage = 'Failed to send message. Please try again.';
          this.isSending = false;
        }
      });
  }
}
