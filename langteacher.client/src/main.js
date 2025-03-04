import './assets/main.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import DataTable from 'datatables.net-vue3';
import DataTablesCore from 'datatables.net-bs5';
import 'datatables.net-select-bs5';
import 'datatables.net-responsive-bs5';

import { createApp } from 'vue'
import App from './App.vue'
import router from './router';
import { createPinia } from 'pinia';

const app = createApp(App)

const pinia = createPinia();

app.use(router);
app.use(pinia);

DataTable.use(DataTablesCore);
app.use(DataTable);

app.mount('#app');
