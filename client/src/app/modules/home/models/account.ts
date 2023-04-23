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
  balance: number;
}

export interface AccountWithInitialBalance {
  id: number;
  name: string;
  currency: Currency;
  initialBalance: number | null;
}

export interface CreateAccount {
  name: string;
  currency: Currency;
  initialBalance: number | null;
}

export interface UpdateAccount extends CreateAccount {
  id: number;
}
