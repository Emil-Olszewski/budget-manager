import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable } from "rxjs";
import { Tag } from "../../models/tag";

@Component({
  selector: 'app-tags-list',
  templateUrl: './tags-list.component.html',
  styleUrls: ['./tags-list.component.scss']
})
export class TagsListComponent {
  @Input() public tags$!: Observable<Tag[]>;
  @Output() public addTag$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public addChild$: EventEmitter<number> = new EventEmitter<number>();
  @Output() public editTag$: EventEmitter<number> = new EventEmitter<number>();
}
