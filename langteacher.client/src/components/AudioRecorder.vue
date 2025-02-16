<template>
  <div>
    <h2>Rozpoznawanie Mowy</h2>

    <button @click="startRecognition" :disabled="isRecognizing">🎤 Start</button>
    <button @click="stopRecognition" :disabled="!isRecognizing">🛑 Stop</button>

    <p v-if="recognizedText"><strong>Rozpoznany tekst:</strong> {{ recognizedText }}</p>

    <button v-if="recognizedText" @click="sendTextToBackend">📤 Wyślij tekst</button>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import axios from 'axios';

  // Zmienne reaktywne
  const recognizedText = ref('');
  const isRecognizing = ref(false);
  let recognition = null;

  // ✅ Sprawdzenie, czy przeglądarka obsługuje Web Speech API
  if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    recognition = new SpeechRecognition();

    // 🔄 Ustawienia rozpoznawania mowy
    recognition.lang = 'pl-PL'; // 🇵🇱 Język polski
    recognition.continuous = false; // Czy ma działać w pętli?
    recognition.interimResults = false; // Czy ma zwracać częściowe wyniki?

    // 🎤 Gdy zostanie wykryta mowa
    recognition.onresult = (event) => {
      recognizedText.value = event.results[0][0].transcript; // Pobranie tekstu
    };

    // 🔄 Gdy rozpoznawanie się zatrzyma
    recognition.onend = () => {
      isRecognizing.value = false;
    };
  } else {
    console.error('❌ Web Speech API nie jest obsługiwane w tej przeglądarce.');
  }

  // 🎤 Start rozpoznawania mowy
  const startRecognition = () => {
    if (recognition) {
      recognizedText.value = '';
      isRecognizing.value = true;
      recognition.start();
    }
  };

  // 🛑 Stop rozpoznawania
  const stopRecognition = () => {
    if (recognition) {
      recognition.stop();
      isRecognizing.value = false;
    }
  };

  // 📤 Wysłanie tekstu na backend
  const sendTextToBackend = async () => {
    try {
      const response = await axios.post('/api/conversations', { text: recognizedText.value });
      console.log('Tekst wysłany:', response.data);
    } catch (error) {
      console.error('Błąd wysyłania tekstu:', error);
    }
  };
</script>

<style scoped>
  button {
    margin: 10px;
  }
</style>
