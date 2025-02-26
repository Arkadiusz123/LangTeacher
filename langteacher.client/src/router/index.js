import { createRouter, createWebHistory } from 'vue-router';

import AudioRecorder from '../components/AudioRecorder.vue'


// Konfiguracja tras (routes)
const routes = [
  {
    path: '/audio-recorder',
    name: 'audioRecorder',
    component: AudioRecorder,
  },
];

// Utwórz router
const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
