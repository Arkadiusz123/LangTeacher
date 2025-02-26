import axios from 'axios';
import { defineStore } from 'pinia';

export const useConversationStore = defineStore('conversationStore', {
  state: () => ({
    conversations: [],
    isDataLoaded: false
  }),
  actions: {
    async getConversations() {
      if (this.isDataLoaded) {
        return;
      }

      try {
        const response = await axios.get('/api/conversations/list');
        this.conversations = response.data;
        this.isDataLoaded = true;
      } catch (error) {
        console.log(error);
      }
    },
    saveNewConversation(conversation) {
      this.conversations.unshift(conversation);
    }
  }
});
