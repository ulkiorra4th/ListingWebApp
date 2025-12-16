import type { ReactNode } from 'react';

export function StatPill({ title, value, icon }: { title: string; value: ReactNode; icon?: ReactNode }) {
  return (
    <div className="glass card-hover flex items-center justify-between rounded-2xl border border-white/10 px-4 py-3">
      <div>
        <p className="text-xs uppercase tracking-wide text-slate-400">{title}</p>
        <p className="text-xl font-semibold text-white">{value}</p>
      </div>
      {icon && <div className="text-primary-200">{icon}</div>}
    </div>
  );
}
