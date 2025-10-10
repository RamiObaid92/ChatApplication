import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(),
    tailwindcss()
  ],
  server: { 
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:7277',
        secure: false,
      },
      '/chathub': {
        target: 'https://localhost:7277',
        secure: false,
        ws: true,
      }
    }
  }
});
