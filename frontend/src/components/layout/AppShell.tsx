import type { ReactNode } from 'react';
import { Header } from './Header';

export function AppShell({
  children,
  profileName,
  email,
  walletLabel,
  avatarUrl,
  onLogout,
}: {
  children: ReactNode;
  profileName?: string;
  email?: string;
  walletLabel?: string;
  avatarUrl?: string | null;
  onLogout: () => void;
}) {
  return (
    <div className="min-h-screen bg-surface px-5 py-6 text-slate-50">
      <div className="mx-auto flex max-w-6xl flex-col gap-5">
        <Header profileName={profileName} email={email} walletLabel={walletLabel} avatarUrl={avatarUrl} onLogout={onLogout} />
        <main className="flex flex-col gap-5">{children}</main>
      </div>
    </div>
  );
}
