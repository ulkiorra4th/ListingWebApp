import type { InputHTMLAttributes, ReactNode } from 'react';
import clsx from 'clsx';

type Props = InputHTMLAttributes<HTMLInputElement> & {
  label?: string;
  error?: string;
  icon?: ReactNode;
};

export function Input({ label, error, icon, className, ...props }: Props) {
  return (
    <label className="flex w-full flex-col gap-1 text-sm text-slate-200">
      {label && <span className="text-xs uppercase tracking-wide text-slate-400">{label}</span>}
      <div
        className={clsx(
          'flex items-center gap-2 rounded-xl border border-white/10 bg-white/5 px-3 py-2 focus-within:border-primary-400 focus-within:bg-white/10',
          error && 'border-red-500/70',
        )}
      >
        {icon}
        <input
          className={clsx('w-full bg-transparent text-sm text-slate-50 outline-none placeholder:text-slate-500', className)}
          {...props}
        />
      </div>
      {error && <span className="text-xs text-red-400">{error}</span>}
    </label>
  );
}
