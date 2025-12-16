import { ItemRarity } from '@/types';

export type InventoryItem = {
  id: string;
  entryId: string;
  name: string;
  rarity: ItemRarity;
  description: string;
  state: 'equipped' | 'tradable' | 'cooldown';
  icon?: string;
  lastAction?: string;
  tag?: string;
};

export const inventoryItems: InventoryItem[] = [
  {
    id: 'item-001',
    entryId: 'f48a9e1b-3c2b-4873-9b9c-0c2f1b4c77ae',
    name: 'Crimson Katana',
    rarity: ItemRarity.Legendary,
    description: 'Легендарный клинок. Даёт +15% к скорости удара, эффект поджога.',
    state: 'equipped',
    icon: '🗡️',
    lastAction: 'Использовалось в рейде 2 часа назад',
    tag: 'Любимый',
  },
  {
    id: 'item-002',
    entryId: '6a1b9f52-07c3-4b72-a701-85e89c6a1c16',
    name: 'Aurora Bow',
    rarity: ItemRarity.Epic,
    description: 'Эпический лук с ледяными стрелами. +10% к замедлению.',
    state: 'tradable',
    icon: '🏹',
    lastAction: 'Готов к продаже',
  },
  {
    id: 'item-003',
    entryId: '5a07b1c8-53cf-4c2e-8c12-217944b16f2c',
    name: 'Stormcaster Rifle',
    rarity: ItemRarity.Epic,
    description: 'Пробивной импульсный выстрел. Лучший выбор для PvP.',
    state: 'tradable',
    icon: '🔫',
    lastAction: 'Добавлен в инвентарь сегодня',
  },
  {
    id: 'item-004',
    entryId: '98f3c6b2-7e4a-4b1c-9d2e-0f1a6c3b4e57',
    name: 'Desert Nomad Armor',
    rarity: ItemRarity.Rare,
    description: 'Сет с бонусом к уклонению и регенерации выносливости.',
    state: 'cooldown',
    icon: '🛡️',
    lastAction: 'Можно выставить через 1 час',
  },
  {
    id: 'item-005',
    entryId: 'c4d5e6f7-a8b9-4c0d-1e2f-3a4b5c6d7e8f',
    name: 'Glacier Shard',
    rarity: ItemRarity.Rare,
    description: 'Амулет с сопротивлением холоду и +5% к броне.',
    state: 'tradable',
    icon: '🧊',
    lastAction: 'Готов к листингу',
    tag: 'PvE',
  },
  {
    id: 'item-006',
    entryId: 'd4c3b2a1-f6e5-8b7a-9c0d-1e2f3a4b5c6d',
    name: 'Iron Warden Shield',
    rarity: ItemRarity.Common,
    description: 'Надежный щит для данжей. Добавляет устойчивость к контролю.',
    state: 'tradable',
    icon: '🛡️',
    lastAction: 'Использовался вчера',
  },
];
