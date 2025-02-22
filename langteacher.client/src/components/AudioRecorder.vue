<template>
  <div>
    <h2>Speech Recognition</h2>

    <label for="language">Select language:</label>
    <select v-model="selectedLanguage">
      <option value="pl-PL">🇵🇱 Polski</option>
      <option value="en-US">🇬🇧 English</option>
    </select>

    <button @click="startRecognition" :disabled="isRecognizing">🎤 Start</button>
    <button @click="stopRecognition" :disabled="!isRecognizing">🛑 Stop</button>
    <button @click="displayResponse = !displayResponse" :disabled="!serverResponse">{{ displayResponse ? 'Hide response' : 'Display response' }}</button>

    <p v-if="recognizedText"><strong>Recognized text:</strong> {{ recognizedText }}</p>

    <button v-if="recognizedText" @click="sendTextToBackend" :disabled="isSending">📤 Send text</button>
    <button v-if="serverResponse" @click="speakResponse">🔊 Read answear</button>

    <p v-if="serverResponse" v-show="displayResponse"><strong>Answear:</strong> {{ serverResponse }}</p>
  </div>
</template>

<script setup>
  import { ref } from 'vue';
  import axios from 'axios';

  // Zmienne reaktywne
  const recognizedText = ref('');
  const isRecognizing = ref(false);
  const isSending = ref(false);
  const displayResponse = ref(false);
  const conversationId = ref(null);
  const serverResponse = ref('');
  const selectedLanguage = ref('en-US'); // Domyślnie polski
  let recognition = null;

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
      const response = await axios.post('/api/conversations', {
        text: recognizedText.value,
        conversationId: conversationId.value, // Przesyłanie ID konwersacji
      });

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

  const speakResponse = () => {
    if (!serverResponse.value || serverResponse.value.trim() === '') {
      console.warn('Brak tekstu do odczytania.');
      return;
    }

    // ❌ Anulujemy poprzednie czytanie, jeśli jakieś trwa
    speechSynthesis.cancel();

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

<!--serverResponse.value = response.data.response;
conversationId.value = response.data.conversationId;-->
