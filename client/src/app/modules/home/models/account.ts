export enum Currency {
  notDefined = 0,
  pln = 1,
  eur = 2,
  usd = 3
}

export interface Account {
  id: number;
  name: string;
  currency: Currency;
}
export interface AccountWithBalance extends Account {
  balance: number;
}
