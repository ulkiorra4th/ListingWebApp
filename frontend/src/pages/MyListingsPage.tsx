import clsx from 'clsx';
import { AlertCircle, CheckCircle2, Clipboard, ShieldCheck, ShoppingCart } from 'lucide-react';
import { useMemo, useState } from 'react';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { openListings } from '@/data/mockListings';
import { useListings } from '@/hooks/useListings';
import { formatCurrency, formatDate, rarityColor } from '@/utils/formatters';
import { ItemRarity, ListingStatus } from '@/types';

const statusLabel: Record<number, string> = {
  [ListingStatus.Draft]: 'Черновик',
  [ListingStatus.Pending]: 'Модерация',
  [ListingStatus.Approved]: 'Открыт',
  [ListingStatus.Rejected]: 'Отклонён',
  [ListingStatus.Closed]: 'Закрыт',
};

const rarityLabel = (rarity: ItemRarity | number) => {
  if (rarity === ItemRarity.Legendary) return 'Legendary';
  if (rarity === ItemRarity.Epic) return 'Epic';
  if (rarity === ItemRarity.Rare) return 'Rare';
  return 'Common';
};

export default function MyListingsPage() {
  const myListings = useMemo(() => openListings.filter((l) => l.isMine), []);
  const [status, setStatus] = useState<'all' | string>('all');
  const [message, setMessage] = useState('');
  const [lookupId, setLookupId] = useState('');
  const { listing, loading, fetchListing } = useListings();

  const filtered = useMemo(() => {
    if (status === 'all') return myListings;
    return myListings.filter((l) => `${l.status}` === status);
  }, [myListings, status]);

  const stats = useMemo(
    () => ({
      open: myListings.filter((l) => l.status === ListingStatus.Approved).length,
      pending: myListings.filter((l) => l.status === ListingStatus.Pending).length,
      closed: myListings.filter((l) => l.status === ListingStatus.Closed).length,
    }),
    [myListings],
  );

  const handleLookup = async () => {
    if (!lookupId) return;
    setMessage('');
    try {
      await fetchListing(lookupId);
      setMessage('Лот загружен из бэкенда.');
    } catch (err) {
      setMessage((err as Error).message);
    }
  };

  return (
    <div className="flex flex-col gap-4">
      <Card title="Мои лоты" subtitle="Следите за статусами и проверяйте реальные лоты по ID.">
        <div className="grid grid-cols-1 gap-3 md:grid-cols-4">
          <Select label="Статус" value={status} onChange={(e) => setStatus(e.target.value)}>
            <option value="all">Все</option>
            <option value={`${ListingStatus.Approved}`}>Открыт</option>
            <option value={`${ListingStatus.Pending}`}>Модерация</option>
            <option value={`${ListingStatus.Closed}`}>Закрыт</option>
          </Select>
          <div className="rounded-2xl border border-white/10 bg-white/5 p-3 text-sm text-slate-200">
            <p className="text-xs uppercase tracking-wide text-slate-400">Открыто</p>
            <p className="text-lg font-semibold text-white">{stats.open}</p>
          </div>
          <div className="rounded-2xl border border-white/10 bg-white/5 p-3 text-sm text-slate-200">
            <p className="text-xs uppercase tracking-wide text-slate-400">Модерация</p>
            <p className="text-lg font-semibold text-white">{stats.pending}</p>
          </div>
          <div className="rounded-2xl border border-white/10 bg-white/5 p-3 text-sm text-slate-200">
            <p className="text-xs uppercase tracking-wide text-slate-400">Закрыто</p>
            <p className="text-lg font-semibold text-white">{stats.closed}</p>
          </div>
        </div>
      </Card>

      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
        {filtered.map((lot) => {
          const rarityName = rarityLabel(lot.rarity);
          return (
            <Card key={lot.id} className="flex flex-col gap-3">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <p className="text-lg font-semibold text-white">{lot.title}</p>
                  <p className="text-sm text-slate-400">{lot.subtitle}</p>
                  <p className="text-xs text-slate-500">ItemEntry: {lot.itemEntryId}</p>
                </div>
                <Badge tone="info">{statusLabel[lot.status] ?? lot.status}</Badge>
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
                <Badge tone="success">{formatCurrency(lot.price, lot.currency)}</Badge>
                <Badge tone="ghost" className="gap-1">
                  <Clipboard size={14} />
                  {lot.id.slice(0, 8)}...
                </Badge>
              </div>

              <div className="flex items-center justify-between text-xs text-slate-400">
                <span>Продавец: вы</span>
                <span>Обновлено: {formatDate(lot.updatedAt)}</span>
              </div>

              <div className="flex gap-3">
                <Button className="flex-1" variant="secondary" iconLeft={<CheckCircle2 size={16} />}>
                  Продлить лот
                </Button>
                <Button className="flex-1" iconLeft={<ShoppingCart size={16} />}>
                  Открыть детали
                </Button>
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
}
