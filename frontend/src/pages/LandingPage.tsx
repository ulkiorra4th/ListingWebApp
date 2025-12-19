import { useEffect, useMemo, useState } from 'react';
import { ArrowRight, CheckCircle2, ImagePlus, ShieldCheck, UserPlus, Wallet2 } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Badge } from '@/components/ui/Badge';
import { useAuth } from '@/hooks/useAuth';
import { useCurrencies } from '@/hooks/useCurrencies';
import { AccountStatus, CountryCode, LanguageCode } from '@/types';

function getErrorMessage(error: unknown) {
  if (error instanceof Error) return error.message;
  if (typeof error === 'string') return error;
  return 'Не удалось выполнить действие';
}

export default function LandingPage() {
  const navigate = useNavigate();
  const {
    authStep,
    account,
    isAuthenticated,
    loading,
    login,
    register,
    verify,
    createProfile,
    uploadAvatar,
    skipAvatar,
    profile,
    createWallet,
    walletReady,
  } = useAuth();
  const { currencies, refresh: refreshCurrencies, loading: currenciesLoading } = useCurrencies(account?.id ?? isAuthenticated);

  const [loginForm, setLoginForm] = useState({ email: '', password: '' });
  const [code, setCode] = useState('');
  const [profileForm, setProfileForm] = useState({
    nickname: '',
    age: 21,
    languageCode: 4, // Russian
    countryCode: 4, // Russia
  });
  const [currencyCode, setCurrencyCode] = useState('');
  const [avatarFile, setAvatarFile] = useState<File | null>(null);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const statusLabel =
    typeof account?.status === 'number' ? AccountStatus[account.status] ?? account?.status : account?.status ?? 'нет';

  useEffect(() => {
    if (isAuthenticated && authStep === 'ready') {
      navigate('/app', { replace: true });
    }
  }, [authStep, isAuthenticated, navigate]);

  useEffect(() => {
    if (!currencyCode && currencies.length) {
      setCurrencyCode(currencies[0].currencyCode);
    }
  }, [currencies, currencyCode]);

  useEffect(() => {
    if (authStep === 'wallet') {
      refreshCurrencies();
    }
  }, [authStep, refreshCurrencies]);

  const handleLogin = async () => {
    setError('');
    setMessage('');
    try {
      await login(loginForm.email, loginForm.password);
      setMessage('Успешный вход. Продолжайте по шагам верификации/профиля, если нужно.');
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  const handleRegister = async () => {
    setError('');
    setMessage('');
    try {
      await register(loginForm.email, loginForm.password);
      setMessage('Регистрация завершена. Проверьте почту и введите код подтверждения.');
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  const handleVerify = async () => {
    setError('');
    setMessage('');
    try {
      await verify(code);
      setMessage('Аккаунт подтвержден. Создайте профиль.');
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  const handleCreateProfile = async () => {
    setError('');
    setMessage('');
    try {
      await createProfile(profileForm);
      setMessage('Профиль создан. При желании загрузите аватар.');
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  const handleUploadAvatar = async () => {
    if (!avatarFile) {
      setError('Выберите файл для загрузки');
      return;
    }
    setError('');
    setMessage('');
    try {
      await uploadAvatar(avatarFile);
      setMessage('Аватар загружен. Можно перейти в маркет.');
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  const handleCreateWallet = async () => {
    if (!currencyCode) {
      setError('Выберите валюту');
      return;
    }
    setError('');
    setMessage('');
    try {
      await createWallet(currencyCode);
      setMessage(`Кошелек в ${currencyCode} готов! Теперь можно перейти в дашборд.`);
    } catch (err) {
      setError(getErrorMessage(err));
    }
  };

  const steps = useMemo(
    () => [
      { key: 'login', label: 'Создать аккаунт', done: authStep !== 'login', icon: <UserPlus size={16} /> },
      { key: 'verify', label: 'Верификация', done: authStep !== 'verify' && authStep !== 'login', icon: <ShieldCheck size={16} /> },
      { key: 'profile', label: 'Профиль', done: authStep === 'avatar' || authStep === 'wallet' || authStep === 'ready', icon: <CheckCircle2 size={16} /> },
      { key: 'avatar', label: 'Аватар (опционально)', done: authStep === 'wallet' || authStep === 'ready' || !!profile?.iconKey, icon: <ImagePlus size={16} /> },
      { key: 'wallet', label: 'Кошелек', done: authStep === 'ready' || walletReady, icon: <Wallet2 size={16} /> },
    ],
    [authStep, profile?.iconKey, walletReady],
  );

  const languageOptions = [
    { label: 'English', value: LanguageCode.English },
    { label: 'French', value: LanguageCode.French },
    { label: 'Italian', value: LanguageCode.Italian },
    { label: 'Spanish', value: LanguageCode.Spanish },
    { label: 'Russian', value: LanguageCode.Russian },
  ];

  const countryOptions = [
    { label: 'England', value: CountryCode.England },
    { label: 'Spain', value: CountryCode.Spain },
    { label: 'France', value: CountryCode.France },
    { label: 'Norway', value: CountryCode.Norway },
    { label: 'Russia', value: CountryCode.Russia },
    { label: 'Italy', value: CountryCode.Italy },
  ];

  return (
    <div className="min-h-screen bg-surface px-4 py-10 text-slate-50">
      <div className="mx-auto flex max-w-3xl flex-col gap-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-gradient-to-br from-primary-500 to-accent-500 text-lg font-bold text-white shadow-glow">
              LM
            </div>
            <div>
              <p className="text-xs uppercase tracking-[0.2em] text-slate-400">Listing Market</p>
              <p className="text-xl font-semibold text-white">Авторизация</p>
            </div>
          </div>
          <Badge tone="info">Шаги: Аккаунт → Верификация → Профиль</Badge>
        </div>

        <Card title="Создайте аккаунт и пройдите шаги" subtitle={`Текущий статус: ${statusLabel}`}>
          <div className="flex flex-wrap items-center gap-3 pb-5">
            {steps.map((step, index) => (
              <div key={step.key} className="flex items-center gap-2">
                <div
                  className={`flex h-10 w-10 items-center justify-center rounded-full border ${
                    step.done ? 'border-primary-400 bg-primary-500/30 text-white' : 'border-white/15 bg-white/5 text-slate-400'
                  }`}
                >
                  {step.icon}
                </div>
                <div>
                  <p className="text-xs uppercase tracking-wide text-slate-400">Шаг {index + 1}</p>
                  <p className="text-sm font-semibold text-white">{step.label}</p>
                </div>
                {index < steps.length - 1 && <div className="w-6 border-t border-dashed border-white/20" />}
              </div>
            ))}
          </div>

          {authStep === 'login' && (
            <div className="flex flex-col gap-3">
              <Input
                label="Email"
                placeholder="you@example.com"
                type="email"
                value={loginForm.email}
                onChange={(e) => setLoginForm((prev) => ({ ...prev, email: e.target.value }))}
              />
              <Input
                label="Пароль"
                placeholder="••••••••"
                type="password"
                value={loginForm.password}
                onChange={(e) => setLoginForm((prev) => ({ ...prev, password: e.target.value }))}
              />
              <div className="flex gap-3">
                <Button onClick={handleRegister} loading={loading} iconRight={<ArrowRight size={16} />}>
                  Создать аккаунт
                </Button>
                <Button variant="secondary" onClick={handleLogin} loading={loading}>
                  Уже есть аккаунт — войти
                </Button>
              </div>
              <p className="text-xs text-slate-400">
                После регистрации проверьте почту: код нужен на следующем шаге. Если вы уже регистрировались — просто войдите и
                перейдите к верификации/профилю.
              </p>
            </div>
          )}

          {authStep === 'verify' && (
            <div className="flex flex-col gap-3">
              <p className="text-sm text-slate-300">На почту отправлен код. Введите его, чтобы подтвердить аккаунт.</p>
              <Input label="Код из письма" placeholder="123456" value={code} onChange={(e) => setCode(e.target.value)} />
              <Button onClick={handleVerify} loading={loading} variant="secondary" iconRight={<ShieldCheck size={16} />}>
                Подтвердить
              </Button>
            </div>
          )}

          {authStep === 'profile' && (
            <div className="flex flex-col gap-3">
              <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
                <Input
                  label="Никнейм"
                  placeholder="RogueTrader"
                  value={profileForm.nickname}
                  onChange={(e) => setProfileForm((prev) => ({ ...prev, nickname: e.target.value }))}
                />
                <Input
                  label="Возраст"
                  type="number"
                  value={profileForm.age}
                  onChange={(e) => setProfileForm((prev) => ({ ...prev, age: Number(e.target.value) }))}
                />
                <Select
                  label="Язык"
                  value={profileForm.languageCode}
                  onChange={(e) => setProfileForm((prev) => ({ ...prev, languageCode: Number(e.target.value) }))}
                >
                  {languageOptions.map((lang) => (
                    <option key={lang.value} value={lang.value}>
                      {lang.label}
                    </option>
                  ))}
                </Select>
                <Select
                  label="Страна"
                  value={profileForm.countryCode}
                  onChange={(e) => setProfileForm((prev) => ({ ...prev, countryCode: Number(e.target.value) }))}
                >
                  {countryOptions.map((country) => (
                    <option key={country.value} value={country.value}>
                      {country.label}
                    </option>
                  ))}
                </Select>
              </div>
              <Button onClick={handleCreateProfile} loading={loading} iconRight={<ArrowRight size={16} />}>
                Создать профиль
              </Button>
            </div>
          )}

          {authStep === 'avatar' && (
            <div className="flex flex-col gap-3">
              <p className="text-sm text-slate-300">Загрузите картинку для профиля или пропустите шаг.</p>
              <Input
                label="Файл"
                type="file"
                accept="image/*"
                onChange={(e) => setAvatarFile(e.target.files?.[0] ?? null)}
              />
              <div className="flex gap-3">
                <Button onClick={handleUploadAvatar} loading={loading} iconRight={<ImagePlus size={16} />}>
                  Загрузить
                </Button>
                <Button variant="ghost" onClick={skipAvatar}>
                  Пропустить
                </Button>
              </div>
            </div>
          )}

          {authStep === 'ready' && (
            <div className="flex flex-col gap-3">
              <p className="text-sm text-slate-300">Вы готовы к торговле.</p>
              <Button onClick={() => navigate('/app')} iconRight={<ArrowRight size={16} />}>
                Перейти в дашборд
              </Button>
            </div>
          )}

          {authStep === 'wallet' && (
            <div className="flex flex-col gap-3">
              <p className="text-sm text-slate-300">Выберите валюту и создайте основной кошелек.</p>
              <Select label="Валюта" value={currencyCode} onChange={(e) => setCurrencyCode(e.target.value)} disabled={currenciesLoading}>
                {currencies.length === 0 && <option value="">Нет доступных валют</option>}
                {currencies.map((c) => (
                  <option key={c.currencyCode} value={c.currencyCode}>
                    {c.currencyCode} — {c.name}
                  </option>
                ))}
              </Select>
              <Button onClick={handleCreateWallet} loading={loading} iconRight={<Wallet2 size={16} />}>
                Создать кошелек
              </Button>
            </div>
          )}

          {message && <p className="mt-3 rounded-xl bg-emerald-500/15 px-3 py-2 text-sm text-emerald-200">{message}</p>}
          {error && <p className="mt-3 rounded-xl bg-red-500/15 px-3 py-2 text-sm text-red-200">{error}</p>}
        </Card>
      </div>
    </div>
  );
}
