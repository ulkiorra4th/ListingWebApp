export function formatCurrency(amount: number, currencyCode?: string) {
  const rounded = Math.round(amount * 100) / 100;
  if (!currencyCode) return rounded.toLocaleString('ru-RU', { maximumFractionDigits: 2 });
  return `${rounded.toLocaleString('ru-RU', { maximumFractionDigits: 2 })} ${currencyCode}`;
}

export function formatDate(value: string | number | Date) {
  const date = value instanceof Date ? value : new Date(value);
  return date.toLocaleString('ru-RU', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function rarityColor(rarity: string) {
  switch (rarity) {
    case 'Legendary':
      return 'from-orange-500 to-amber-400';
    case 'Epic':
      return 'from-fuchsia-500 to-purple-500';
    case 'Rare':
      return 'from-sky-500 to-cyan-400';
    default:
      return 'from-slate-500 to-slate-400';
  }
}
