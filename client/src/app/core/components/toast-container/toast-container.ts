import { Component } from '@angular/core';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-toast-container',
  imports: [NgbToastModule],
  templateUrl: './toast-container.html',
  host: {
    class: 'toast-container position-fixed top-0 end-0 p-3',
    style: 'z-index: 1200',
  },
})
export class ToastContainer {
  public constructor(protected toastService: ToastService) {}
}
