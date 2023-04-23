export interface Account {
  id: number;
  name: string;
}
export interface AccountWithBalance extends Account {
  balance: number;
}
