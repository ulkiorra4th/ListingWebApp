import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AuthProvider, useAuth } from '@/hooks/useAuth';
import AppLayout from '@/pages/AppLayout';
import DashboardPage from '@/pages/DashboardPage';
import InventoryPage from '@/pages/InventoryPage';
import LandingPage from '@/pages/LandingPage';
import MarketplacePage from '@/pages/MarketplacePage';
import MyListingsPage from '@/pages/MyListingsPage';

function ProtectedRoute({ children }: { children: JSX.Element }) {
  const { isAuthenticated, authStep } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  if (authStep !== 'ready') {
    return <Navigate to="/" replace />;
  }

  return children;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route
            path="/app/*"
            element={
              <ProtectedRoute>
                <AppLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<MarketplacePage />} />
            <Route path="inventory" element={<InventoryPage />} />
            <Route path="listings" element={<MyListingsPage />} />
            <Route path="tools" element={<DashboardPage embed />} />
            <Route path="*" element={<Navigate to="/app" replace />} />
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
