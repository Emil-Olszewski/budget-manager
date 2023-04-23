export enum TransactionType {
  notDefined = 0,
  initialBalance = 1,
  income = 2,
  expense = 3,
  transfer = 4
}

export interface Transaction {
  id: number;
  accountId: number;
  name: string;
  amount: number;
  type: TransactionType
  date: Date;
  tags: TagForTransaction[];
}

export interface TagForTransaction {
  id: number;
  name: string;
}

export interface CreateTransaction {
  accountId: number;
  name: string;
  amount: number;
  type: TransactionType;
  date: Date;
  tagIds: number[];
}

export interface UpdateTransaction extends CreateTransaction {
  id: number;
}


