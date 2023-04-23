export enum TagType {
  notDefined = 0,
  forIncome = 1,
  ForOutcome = 2
}

export interface Tag {
  id: number;
  name: string
  type: TagType;
  children: Tag[];
}


export interface CreateTag {
  parentId: number | null;
  name: string;
  type: TagType;
}

export interface UpdateTag extends Omit<CreateTag, "parentId"> {
  id: number;
}
