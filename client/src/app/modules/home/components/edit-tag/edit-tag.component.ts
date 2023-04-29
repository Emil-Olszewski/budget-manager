import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Observable, take } from "rxjs";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Tag, UpdateTag } from "../../models/tag";

@Component({
  selector: 'app-edit-tag',
  templateUrl: './edit-tag.component.html',
  styleUrls: ['./edit-tag.component.scss']
})
export class EditTagComponent implements OnInit {
  private id!: number;
  @Input() public tag$!: Observable<Tag>;
  @Output() public save$: EventEmitter<UpdateTag> = new EventEmitter<UpdateTag>();
  @Output() public delete$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public goBack$: EventEmitter<never> = new EventEmitter<never>();
  public form: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    type2: new FormControl('', [Validators.required])
  });

  public ngOnInit(): void {
    if (!this.tag$) {
      return;
    }

    this.tag$
      .pipe(take(1))
      .subscribe(x => {
        this.form.controls['name'].setValue(x.name);
        this.form.controls['type2'].setValue(x.type);
        this.id = x.id;
      });
  }

  public submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const tag: UpdateTag = {
      id: this.id,
      name: this.form.controls['name'].value,
      type: +this.form.controls['type2'].value
    };
    this.save$.emit(tag);
  }
}
