import { Injectable } from '@angular/core';
import { ApiService } from "../../../core/http/api.service";
import { Observable } from "rxjs";
import { ApiResponse } from "../../../shared/models/api-response";
import { CreateTag, Tag, UpdateTag } from "../models/tag";

@Injectable({
  providedIn: 'root'
})
export class TagService {
  public constructor(private service: ApiService) { }

  public getAllTags(): Observable<ApiResponse<Tag[]>> {
    return this.service.get("tags");
  }

  public getTag(id: number): Observable<ApiResponse<Tag>> {
    return this.service.get("tags/" + id);
  }

  public createTag(tag: CreateTag): Observable<undefined> {
    return this.service.post("tags", tag);
  }

  public updateTag(tag: UpdateTag): Observable<undefined> {
    return this.service.put("tags/" + tag.id, tag);
  }

  public deleteTag(id: number): Observable<undefined> {
    return this.service.delete("tags/" + id);
  }
}
