<template>
  <div class="container mt-4">
    <h3>Conversations</h3>

    <DataTable class="display responsive nowrap table table-striped"
               :data="conversationStore.conversations"
               :columns="columns"
               :options="options"
               width="100%" />
  </div>
</template>

<script setup>
  import { onMounted } from 'vue';
  import { useConversationStore } from '../services/useConversations';
  import DataTable from "datatables.net-vue3";

  const columns = [
    { data: 'conversationId', title: 'Id' },
    { data: 'title', title: 'Title' },
    { data: 'lastMessageDate', title: 'Last message' },
  ];

  const options = {
    responsive: true,
    select: {
      style: "single"
    },
  };

  const conversationStore = useConversationStore();

  onMounted(() => {
    conversationStore.getConversations();
  });
</script>

<style>
  @import 'datatables.net-responsive-bs5';
  @import 'datatables.net-bs5';
  @import 'datatables.net-select-bs5';
</style>

