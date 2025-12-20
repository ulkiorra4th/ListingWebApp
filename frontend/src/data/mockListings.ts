import { ItemRarity, ListingStatus } from '@/types';

export type MarketListing = {
  id: string;
  itemEntryId: string;
  title: string;
  subtitle: string;
  rarity: ItemRarity;
  price: number;
  currency: string;
  seller: string;
  status: ListingStatus;
  icon?: string;
  demand?: 'high' | 'medium' | 'low';
  updatedAt: string;
  isMine?: boolean;
};

export const openListings: MarketListing[] = [
  {
    id: '8c2a6f6a-4b1f-4e2b-8b8c-f21e0c14b101',
    itemEntryId: 'f48a9e1b-3c2b-4873-9b9c-0c2f1b4c77ae',
    title: 'Crimson Katana',
    subtitle: 'Легендарный клинок с эффектом горения',
    rarity: ItemRarity.Legendary,
    price: 1250,
    currency: 'USD',
    seller: 'NeoSamurai',
    status: ListingStatus.Approved,
    icon: '🗡️',
    updatedAt: '2025-12-12T10:00:00Z',
  },
  {
    id: '12c43b1a-8be6-4768-9cd9-92db0a1fc874',
    itemEntryId: '6a1b9f52-07c3-4b72-a701-85e89c6a1c16',
    title: 'Aurora Bow',
    subtitle: 'Стрелы с ледяным уроном и замедлением',
    rarity: ItemRarity.Epic,
    price: 640,
    currency: 'USD',
    seller: 'Frostbite',
    status: ListingStatus.Approved,
    icon: '🏹',
    updatedAt: '2025-12-12T09:10:00Z',
  },
  {
    id: '06d1b6a1-3f7f-4e9c-bf2c-02f9d7e4c1b9',
    itemEntryId: '5a07b1c8-53cf-4c2e-8c12-217944b16f2c',
    title: 'Stormcaster Rifle',
    subtitle: 'Пробивной импульсный выстрел, подходит для PvP',
    rarity: ItemRarity.Epic,
    price: 520,
    currency: 'USD',
    seller: 'VoltRider',
    status: ListingStatus.Approved,
    icon: '🔫',
    updatedAt: '2025-12-12T08:45:00Z',
  },
  {
    id: '7f5a1b2c-9d4e-4fa1-8739-8f1c2d0b4e67',
    itemEntryId: '98f3c6b2-7e4a-4b1c-9d2e-0f1a6c3b4e57',
    title: 'Desert Nomad Armor',
    subtitle: 'Редкий сет с бонусом к уклонению',
    rarity: ItemRarity.Rare,
    price: 210,
    currency: 'USD',
    seller: 'SandRunner',
    status: ListingStatus.Approved,
    icon: '🛡️',
    updatedAt: '2025-12-12T07:30:00Z',
  },
  {
    id: '0f3e5b7c-2a9d-4c6b-8f1a-3d7e9c2b5a61',
    itemEntryId: '1b2c3d4e-5f6a-7b8c-9d0e-1f2a3b4c5d6e',
    title: 'Void Runner Boots',
    subtitle: 'Эпический бонус к скорости и двойной прыжок',
    rarity: ItemRarity.Epic,
    price: 430,
    currency: 'USD',
    seller: 'GhostStep',
    status: ListingStatus.Approved,
    icon: '🥾',
    updatedAt: '2025-12-12T06:20:00Z',
  },
  {
    id: 'b3a2c1d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d',
    itemEntryId: 'c4d5e6f7-a8b9-4c0d-1e2f-3a4b5c6d7e8f',
    title: 'Glacier Shard',
    subtitle: 'Редкий амулет с сопротивлением холоду',
    rarity: ItemRarity.Rare,
    price: 155,
    currency: 'USD',
    seller: 'Frostbite',
    status: ListingStatus.Approved,
    icon: '🧊',
    updatedAt: '2025-12-12T06:00:00Z',
    isMine: true,
  },
  {
    id: 'a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d',
    itemEntryId: 'd4c3b2a1-f6e5-8b7a-9c0d-1e2f3a4b5c6d',
    title: 'Iron Warden Shield',
    subtitle: 'Надежный щит для PvE и рейдов',
    rarity: ItemRarity.Common,
    price: 75,
    currency: 'USD',
    seller: 'Guardian',
    status: ListingStatus.Approved,
    icon: '🛡️',
    updatedAt: '2025-12-12T05:45:00Z',
    isMine: true,
  },
  {
    id: 'c0ffee00-1234-4567-8910-abcdefabcdef',
    itemEntryId: 'deadbeef-0000-0000-0000-feedfaceb00c',
    title: 'Midnight Dagger',
    subtitle: 'Тихое оружие для скрытных билдов',
    rarity: ItemRarity.Rare,
    price: 190,
    currency: 'USD',
    seller: 'Shadowstep',
    status: ListingStatus.Approved,
    icon: '🗡️',
    updatedAt: '2025-12-12T05:30:00Z',
  },
];
