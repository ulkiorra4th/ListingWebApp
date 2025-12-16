import { useEffect, useMemo, useState } from 'react';
import { ArrowRight, Banknote, CheckCircle2, ClipboardCheck, Coins, CreditCard, ShoppingCart, Wallet2 } from 'lucide-react';
import { AppShell } from '@/components/layout/AppShell';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Badge } from '@/components/ui/Badge';
import { useAuth } from '@/hooks/useAuth';
import { useCurrencies } from '@/hooks/useCurrencies';
import { useWallet } from '@/hooks/useWallet';
import { useItems } from '@/hooks/useItems';
import { useListings } from '@/hooks/useListings';
import { useTransactions } from '@/hooks/useTransactions';
import { formatCurrency, formatDate, rarityColor } from '@/utils/formatters';
import { AccountStatus, ItemRarity, ListingStatus } from '@/types';

export default function DashboardPage({ embed = false }: { embed?: boolean } = {}) {
  const { profile, account, accountId, logout } = useAuth();
  const { currencies } = useCurrencies();
  const [currencyCode, setCurrencyCode] = useState<string | null>(null);
  const { wallet, credit, debit, refresh: refreshWallet, loading: walletLoading } = useWallet(accountId, currencyCode);

  const { item, entry, fetchItem, fetchEntry, createEntry, loading: itemsLoading } = useItems();
  const { listing, purchaseResult, create: createListing, fetchListing, purchase, loading: listingsLoading } = useListings();
  const { transaction, fetchTransaction, loading: txLoading } = useTransactions();

  const [feedback, setFeedback] = useState('');
  const [error, setError] = useState('');

  const [walletForm, setWalletForm] = useState({ credit: 100, debit: 50 });
  const [itemForm, setItemForm] = useState({ itemTypeId: '', pseudonym: '' });
  const [itemIdInput, setItemIdInput] = useState('');
  const [entryIdInput, setEntryIdInput] = useState('');

  const [listingForm, setListingForm] = useState({
    itemEntryId: '',
    priceAmount: 100,
    status: ListingStatus.Approved,
    currencyCode: '',
  });
  const [listingIdInput, setListingIdInput] = useState('');
  const [purchaseIdInput, setPurchaseIdInput] = useState('');

  const [txIdInput, setTxIdInput] = useState('');

  useEffect(() => {
    if (!currencyCode && currencies.length) {
      const code = currencies[0].currencyCode;
      setCurrencyCode(code);
      setListingForm((prev) => ({ ...prev, currencyCode: prev.currencyCode || code }));
    }
  }, [currencies, currencyCode]);

  const listingStatusOptions = useMemo(
    () => [
      { label: 'Draft', value: ListingStatus.Draft },
      { label: 'Pending', value: ListingStatus.Pending },
      { label: 'Approved', value: ListingStatus.Approved },
      { label: 'Rejected', value: ListingStatus.Rejected },
      { label: 'Closed', value: ListingStatus.Closed },
    ],
    [],
  );

  const rarityLabel = (rarity?: number) => {
    if (rarity === ItemRarity.Legendary) return 'Legendary';
    if (rarity === ItemRarity.Epic) return 'Epic';
    if (rarity === ItemRarity.Rare) return 'Rare';
    if (rarity === ItemRarity.Common) return 'Common';
    return '—';
  };

  const accountStatusLabel = useMemo(() => {
    if (account?.status === undefined || account?.status === null) return '—';
    if (typeof account.status === 'number') {
      const label = (AccountStatus as Record<number, string>)[account.status];
      return label ?? account.status;
    }
    return account.status;
  }, [account?.status]);

  const handleCredit = async () => {
    if (!accountId || !currencyCode) return;
    setError('');
    setFeedback('');
    try {
      await credit(walletForm.credit);
      setFeedback('Баланс пополнен');
      await refreshWallet();
    } catch (err) {
      setError((err as Error).message);
    }
  };

  const handleDebit = async () => {
    if (!accountId || !currencyCode) return;
    setError('');
    setFeedback('');
    try {
      await debit(walletForm.debit);
      setFeedback('Средства списаны');
      await refreshWallet();
    } catch (err) {
      setError((err as Error).message);
    }
  };

  const handleCreateEntry = async () => {
    if (!accountId) return;
    setError('');
    setFeedback('');
    try {
      const id = await createEntry(accountId, itemForm.itemTypeId, itemForm.pseudonym || undefined);
      setFeedback(`Создан item-entry ${id}`);
      setListingForm((prev) => ({ ...prev, itemEntryId: id }));
      setEntryIdInput(id);
    } catch (err) {
      setError((err as Error).message);
    }
  };

  const handleCreateListing = async () => {
    if (!accountId) return;
    setError('');
    setFeedback('');
    try {
      const selectedCurrency = currencyCode || listingForm.currencyCode || currencies[0]?.currencyCode || '';
      const created = await createListing({
        sellerId: accountId,
        itemEntryId: listingForm.itemEntryId,
        currencyCode: selectedCurrency,
        priceAmount: listingForm.priceAmount,
        status: listingForm.status,
      });
      setFeedback('Лот создан');
      if (created?.id) {
        setListingIdInput(created.id);
      }
    } catch (err) {
      setError((err as Error).message);
    }
  };

  const handleFetchListing = async () => {
    setError('');
    setFeedback('');
    try {
      await fetchListing(listingIdInput);
      setFeedback('Лот загружен');
    } catch (err) {
      setError((err as Error).message);
    }
  };

  const handlePurchase = async () => {
    setError('');
    setFeedback('');
    try {
      const tx = await purchase(purchaseIdInput);
      setFeedback(`Покупка завершена. Транзакция ${tx.id}`);
      setTxIdInput(tx.id);
      await refreshWallet();
    } catch (err) {
      setError((err as Error).message);
    }
  };

  const handleFetchTx = async () => {
    setError('');
    setFeedback('');
    try {
      await fetchTransaction(txIdInput);
      setFeedback('Транзакция загружена');
    } catch (err) {
      setError((err as Error).message);
    }
  };

  const content = (
    <>
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
        <Card title="Аккаунт" className="lg:col-span-1">
          <div className="flex flex-col gap-2 text-sm text-slate-300">
            <div className="flex items-center justify-between">
              <span>Email</span>
              <span className="text-white">{account?.email ?? '—'}</span>
            </div>
            <div className="flex items-center justify-between">
              <span>Статус</span>
              <Badge tone="info">{accountStatusLabel}</Badge>
            </div>
            <div className="flex items-center justify-between">
              <span>Профиль</span>
              <span className="text-white">{profile?.nickname ?? '—'}</span>
            </div>
            {profile?.iconKey && (
              <div className="flex items-center justify-between">
                <span>Аватар</span>
                <span className="text-primary-200">{profile.iconKey}</span>
              </div>
            )}
          </div>
        </Card>

        <Card title="Валюты" subtitle="Список с бэкенда" className="lg:col-span-2">
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
            {currencies.map((c) => (
              <div key={c.currencyCode} className="rounded-2xl border border-white/10 bg-white/5 p-3">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <span className="text-lg font-semibold text-white">{c.currencyCode}</span>
                    {c.iconKey && <span className="text-xl">{c.iconKey}</span>}
                  </div>
                  <Badge tone="info">{c.isTransferAllowed ? 'transfer' : 'locked'}</Badge>
                </div>
                <p className="text-sm text-slate-300">{c.name}</p>
                {c.description && <p className="text-xs text-slate-500">{c.description}</p>}
              </div>
            ))}
          </div>
        </Card>
      </div>

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <Card title="Кошелек" subtitle="Получение/пополнение/списание">
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
            <Select label="Валюта" value={currencyCode ?? ''} onChange={(e) => setCurrencyCode(e.target.value)}>
              {currencies.map((c) => (
                <option key={c.currencyCode} value={c.currencyCode}>
                  {c.currencyCode} — {c.name}
                </option>
              ))}
            </Select>
            <Button variant="secondary" onClick={refreshWallet} loading={walletLoading} iconLeft={<Wallet2 size={16} />}>
              Обновить кошелек
            </Button>
          </div>
          <div className="mt-3 rounded-2xl bg-white/5 p-3">
            <p className="text-sm text-slate-400">Баланс</p>
            <p className="text-2xl font-semibold text-white">
              {wallet ? formatCurrency(wallet.balance, wallet.currencyCode) : '—'}
            </p>
            <p className="text-xs text-slate-500">
              Последняя транзакция: {wallet?.lastTransactionDate ? formatDate(wallet.lastTransactionDate) : '—'}
            </p>
          </div>
          <div className="mt-4 grid grid-cols-1 gap-3 md:grid-cols-2">
            <div className="rounded-2xl border border-white/10 bg-white/5 p-3">
              <Input
                label="Пополнить"
                type="number"
                value={walletForm.credit}
                onChange={(e) => setWalletForm((prev) => ({ ...prev, credit: Number(e.target.value) }))}
              />
              <Button className="mt-2" onClick={handleCredit} loading={walletLoading} iconLeft={<Coins size={16} />}>
                Пополнить
              </Button>
            </div>
            <div className="rounded-2xl border border-white/10 bg-white/5 p-3">
              <Input
                label="Списать"
                type="number"
                value={walletForm.debit}
                onChange={(e) => setWalletForm((prev) => ({ ...prev, debit: Number(e.target.value) }))}
              />
              <Button className="mt-2" variant="secondary" onClick={handleDebit} loading={walletLoading} iconLeft={<Banknote size={16} />}>
                Списать
              </Button>
            </div>
          </div>
        </Card>

        <Card title="Item / Entry" subtitle="Получить предмет или создать запись">
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
            <Input label="Item Id" placeholder="GUID" value={itemIdInput} onChange={(e) => setItemIdInput(e.target.value)} />
            <Button variant="secondary" onClick={() => fetchItem(itemIdInput)} loading={itemsLoading} iconLeft={<ClipboardCheck size={16} />}>
              Получить item
            </Button>
          </div>
          {item && (
            <div className="mt-3 rounded-2xl bg-white/5 p-3 text-sm text-slate-200">
              <div className="flex items-center justify-between">
                <span className="text-white">{item.name}</span>
                <span className={`rounded-full px-3 py-1 text-xs font-semibold text-white bg-gradient-to-r ${rarityColor(rarityLabel(item.rarity))}`}>
                  {rarityLabel(item.rarity)}
                </span>
              </div>
              <p className="text-xs text-slate-400">Base price: {formatCurrency(item.basePrice)}</p>
            </div>
          )}

          <div className="mt-4 grid grid-cols-1 gap-3 md:grid-cols-2">
            <Input
              label="ItemType Id для entry"
              placeholder="GUID"
              value={itemForm.itemTypeId}
              onChange={(e) => setItemForm((prev) => ({ ...prev, itemTypeId: e.target.value }))}
            />
            <Input
              label="Псевдоним"
              placeholder="Отображаемое имя"
              value={itemForm.pseudonym}
              onChange={(e) => setItemForm((prev) => ({ ...prev, pseudonym: e.target.value }))}
            />
            <Button onClick={handleCreateEntry} loading={itemsLoading} iconLeft={<CheckCircle2 size={16} />}>
              Создать entry
            </Button>
          </div>

          <div className="mt-4 grid grid-cols-1 gap-3 md:grid-cols-2">
            <Input label="Получить entry" placeholder="GUID" value={entryIdInput} onChange={(e) => setEntryIdInput(e.target.value)} />
            <Button variant="secondary" onClick={() => fetchEntry(entryIdInput)} loading={itemsLoading}>
              Загрузить entry
            </Button>
          </div>

          {entry && (
            <div className="mt-3 rounded-2xl bg-white/5 p-3 text-sm text-slate-200">
              <p className="text-white">Entry: {entry.id}</p>
              <p className="text-xs text-slate-400">Owner: {entry.ownerId}</p>
              <p className="text-xs text-slate-400">ItemType: {entry.itemTypeId}</p>
            </div>
          )}
        </Card>
      </div>

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <Card title="Лоты" subtitle="Создать / получить / купить">
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
            <Input
              label="ItemEntry Id"
              placeholder="GUID"
              value={listingForm.itemEntryId}
              onChange={(e) => setListingForm((prev) => ({ ...prev, itemEntryId: e.target.value }))}
            />
            <Select
              label="Валюта"
              value={currencyCode ?? listingForm.currencyCode}
              onChange={(e) => {
                setCurrencyCode(e.target.value);
                setListingForm((prev) => ({ ...prev, currencyCode: e.target.value }));
              }}
            >
              {currencies.map((c) => (
                <option key={c.currencyCode} value={c.currencyCode}>
                  {c.currencyCode}
                </option>
              ))}
            </Select>
            <Input
              label="Цена"
              type="number"
              value={listingForm.priceAmount}
              onChange={(e) => setListingForm((prev) => ({ ...prev, priceAmount: Number(e.target.value) }))}
            />
            <Select
              label="Статус"
              value={listingForm.status}
              onChange={(e) => setListingForm((prev) => ({ ...prev, status: Number(e.target.value) }))}
            >
              {listingStatusOptions.map((s) => (
                <option key={s.value} value={s.value}>
                  {s.label}
                </option>
              ))}
            </Select>
          </div>
          <Button className="mt-3" onClick={handleCreateListing} loading={listingsLoading} iconLeft={<ArrowRight size={16} />}>
            Создать лот
          </Button>

          <div className="mt-4 grid grid-cols-1 gap-3 md:grid-cols-2">
            <Input label="ID лота" placeholder="GUID" value={listingIdInput} onChange={(e) => setListingIdInput(e.target.value)} />
            <Button variant="secondary" onClick={handleFetchListing} loading={listingsLoading}>
              Загрузить лот
            </Button>
          </div>

          <div className="mt-3 grid grid-cols-1 gap-3 md:grid-cols-2">
            <Input label="Купить лот" placeholder="GUID" value={purchaseIdInput} onChange={(e) => setPurchaseIdInput(e.target.value)} />
            <Button variant="secondary" onClick={handlePurchase} loading={listingsLoading} iconLeft={<ShoppingCart size={16} />}>
              Купить
            </Button>
          </div>

          {listing && (
            <div className="mt-4 rounded-2xl bg-white/5 p-3 text-sm text-slate-200">
              <div className="flex items-center justify-between">
                <p className="text-white">{listing.id}</p>
                <Badge tone="info">{ListingStatus[listing.status as ListingStatus] ?? listing.status}</Badge>
              </div>
              <p className="text-xs text-slate-400">Цена: {formatCurrency(Number(listing.priceAmount), listing.currencyCode)}</p>
              <p className="text-xs text-slate-400">ItemEntry: {listing.itemEntryId}</p>
              <p className="text-xs text-slate-500">Создан: {formatDate(listing.createdAt)}</p>
            </div>
          )}

          {purchaseResult && (
            <div className="mt-3 rounded-2xl border border-emerald-500/30 bg-emerald-500/10 p-3 text-sm text-emerald-100">
              Покупка успешно выполнена. Транзакция {purchaseResult.id}
            </div>
          )}
        </Card>

        <Card title="Транзакции" subtitle="Получить по ID">
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
            <Input label="ID транзакции" placeholder="GUID" value={txIdInput} onChange={(e) => setTxIdInput(e.target.value)} />
            <Button variant="secondary" onClick={handleFetchTx} loading={txLoading} iconLeft={<CreditCard size={16} />}>
              Загрузить
            </Button>
          </div>
          {transaction && (
            <div className="mt-4 rounded-2xl bg-white/5 p-3 text-sm text-slate-200">
              <p className="text-white">{transaction.id}</p>
              <p className="text-xs text-slate-400">Listing: {transaction.listingId}</p>
              <p className="text-xs text-slate-400">Покупатель: {transaction.buyerAccountId}</p>
              <p className="text-xs text-slate-400">Продавец: {transaction.sellerAccountId}</p>
              <p className="text-xs text-slate-400">Сумма: {formatCurrency(Number(transaction.amount), transaction.currencyCode)}</p>
              <p className="text-xs text-slate-500">Дата: {formatDate(transaction.transactionDate)}</p>
            </div>
          )}
        </Card>
      </div>

      {feedback && <div className="rounded-2xl border border-emerald-500/30 bg-emerald-500/10 p-3 text-sm text-emerald-100">{feedback}</div>}
      {error && <div className="rounded-2xl border border-red-500/30 bg-red-500/10 p-3 text-sm text-red-100">{error}</div>}
    </>
  );

  if (embed) {
    return <div className="flex flex-col gap-5">{content}</div>;
  }

  return (
    <AppShell
      profileName={profile?.nickname ?? 'Трейдер'}
      email={account?.email}
      walletLabel={wallet ? formatCurrency(wallet.balance, wallet.currencyCode) : undefined}
      onLogout={logout}
    >
      {content}
    </AppShell>
  );
}
