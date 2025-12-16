import clsx from 'clsx';
import { Boxes, Sparkles, Tags, Wrench } from 'lucide-react';
import { NavLink, Outlet } from 'react-router-dom';
import { AppShell } from '@/components/layout/AppShell';
import { useAuth } from '@/hooks/useAuth';

const navItems = [
  { to: '/app', label: 'Открытые лоты', icon: <Sparkles size={16} />, end: true },
  { to: '/app/inventory', label: 'Мои предметы', icon: <Boxes size={16} /> },
  { to: '/app/listings', label: 'Мои лоты', icon: <Tags size={16} /> },
  { to: '/app/tools', label: 'Сервисные формы', icon: <Wrench size={16} /> },
];

export default function AppLayout() {
  const { profile, account, logout } = useAuth();

  return (
    <AppShell profileName={profile?.nickname} email={account?.email} onLogout={logout}>
      <nav className="flex flex-wrap gap-2 rounded-2xl border border-white/10 bg-white/5 p-2">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            end={item.end}
            className={({ isActive }) =>
              clsx(
                'flex items-center gap-2 rounded-xl px-4 py-2 text-sm font-semibold transition-all',
                isActive
                  ? 'bg-gradient-to-r from-primary-500/80 to-primary-400/80 text-white shadow-glow'
                  : 'text-slate-300 hover:text-white hover:bg-white/5',
              )
            }
          >
            {item.icon}
            <span>{item.label}</span>
          </NavLink>
        ))}
      </nav>

      <Outlet />
    </AppShell>
  );
}
