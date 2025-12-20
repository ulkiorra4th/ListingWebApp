import { LogOut, Wallet2 } from 'lucide-react';
import { Button } from '@/components/ui/Button';

export function Header({
  profileName,
  email,
  walletLabel,
  avatarUrl,
  onLogout,
}: {
  profileName?: string;
  email?: string;
  walletLabel?: string;
  avatarUrl?: string | null;
  onLogout: () => void;
}) {
  return (
    <header className="sticky top-0 z-20 flex items-center justify-between rounded-2xl border border-white/10 bg-surface/80 p-4 backdrop-blur">
      <div className="flex items-center gap-3">
        <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br from-primary-500 to-accent-500 text-lg font-bold text-white shadow-glow">
          LM
        </div>
        <div>
          <p className="text-sm uppercase tracking-[0.2em] text-slate-400">Listing Market</p>
          <p className="text-lg font-semibold text-white">Игровая торговая площадка</p>
        </div>
      </div>

      <div className="flex items-center gap-3">
        {walletLabel && (
          <div className="flex items-center gap-2 rounded-full bg-white/5 px-3 py-2 text-sm text-slate-100">
            <Wallet2 size={18} className="text-primary-200" />
            <span>{walletLabel}</span>
          </div>
        )}
        <div className="flex items-center gap-3">
          <div className="flex h-12 w-12 items-center justify-center overflow-hidden rounded-2xl border border-white/10 bg-white/5 shadow-inner">
            {avatarUrl ? (
              <img src={avatarUrl} alt="Аватар" className="h-full w-full object-cover" />
            ) : (
              <div className="flex h-full w-full items-center justify-center bg-gradient-to-br from-primary-500/50 to-accent-500/50 text-white">
                {profileName?.slice(0, 1).toUpperCase() ?? 'LM'}
              </div>
            )}
          </div>
          <div className="flex flex-col text-right">
            <span className="text-sm font-semibold text-white">{profileName ?? 'Гость'}</span>
            <span className="text-xs text-slate-400">{email ?? 'Не авторизован'}</span>
          </div>
        </div>
        <Button variant="ghost" size="sm" onClick={onLogout} iconLeft={<LogOut size={16} />}>
          Выйти
        </Button>
      </div>
    </header>
  );
}
