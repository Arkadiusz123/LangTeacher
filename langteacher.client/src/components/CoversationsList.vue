<template>
  <div class="container mt-4">
    <h3>test conversations</h3>
    <DataTable :columns="columns"
               :data="conversationStore.conversations"
               :apiUrl="'test'"
               :pagination="{ page: 1, pageSize: 10 }"
               :filters="{ }"
               :serverSide="false" />
    </div>
</template>

<script setup>
  import { onMounted } from 'vue';
  import { useConversationStore } from '../services/useConversations';
  import DataTable from "@/components/DataTable.vue";

  const columns = [
    { key: 'title', label: 'Title', sortable: true },
    { key: 'conversationId', label: 'Id', sortable: true },
    { key: 'lastMessageDate', label: 'Last message', sortable: true },
  ]

  const conversationStore = useConversationStore();

  onMounted(() => {
    conversationStore.getConversations();
  });
</script>

