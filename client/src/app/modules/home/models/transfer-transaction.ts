export interface TransferTransactionAccount {
  id: number;
  name: string;
}

export interface TransferTransactionTransaction {
  id: number;
  amount: number;
}

export interface TransferTransaction {
  id: number,
  accountFrom: TransferTransactionAccount;
  accountTo: TransferTransactionAccount;
  inputTransaction: TransferTransactionTransaction;
  outputTransaction: TransferTransactionTransaction;
  currencyConversionRate: number | null;
  date: string;
}

export interface UpdateTransferTransaction {
  id: number;
  amount: number;
  currencyConversionRate: number | null;
  date: string;
}

export interface CreateTransferTransaction {
  accountFromId: number;
  accountToId: number;
  amount: number;
  currencyConversionRate: number | null;
  date: string;
}

export interface TransferTransactionModel extends CreateTransferTransaction {
  id: number;
}
