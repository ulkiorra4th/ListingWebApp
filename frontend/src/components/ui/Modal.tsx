import type { ReactNode } from 'react';
import { X } from 'lucide-react';
import clsx from 'clsx';

export type ModalProps = {
  open: boolean;
  onClose: () => void;
  title?: string;
  width?: 'sm' | 'md' | 'lg';
  children: ReactNode;
};

export function Modal({ open, onClose, title, width = 'md', children }: ModalProps) {
  if (!open) return null;

  const widthClass = {
    sm: 'max-w-md',
    md: 'max-w-2xl',
    lg: 'max-w-4xl',
  }[width];

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4 backdrop-blur">
      <div className={clsx('glass w-full rounded-3xl border border-white/10 p-6 shadow-2xl', widthClass)}>
        <div className="mb-4 flex items-center justify-between">
          {title && <h3 className="text-lg font-semibold text-white">{title}</h3>}
          <button onClick={onClose} className="text-slate-400 hover:text-white">
            <X size={18} />
          </button>
        </div>
        {children}
      </div>
    </div>
  );
}
