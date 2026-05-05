/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        ink: 'rgb(var(--color-ink) / <alpha-value>)',
        muted: 'rgb(var(--color-muted) / <alpha-value>)',
        accent: '#ff7a3d',
        surface: 'rgb(var(--color-surface) / <alpha-value>)',
        panel: 'rgba(255, 248, 240, 0.78)',
      },
      fontFamily: {
        sans: ['Space Grotesk', 'Segoe UI', 'sans-serif'],
      },
      boxShadow: {
        card: '0 24px 60px rgba(23, 22, 45, 0.12)',
      },
      borderRadius: {
        xl4: '2rem',
      },
    },
  },
  plugins: [],
}

