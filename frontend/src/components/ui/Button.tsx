import type { ButtonHTMLAttributes, ReactNode } from 'react';
import clsx from 'clsx';

export type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  loading?: boolean;
  iconLeft?: ReactNode;
  iconRight?: ReactNode;
};

export function Button({
  variant = 'primary',
  size = 'md',
  loading = false,
  iconLeft,
  iconRight,
  className,
  children,
  disabled,
  ...props
}: ButtonProps) {
  const base = 'inline-flex items-center justify-center rounded-xl font-semibold transition-all focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary-400';
  const variants: Record<NonNullable<ButtonProps['variant']>, string> = {
    primary:
      'bg-gradient-to-r from-primary-500 to-primary-400 text-white shadow-glow hover:shadow-lg hover:-translate-y-0.5 disabled:opacity-60 disabled:cursor-not-allowed',
    secondary:
      'bg-white/5 border border-white/10 text-white hover:border-primary-300/40 hover:bg-primary-500/10 disabled:opacity-60 disabled:cursor-not-allowed',
    ghost:
      'bg-transparent text-slate-200 hover:bg-white/5 border border-transparent hover:border-white/10 disabled:opacity-60 disabled:cursor-not-allowed',
    danger:
      'bg-red-600/90 text-white hover:bg-red-600 disabled:opacity-60 disabled:cursor-not-allowed',
  };

  const sizes: Record<NonNullable<ButtonProps['size']>, string> = {
    sm: 'px-3 py-1.5 text-sm gap-2',
    md: 'px-4 py-2 text-sm gap-2',
    lg: 'px-5 py-3 text-base gap-3',
  };

  return (
    <button
      className={clsx(base, variants[variant], sizes[size], className)}
      disabled={disabled || loading}
      {...props}
    >
      {loading && <span className="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-white/20 border-t-white" />}
      {iconLeft && <span className="mr-2">{iconLeft}</span>}
      {children}
      {iconRight && <span className="ml-2">{iconRight}</span>}
    </button>
  );
}
