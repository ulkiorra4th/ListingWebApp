import clsx from 'clsx';
import { Flame, Search, ShoppingBag, Sparkles } from 'lucide-react';
import { useMemo, useState } from 'react';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { openListings } from '@/data/mockListings';
import { formatCurrency, formatDate, rarityColor } from '@/utils/formatters';
import { ItemRarity, ListingStatus } from '@/types';

const rarityLabel = (rarity: ItemRarity | number) => {
  if (rarity === ItemRarity.Legendary) return 'Legendary';
  if (rarity === ItemRarity.Epic) return 'Epic';
  if (rarity === ItemRarity.Rare) return 'Rare';
  return 'Common';
};

export default function MarketplacePage() {
  const [search, setSearch] = useState('');
  const [rarity, setRarity] = useState<'all' | string>('all');
  const [currency, setCurrency] = useState<'all' | string>('all');
  const [sort, setSort] = useState<'trending' | 'priceAsc' | 'priceDesc'>('trending');

  const curatedOpenListings = useMemo(
    () => openListings.filter((l) => l.status === ListingStatus.Approved),
    [],
  );

  const filtered = useMemo(() => {
    let result = curatedOpenListings;

    if (rarity !== 'all') {
      result = result.filter((l) => `${l.rarity}` === rarity);
    }
    if (currency !== 'all') {
      result = result.filter((l) => l.currency === currency);
    }
    if (search.trim()) {
      const term = search.trim().toLowerCase();
      result = result.filter(
        (l) =>
          l.title.toLowerCase().includes(term) ||
          l.subtitle.toLowerCase().includes(term) ||
          l.seller.toLowerCase().includes(term),
      );
    }

    if (sort === 'priceAsc') {
      result = [...result].sort((a, b) => a.price - b.price);
    } else if (sort === 'priceDesc') {
      result = [...result].sort((a, b) => b.price - a.price);
    } else {
      const score = (demand?: string) => {
        if (demand === 'high') return 2;
        if (demand === 'medium') return 1;
        return 0;
      };
      result = [...result].sort((a, b) => score(b.demand) - score(a.demand));
    }

    return result;
  }, [curatedOpenListings, currency, rarity, search, sort]);

  const currencies = useMemo(
    () => Array.from(new Set(curatedOpenListings.map((l) => l.currency))),
    [curatedOpenListings],
  );

  return (
    <div className="flex flex-col gap-4">
      <Card
        title="Маркет — открытые лоты"
        subtitle="Подборка актуальных предложений. Фильтруйте по редкости, цене и валюте."
      >
        <div className="grid grid-cols-1 gap-4 md:grid-cols-4">
          <div className="md:col-span-2">
            <Input
              label="Поиск по названию/продавцу"
              placeholder="например, bow, katana..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              icon={<Search size={16} className="text-slate-400" />}
            />
          </div>
          <Select label="Редкость" value={rarity} onChange={(e) => setRarity(e.target.value as typeof rarity)}>
            <option value="all">Все</option>
            <option value={`${ItemRarity.Legendary}`}>Legendary</option>
            <option value={`${ItemRarity.Epic}`}>Epic</option>
            <option value={`${ItemRarity.Rare}`}>Rare</option>
            <option value={`${ItemRarity.Common}`}>Common</option>
          </Select>
          <Select label="Валюта" value={currency} onChange={(e) => setCurrency(e.target.value as typeof currency)}>
            <option value="all">Любая</option>
            {currencies.map((c) => (
              <option key={c} value={c}>
                {c}
              </option>
            ))}
          </Select>
          <Select label="Сортировка" value={sort} onChange={(e) => setSort(e.target.value as typeof sort)}>
            <option value="trending">По спросу</option>
            <option value="priceAsc">Цена ↑</option>
            <option value="priceDesc">Цена ↓</option>
          </Select>
        </div>
      </Card>

      <div className="grid grid-cols-1 gap-4 xl:grid-cols-3">
        {filtered.map((listing) => {
          const rarityName = rarityLabel(listing.rarity);
          return (
            <Card key={listing.id} className="flex flex-col gap-3">
              <div className="flex items-start justify-between gap-3">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-white/5 text-xl">
                    {listing.icon ?? '🎲'}
                  </div>
                  <div>
                    <p className="text-lg font-semibold text-white">{listing.title}</p>
                    <p className="text-sm text-slate-400">{listing.subtitle}</p>
                  </div>
                </div>
                <Badge tone="info">{listing.seller}</Badge>
              </div>

              <div className="flex items-center gap-2">
                <span
                  className={clsx(
                    'rounded-full bg-gradient-to-r px-3 py-1 text-xs font-semibold text-white',
                    rarityColor(rarityName),
                  )}
                >
                  {rarityName}
                </span>
                <Badge tone="success">{formatCurrency(listing.price, listing.currency)}</Badge>
                <Badge tone="ghost" className="gap-1">
                  <Flame size={14} className="text-amber-300" />
                  {listing.demand === 'high' ? 'Высокий спрос' : listing.demand === 'medium' ? 'Стабильный' : 'Низкий'}
                </Badge>
              </div>

              <div className="flex items-center justify-between text-xs text-slate-400">
                <span>ID: {listing.id}</span>
                <span>Обновлено: {formatDate(listing.updatedAt)}</span>
              </div>

              <div className="flex gap-3">
                <Button className="flex-1" iconLeft={<ShoppingBag size={16} />}>
                  Купить
                </Button>
                <Button variant="secondary" iconLeft={<Sparkles size={16} />}>
                  В избранное
                </Button>
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
}
