export type ApiResponse<T> = {
  status: string;
  data: T;
  error?: string;
};

export type LoginResponse = {
  accessToken: string;
  refreshToken: string;
  accountId: string;
};

export enum AccountStatus {
  Unverified = 0,
  Verified = 1,
  Banned = 2,
  Deleted = 3,
}

export type Account = {
  id: string;
  email: string;
  status: AccountStatus | number;
  createdAt: string;
  updatedAt: string;
};

export enum LanguageCode {
  English = 0,
  French = 1,
  Italian = 2,
  Spanish = 3,
  Russian = 4,
}

export enum CountryCode {
  England = 0,
  Spain = 1,
  France = 2,
  Norway = 3,
  Russia = 4,
  Italy = 5,
}

export enum ItemRarity {
  Common = 0,
  Rare = 1,
  Epic = 2,
  Legendary = 3,
}

export enum ListingStatus {
  Draft = 0,
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  Closed = 4,
}

export type Profile = {
  id: string;
  accountId: string;
  nickname: string;
  age: number;
  iconKey?: string | null;
  languageCode: LanguageCode | number;
  countryCode: CountryCode | number;
  createdAt: string;
  updatedAt: string;
};

export type Currency = {
  currencyCode: string;
  name: string;
  description?: string;
  iconKey?: string;
  minTransferAmount?: number;
  maxTransferAmount?: number;
  isTransferAllowed: boolean;
};

export type Wallet = {
  currencyCode: string;
  accountId: string;
  balance: number;
  lastTransactionDate?: string | null;
  isActive: boolean;
};

export type Item = {
  id: string;
  name: string;
  description?: string | null;
  rarity: ItemRarity | number;
  basePrice: number;
  iconKey?: string | null;
  releaseDate: string;
  isTrading: boolean;
};

export type ItemEntry = {
  id: string;
  ownerId: string;
  itemTypeId: string;
  createdAt: string;
  pseudonym?: string | null;
};

export type Listing = {
  id: string;
  itemEntryId: string;
  sellerId: string;
  priceAmount: number;
  currencyCode: string;
  createdAt: string;
  updatedAt: string;
  status: ListingStatus | number;
};

export type TradeTransaction = {
  id: string;
  buyerAccountId: string;
  sellerAccountId: string;
  currencyCode: string;
  listingId: string;
  amount: number;
  transactionDate: string;
  isSuspicious: boolean;
};

export type AuthTokens = {
  accessToken: string;
  refreshToken: string;
  accountId: string;
};

export type CreateListingPayload = {
  sellerId: string;
  itemEntryId: string;
  priceAmount: number;
  currencyCode: string;
  status?: ListingStatus | number;
};
