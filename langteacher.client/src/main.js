import './assets/main.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import PrimeVue from "primevue/config";
import Aura from "@primeuix/themes/aura"; // Nowy system motywów

import { createApp } from 'vue'
import App from './App.vue'
import router from './router';
import { createPinia } from 'pinia';

const app = createApp(App)

const pinia = createPinia();

app.use(router);
app.use(pinia);

app.use(PrimeVue, {
  theme: {
    preset: Aura, // Ustawienie nowego motywu
    options: {
      prefix: "p", // Prefiks klas CSS
      darkModeSelector: "system", // Tryb ciemny zgodny z systemem
      cssLayer: false // Możesz ustawić `true`, jeśli chcesz używać warstw CSS
    }
  }
});

app.mount('#app');
