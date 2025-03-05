import axios from 'axios';
import { defineStore } from 'pinia';
import Swal from 'sweetalert2';

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
      }
      catch (error) {
        console.log(error);
      }
    },
    saveNewConversation(conversation) {
      this.conversations.unshift(conversation);
    },
    async deleteConversation(id) {
      try {
        await axios.delete(`/api/conversations/${id}`);
        this.conversations = this.conversations.filter(x => x.conversationId !== id)
      }
      catch (error) {
        console.log(error);
        Swal.fire({
          title: "Error!",
          text: "There was a problem deleting the movie. Please try again.",
          icon: "error"
        });
      }      
    }
  }
});
