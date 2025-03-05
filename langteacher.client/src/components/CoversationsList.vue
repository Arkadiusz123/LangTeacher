<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between mb-3">
      <h3>Conversations</h3>
      <button class="btn btn-success ms-auto">Add new</button>
    </div>

    <DataTable class="display responsive nowrap table table-striped"
               :data="conversationStore.conversations"
               :columns="columns"
               :options="options"
               ref="table"
               width="100%">

      <template #column-acts="props">
        <button class="btn btn-danger" @click="deleteRow(props.rowData.conversationId)">Delete</button>
      </template>
    </DataTable>
  </div>
</template>

<script setup>
  import { onMounted, ref } from 'vue';
  import { useConversationStore } from '../services/useConversations';
  import DataTable from "datatables.net-vue3";
  import DataTablesCore from 'datatables.net';
  import { useDateFormat } from '@vueuse/core';
  import Swal from 'sweetalert2';

  const conversationStore = useConversationStore();

  onMounted(function () {
    conversationStore.getConversations();
  });

  const columns = [
    { data: 'conversationId', title: 'Id' },
    { data: 'title', title: 'Title' },
    {
      data: 'lastMessageDate', title: 'Last message',
      render: function (data, type) {
        if (type === "sort" || type === "type") {
          return data;
        }
        return useDateFormat(data, 'DD/MM/YYYY HH:mm:ss').value;
      }
    },
    { name: 'acts', title: 'Actions', data: null, orderable: false },
  ];

  const options = {
    responsive: {
      details: {
        renderer: DataTablesCore.Responsive.renderer.listHiddenNodes()
      }
    }
  };

  const deleteRow = async (id) => {
    const result = await Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!"
    });

    if (!result.isConfirmed) {
      return;
    }
    conversationStore.deleteConversation(id);
  }

</script>

<style>
  @import 'datatables.net-responsive-bs5';
  @import 'datatables.net-bs5';
  @import 'datatables.net-select-bs5';
</style>

