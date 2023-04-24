export class StatsModel {
  balance: number = 0;
  tags: StatsModelTag[] = [];
}

export class StatsModelTag {
  id!: number;
  name!: string;
  balance: number = 0;
  percentageOfTotal!: number;
}
