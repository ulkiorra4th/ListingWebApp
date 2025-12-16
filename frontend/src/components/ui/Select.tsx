import type { SelectHTMLAttributes } from 'react';
import clsx from 'clsx';

type Props = SelectHTMLAttributes<HTMLSelectElement> & {
  label?: string;
  error?: string;
};

export function Select({ label, error, className, children, ...props }: Props) {
  return (
    <label className="flex w-full flex-col gap-1 text-sm text-slate-200">
      {label && <span className="text-xs uppercase tracking-wide text-slate-400">{label}</span>}
      <select
        className={clsx(
          'rounded-xl border border-white/10 bg-white/5 px-3 py-2 text-sm text-slate-50 outline-none focus:border-primary-400 focus:bg-white/10',
          error && 'border-red-500/70',
          className,
        )}
        {...props}
      >
        {children}
      </select>
      {error && <span className="text-xs text-red-400">{error}</span>}
    </label>
  );
}
