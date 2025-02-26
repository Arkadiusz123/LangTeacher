import './assets/main.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

import { createApp } from 'vue'
import App from './App.vue'
import router from './router';
import { createPinia } from 'pinia';

const app = createApp(App)

const pinia = createPinia();

app.use(router);
app.use(pinia);

app.mount('#app');
