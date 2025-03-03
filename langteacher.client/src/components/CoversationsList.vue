<template>
  <div class="container mt-4">

    <h3 class="mb-3">Conversations</h3>

    <div class="action-bar">
      <button class="btn btn-success me-2">Add</button>
      <button class="btn btn-warning me-2" :disabled="!selectedRow">Edit</button>
      <button class="btn btn-danger" :disabled="!selectedRow" @click="deleteRow">Remove</button>
    </div>

      <DataTable class="display responsive nowrap table table-striped"
                 :data="conversationStore.conversations"
                 :columns="columns"
                 :options="options"
                 ref="table"
                 width="100%" />
    </div>
</template>

<script setup>
  import { onMounted, ref } from 'vue';
  import { useConversationStore } from '../services/useConversations';
  import DataTable from "datatables.net-vue3";
  import { useDateFormat } from '@vueuse/core';

  const conversationStore = useConversationStore();

  let dt = null;
  const table = ref();
  const selectedRow = ref(null);

  onMounted(function () {
    conversationStore.getConversations();
    dt = table.value.dt;

    dt.on('select', saveSelectedRow);
    dt.on('deselect', function () { selectedRow.value = null; });
  });

  const columns = [
    { data: 'conversationId', title: 'Id' },
    { data: 'title', title: 'Title' },
    {
      data: 'lastMessageDate', title: 'Last message',
      render: function (data) { return useDateFormat(data, 'DD/MM/YYYY HH:mm:ss').value; }
    },
  ];

  const options = {
    responsive: true,
    select: {
      style: "single"
    },
  };

  const saveSelectedRow = () => {
    const selectedRows = dt.rows({ selected: true });
    if (!selectedRows.any()) {
      selectedRow.value = null;
      return;
    }
    selectedRow.value = selectedRows.data()[0].conversationId;
  };

  const deleteRow = () => {
    console.log(selectedRow.value);   //todo
  }

</script>

<style>
  @import 'datatables.net-responsive-bs5';
  @import 'datatables.net-bs5';
  @import 'datatables.net-select-bs5';

  .container {
    position: relative;
  }

  .action-bar {
    position: sticky;
    top: 56px;
    z-index: 1000;
    background-color: white;
    padding: 5px 0;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  }

</style>

