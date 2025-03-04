import { createRouter, createWebHistory } from 'vue-router';

import AudioRecorder from '../components/AudioRecorder.vue'
import CoversationsList from '../components/CoversationsList.vue'


// Konfiguracja tras (routes)
const routes = [
  {
    path: '/audio-recorder',
    name: 'audioRecorder',
    component: AudioRecorder,
  },
  {
    path: '/coversations-list',
    name: 'coversationsList',
    component: CoversationsList,
  },
];

// Utwórz router
const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
