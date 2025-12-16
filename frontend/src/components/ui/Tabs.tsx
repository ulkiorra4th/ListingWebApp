import type { ReactNode } from 'react';
import clsx from 'clsx';

type Tab = {
  key: string;
  label: ReactNode;
};

type Props = {
  tabs: Tab[];
  active: string;
  onChange: (key: string) => void;
};

export function Tabs({ tabs, active, onChange }: Props) {
  return (
    <div className="glass flex rounded-2xl border border-white/10 p-1 text-sm">
      {tabs.map((tab) => {
        const isActive = tab.key === active;
        return (
          <button
            key={tab.key}
            onClick={() => onChange(tab.key)}
            className={clsx(
              'flex-1 rounded-xl px-4 py-2 font-semibold transition-all',
              isActive
                ? 'bg-gradient-to-r from-primary-500/80 to-primary-400/80 text-white shadow-glow'
                : 'text-slate-300 hover:text-white hover:bg-white/5',
            )}
          >
            {tab.label}
          </button>
        );
      })}
    </div>
  );
}
