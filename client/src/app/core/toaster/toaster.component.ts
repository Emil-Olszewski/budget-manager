import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ToastEvent, ToastService } from "./toast.service";
import { Subscription } from "rxjs";

@Component({
  selector: 'app-toaster',
  templateUrl: './toaster.component.html',
  styleUrls: ['./toaster.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ToasterComponent implements OnInit, OnDestroy {
  public currentToasts: ToastEvent[] = [];
  private sub!: Subscription;
  public constructor(private service: ToastService, private cdr: ChangeDetectorRef) {
  }

  public ngOnInit() {
    this.sub = this.service.toastEvents$.subscribe(event =>{
      this.currentToasts.push(event);
      this.cdr.detectChanges();
    });
  }

  public dispose(index: number) {
    this.currentToasts.splice(index, 1);
    this.cdr.detectChanges();
  }

  public ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
