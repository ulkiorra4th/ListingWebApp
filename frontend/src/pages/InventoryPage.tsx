import clsx from 'clsx';
import { Backpack, ListChecks } from 'lucide-react';
import { useMemo, useState } from 'react';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { inventoryItems } from '@/data/mockInventory';
import { formatDate, rarityColor } from '@/utils/formatters';
import { ItemRarity } from '@/types';

const rarityLabel = (rarity: ItemRarity | number) => {
  if (rarity === ItemRarity.Legendary) return 'Legendary';
  if (rarity === ItemRarity.Epic) return 'Epic';
  if (rarity === ItemRarity.Rare) return 'Rare';
  return 'Common';
};

const stateLabel: Record<string, string> = {
  equipped: 'Экипирован',
  tradable: 'Готов к продаже',
  cooldown: 'На перезарядке',
};

export default function InventoryPage() {
  const [search, setSearch] = useState('');
  const [rarity, setRarity] = useState<'all' | string>('all');
  const [state, setState] = useState<'all' | string>('all');
  const [message, setMessage] = useState('');

  const filtered = useMemo(() => {
    let items = inventoryItems;
    if (rarity !== 'all') {
      items = items.filter((i) => `${i.rarity}` === rarity);
    }
    if (state !== 'all') {
      items = items.filter((i) => i.state === state);
    }
    if (search.trim()) {
      const term = search.trim().toLowerCase();
      items = items.filter(
        (i) => i.name.toLowerCase().includes(term) || i.description.toLowerCase().includes(term) || i.entryId.includes(term),
      );
    }
    return items;
  }, [rarity, search, state]);

  return (
    <div className="flex flex-col gap-4">
      <Card title="Мои предметы" subtitle="Инвентарь профиля: смотрите статусы и выставляйте в лоты.">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-4">
          <div className="md:col-span-2">
            <Input
              label="Поиск по названию или EntryId"
              placeholder="например, katana, 6a1b9f52..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              icon={<ListChecks size={16} className="text-slate-400" />}
            />
          </div>
          <Select label="Редкость" value={rarity} onChange={(e) => setRarity(e.target.value as typeof rarity)}>
            <option value="all">Все</option>
            <option value={`${ItemRarity.Legendary}`}>Legendary</option>
            <option value={`${ItemRarity.Epic}`}>Epic</option>
            <option value={`${ItemRarity.Rare}`}>Rare</option>
            <option value={`${ItemRarity.Common}`}>Common</option>
          </Select>
          <Select label="Статус" value={state} onChange={(e) => setState(e.target.value as typeof state)}>
            <option value="all">Все</option>
            <option value="equipped">Экипирован</option>
            <option value="tradable">Готов к продаже</option>
            <option value="cooldown">На перезарядке</option>
          </Select>
        </div>
      </Card>

      {message && (
        <div className="rounded-2xl border border-emerald-400/40 bg-emerald-500/10 px-4 py-3 text-sm text-emerald-100">
          {message}
        </div>
      )}

      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
        {filtered.map((item) => {
          const rarityName = rarityLabel(item.rarity);
          return (
            <Card key={item.id} className="flex flex-col gap-3">
              <div className="flex items-start justify-between gap-3">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-white/5 text-xl">
                    {item.icon ?? '🎁'}
                  </div>
                  <div>
                    <p className="text-lg font-semibold text-white">{item.name}</p>
                    <p className="text-sm text-slate-400">{item.description}</p>
                  </div>
                </div>
                <span
                  className={clsx(
                    'rounded-full bg-gradient-to-r px-3 py-1 text-xs font-semibold text-white',
                    rarityColor(rarityName),
                  )}
                >
                  {rarityName}
                </span>
              </div>

              <div className="flex flex-wrap items-center gap-2 text-xs text-slate-300">
                <Badge tone="info">EntryId: {item.entryId}</Badge>
                <Badge tone="ghost">{stateLabel[item.state]}</Badge>
                {item.tag && <Badge tone="secondary">{item.tag}</Badge>}
              </div>

              <p className="text-xs text-slate-500">{item.lastAction ?? 'Без активности'}</p>

              <div className="flex gap-3">
                <Button
                  className="flex-1"
                  onClick={() => setMessage(`Предмет "${item.name}" готов к листингу. Используйте ID: ${item.entryId}`)}
                >
                  Выставить на продажу
                </Button>
                <Button variant="secondary" onClick={() => setMessage(`Скопирован EntryId: ${item.entryId}`)}>
                  Скопировать ID
                </Button>
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
}
