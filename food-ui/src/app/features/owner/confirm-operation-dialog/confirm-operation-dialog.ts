import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-operation-dialog',
  standalone: true,
  templateUrl: './confirm-operation-dialog.html',
  styleUrl: './confirm-operation-dialog.css',
})
export class ConfirmOperationDialogComponent {
  title = input('Confirm changes');
  message = input.required<string>();
  confirmed = output<void>();
  cancelled = output<void>();
}
