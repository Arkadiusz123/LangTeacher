<template>
  <div class="container mt-4">
    <div class="card p-4 shadow-sm">
      <h2 class="text-center">Speech Recognition</h2>

      <div class="mb-3">
        <label for="language" class="form-label">Select language:</label>
        <select v-model="selectedLanguage" class="form-select">
          <option value="pl-PL">🇵🇱 Polski</option>
          <option value="en-US">🇬🇧 English</option>
        </select>
      </div>

      <div class="mb-3">
        <label for="conversation" class="form-label">Select conversation:</label>
        <select v-model="conversationId" class="form-select">
          <option :value="null">Empty</option>
          <option v-for="conversation in conversationStore.conversations" :value="conversation.conversationId" :key="conversation.conversationId">
            {{ formatDate(conversation.lastMessageDate) + ': ' +  conversation.title }}
          </option>
        </select>
      </div>

      <div class="d-flex justify-content-center gap-2 mb-3">
        <button @click="startRecognition" class="btn btn-primary" :disabled="isRecognizing">🎤 Start</button>
        <button @click="stopRecognition" class="btn btn-danger" :disabled="!isRecognizing">🛑 Stop</button>
        <button @click="displayResponse = !displayResponse" class="btn btn-secondary" :disabled="!serverResponse">
          {{ displayResponse ? 'Hide response' : 'Display response' }}
        </button>
      </div>

      <div v-if="recognizedText" class="alert alert-info">
        <strong>Recognized text:</strong> {{ recognizedText }}
      </div>

      <div class="d-flex justify-content-center gap-2" v-if="recognizedText">
        <button @click="sendTextToBackend" class="btn btn-success" :disabled="isSending">📤 Send text</button>
      </div>

      <div v-if="serverResponse">
        <div v-show="displayResponse" class="alert alert-success mt-3">
          <strong>Answer:</strong> {{ serverResponse }}
        </div>
        <div class="text-center mt-2">
          <button @click="speakResponse" class="btn btn-warning">🔊 Read answer</button>
          <button @click="stopReading" class="btn btn-warning"> Stop reading</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import { useConversationStore } from '../services/useConversations';
  import { useDateFormat } from '@vueuse/core';

  // Zmienne reaktywne
  const recognizedText = ref('');
  const isRecognizing = ref(false);
  const isSending = ref(false);
  const displayResponse = ref(false);
  const conversationId = ref(null);
  const serverResponse = ref('');
  const selectedLanguage = ref('en-US'); // Domyślnie angielski
  let recognition = null;

  const conversationStore = useConversationStore();


  onMounted(() => {
    conversationStore.getConversations();

    // ✅ Sprawdzenie, czy przeglądarka obsługuje Web Speech API
    if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
      const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
      recognition = new SpeechRecognition();

      // 🔄 Ustawienia: ciągłe rozpoznawanie
      recognition.lang = selectedLanguage.value;
      recognition.continuous = true; // Mikrofon włączony non-stop
      recognition.interimResults = true; // Pokazuje częściowe wyniki

      // 🎤 Gdy zostanie wykryta mowa
      recognition.onresult = (event) => {
        let transcript = '';
        for (let i = 0; i < event.results.length; i++) {
          transcript += event.results[i][0].transcript + ' ';
        }
        recognizedText.value = transcript.trim();
      };

      // 🔄 Gdy rozpoznawanie się zatrzyma
      recognition.onend = () => {
        if (isRecognizing.value) {
          recognition.start(); // Automatyczny restart
        }
      };
    } else {
      console.error('❌ Web Speech API nie jest obsługiwane w tej przeglądarce.');
    }
  });

  function formatDate(date) {
    return useDateFormat(date, 'DD/MM/YYYY').value;
  };

  // 🎤 Start rozpoznawania mowy
  const startRecognition = () => {
    if (recognition) {
      recognizedText.value = '';
      isRecognizing.value = true;
      recognition.lang = selectedLanguage.value; // Ustawienie języka
      recognition.start();
    }
  };

  // 🛑 Stop rozpoznawania (dopiero po kliknięciu)
  const stopRecognition = () => {
    if (recognition) {
      isRecognizing.value = false;
      recognition.stop();
    }
  };

  // 📤 Wysłanie tekstu na backend
  const sendTextToBackend = async () => {
    serverResponse.value = '';
    isSending.value = true;
    try {
      const response = await axios.post('/api/conversations/generate-response', {
        text: recognizedText.value,
        conversationId: conversationId.value, // Przesyłanie ID konwersacji
      });

      if (!conversationId.value) {
        const newConv = {};
        newConv.conversationId = response.data.conversationId;
        newConv.title = response.data.title;
        newConv.lastMessageDate = new Date();

        conversationStore.saveNewConversation(newConv);
      }

      serverResponse.value = response.data.response;
      conversationId.value = response.data.conversationId; // Zapisywanie ID do kolejnych wiadomości

      console.log('Otrzymana odpowiedź:', response.data);
    }
    catch (error) {
      console.error('Błąd wysyłania tekstu:', error);
    }
    finally {
      isSending.value = false;
    }
  };
  const stopReading = () => {
    speechSynthesis.cancel();
  }
  const speakResponse = () => {
    if (!serverResponse.value || serverResponse.value.trim() === '') {
      console.warn('Brak tekstu do odczytania.');
      return;
    }
  
    speechSynthesis.cancel();   // ❌ Anulujemy poprzednie czytanie, jeśli jakieś trwa

    // ✂️ Dzielimy tekst na fragmenty (np. według kropek lub do 100 znaków)
    const sentences = serverResponse.value.match(/[^.!?]+[.!?]*/g) || [serverResponse.value];

    let index = 0;

    const speakNext = () => {
      if (index < sentences.length) {
        const utterance = new SpeechSynthesisUtterance(sentences[index].trim());
        utterance.lang = selectedLanguage.value;
        utterance.rate = 1;
        utterance.pitch = 1;

        utterance.onend = () => {
          index++;
          speakNext(); // ⏩ Przechodzimy do następnego fragmentu
        };

        speechSynthesis.speak(utterance);
      }
    };
    setTimeout(speakNext, 100);   // ⏳ Krótkie opóźnienie, aby uniknąć problemów w Chrome
  };
</script>

<style scoped>
  button, select {
    margin: 10px;
  }
</style>
