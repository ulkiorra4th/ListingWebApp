import type { ReactNode } from 'react';
import clsx from 'clsx';

type Props = {
  children: ReactNode;
  tone?: 'neutral' | 'success' | 'warning' | 'info';
  className?: string;
};

export function Badge({ children, tone = 'neutral', className }: Props) {
  const toneMap: Record<NonNullable<Props['tone']>, string> = {
    neutral: 'bg-white/10 text-slate-100',
    success: 'bg-emerald-500/20 text-emerald-200 border border-emerald-500/30',
    warning: 'bg-amber-500/20 text-amber-100 border border-amber-500/30',
    info: 'bg-primary-500/15 text-primary-100 border border-primary-400/30',
  };

  return (
    <span
      className={clsx(
        'inline-flex items-center gap-2 rounded-full px-3 py-1 text-xs font-semibold uppercase tracking-wide',
        toneMap[tone],
        className,
      )}
    >
      {children}
    </span>
  );
}
