import type { ReactNode } from 'react';
import clsx from 'clsx';

type Props = {
  label: string;
  icon?: ReactNode;
  size?: 'sm' | 'md' | 'lg';
};

export function Avatar({ label, icon, size = 'md' }: Props) {
  const sizes: Record<NonNullable<Props['size']>, string> = {
    sm: 'h-8 w-8 text-sm',
    md: 'h-11 w-11 text-base',
    lg: 'h-14 w-14 text-lg',
  };

  const initials = label
    .split(' ')
    .map((s) => s.charAt(0).toUpperCase())
    .slice(0, 2)
    .join('');

  return (
    <div
      className={clsx(
        'flex items-center justify-center rounded-full bg-gradient-to-br from-primary-400/80 to-accent-400/80 text-white shadow-glow',
        sizes[size],
      )}
    >
      {icon ?? initials}
    </div>
  );
}
