import { Component, OnInit } from '@angular/core';
import { TagService } from "../../services/tag.service";
import { ReplaySubject, take } from "rxjs";
import { Tag } from "../../models/tag";
import { Router } from "@angular/router";

@Component({
  selector: 'app-tags-container',
  templateUrl: './tags-container.component.html',
  styleUrls: ['./tags-container.component.scss']
})
export class TagsContainerComponent implements OnInit {
  public tags$: ReplaySubject<Tag[]> = new ReplaySubject<Tag[]>();
  constructor(private service: TagService, private router: Router) {
  }

  public ngOnInit(): void {
    this.service.getAllTags()
      .pipe(take(1))
      .subscribe({
        next: response => this.tags$.next(response.body)
      });
  }

  public addTag(): void {
    this.router.navigate(['tags', 'edit', 0]);
  }

  public addChild(parentId: number): void {
    this.router.navigate(['tags','edit', '0'], { queryParams: {forParent: parentId} });
  }

  public editTag(id: number): void {
    this.router.navigate(['tags', 'edit', id]);
  }
}
