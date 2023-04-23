import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ToastType } from "../toast.service";
import { Toast } from "bootstrap";
import { fromEvent, take } from "rxjs";
@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss']
})
export class ToastComponent implements OnInit {
  @Input() title!: string;
  @Input() message!: string;
  @Input() type!: ToastType;
  @Output() dispose$: EventEmitter<never> = new EventEmitter<never>();
  @ViewChild('toastElement', { static: true}) toastElement!: ElementRef;
  private toast!: Toast;
  public ngOnInit(): void {
    this.toast = new Toast(
      this.toastElement.nativeElement,
      this.type === ToastType.error
        ? {
          autohide: true,
        }
        : {
          delay: 5000,
        }
    );

    fromEvent(this.toastElement.nativeElement, 'hidden.bs.toast')
      .pipe(take(1))
      .subscribe(() => this.hide());

    this.toast.show();
  }

  public hide() {
    this.dispose$.emit();
  }
}
