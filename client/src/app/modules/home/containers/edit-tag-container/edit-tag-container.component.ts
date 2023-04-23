import { Component, OnInit } from '@angular/core';
import { TagService } from "../../services/tag.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ReplaySubject, take } from "rxjs";
import { CreateTag, Tag, UpdateTag } from "../../models/tag";

@Component({
  selector: 'app-edit-tag-container',
  templateUrl: './edit-tag-container.component.html',
  styleUrls: ['./edit-tag-container.component.scss']
})
export class EditTagContainerComponent implements OnInit {
  private id!: number;
  private parentId: number | null = null;
  public tag$: ReplaySubject<Tag> = new ReplaySubject<Tag>();
  public constructor(private service: TagService, private route: ActivatedRoute, private router: Router) {
  }

  public ngOnInit(): void {
    this.getIdFromRoute();
    this.route.queryParams.pipe(take(1))
      .subscribe(result => {
        const parentId = result['forParent'];
        if (parentId && parentId > 0) {
          this.parentId = parentId;
        }
      });
  }

  private getIdFromRoute() {
    this.route.params
      .pipe(take(1))
      .subscribe(result => {
        const id = result["id"];
        this.id = id;
        if (id > 0) {
          this.getTag(id);
        }
      });
  }

  private getTag(id: number): void {
    this.service.getTag(id)
      .pipe(take(1))
      .subscribe({
        next: response => {
          this.tag$.next(response.body);
        }
      });
  }

  public save(tag: UpdateTag) {
    if (tag.id > 0) {
      this.service.updateTag(tag)
        .pipe(take(1))
        .subscribe({
          next: () => this.goBack()
        });
    } else {
      const createTag: CreateTag = {
        parentId: this.parentId,
        name: tag.name,
        type: tag.type
      }
      this.service.createTag(createTag).pipe(take(1))
        .subscribe({
          next: () => this.goBack()
        });
    }
  }

  public delete(): void {
    this.service.deleteTag(this.id)
      .pipe(take(1))
      .subscribe({
        next: () => this.goBack()
      });
  }

  public goBack(): void {
    this.router.navigate(['tags'])
  }
}
