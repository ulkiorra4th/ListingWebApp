import type { ReactNode } from 'react';
import clsx from 'clsx';

type Props = {
  title?: ReactNode;
  subtitle?: ReactNode;
  actions?: ReactNode;
  children: ReactNode;
  className?: string;
};

export function Card({ title, subtitle, actions, children, className }: Props) {
  return (
    <div className={clsx('glass card-hover rounded-2xl border border-white/10 p-5 shadow-xl', className)}>
      {(title || actions || subtitle) && (
        <div className="mb-4 flex items-start justify-between gap-3">
          <div>
            {title && <h3 className="text-lg font-semibold text-white">{title}</h3>}
            {subtitle && <p className="text-sm text-slate-400">{subtitle}</p>}
          </div>
          {actions}
        </div>
      )}
      {children}
    </div>
  );
}
