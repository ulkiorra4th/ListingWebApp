const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
  content: ['./index.html', './src/**/*.{ts,tsx,js,jsx}'],
  theme: {
    extend: {
      fontFamily: {
        display: ['"Space Grotesk"', 'Inter', ...defaultTheme.fontFamily.sans],
        sans: ['"Manrope"', 'Inter', ...defaultTheme.fontFamily.sans],
      },
      colors: {
        primary: {
          50: '#e9fbff',
          100: '#c8f3ff',
          200: '#9ae5ff',
          300: '#63d2ff',
          400: '#2bb5f7',
          500: '#0e95dd',
          600: '#0a74b1',
          700: '#095d8f',
          800: '#0a4a70',
          900: '#0c3d5d',
        },
        accent: {
          50: '#fff8e7',
          100: '#ffeac2',
          200: '#ffd58a',
          300: '#ffba4d',
          400: '#ff9e1a',
          500: '#f87f00',
          600: '#d26000',
          700: '#a54102',
          800: '#84330a',
          900: '#6c2b0d',
        },
        surface: '#0f1725',
        'surface-muted': '#1b2435',
      },
      boxShadow: {
        glow: '0 10px 50px rgba(14, 149, 221, 0.25)',
      },
    },
  },
  plugins: [],
};
