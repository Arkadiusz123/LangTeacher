<template>
  <div class="table-container">
    <table>
      <thead>
        <tr>
          <th v-for="col in props.columns" :key="col.key" @click="sorting = [{ id: col.key, desc: !sorting[0]?.desc }]" style="width: auto;">
            {{ col.label }}
            <span v-if="col.sortable">🔼🔽</span>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="row in table.getRowModel().rows" :key="row.id">
          <td v-for="col in props.columns" :key="col.key">{{ row.getValue(col.key) }}</td>
        </tr>
      </tbody>
    </table>

    <!-- Paginacja -->
    <div class="pagination">
      <button @click="currentPage--" :disabled="currentPage === 1">⬅️</button>
      <span>Page {{ currentPage }} / {{ Math.ceil(totalItems / pageSize) }}</span>
      <button @click="currentPage++" :disabled="currentPage * pageSize >= totalItems">➡️</button>
    </div>
  </div>
</template>

<script setup>
  import { ref, watch, computed, onMounted } from "vue";
  import { useVueTable, getCoreRowModel, getSortedRowModel, getPaginationRowModel } from "@tanstack/vue-table";

  const props = defineProps({
    columns: Array, // [{ key: 'name', label: 'Name', sortable: true }]
    data: Array, // Opcjonalne lokalne dane
    apiUrl: String, // URL do API z %page%, %size% itp.
    pagination: Object, // { page: 1, pageSize: 10 }
    filters: Object, // { search: "", category: "books" }
    serverSide: Boolean, // true -> API, false -> lokalne
  });

  const emit = defineEmits(["update:pagination", "update:filters", "update:sort"]);
  const tableData = ref(props.data || []);
  const sorting = ref([]);
  const currentPage = ref(props.pagination?.page || 1);
  const pageSize = ref(props.pagination?.pageSize || 10);
  const totalItems = ref(0);

  const computedApiUrl = computed(() => {
    if (!props.apiUrl) return "";
    let url = props.apiUrl
      .replace("%page%", currentPage.value)
      .replace("%size%", pageSize.value);

    const filterParams = new URLSearchParams(props.filters).toString();
    if (filterParams) url += `&${filterParams}`;

    const sortParam = sorting.value.map(s => `${s.id}_${s.desc ? 'desc' : 'asc'}`).join(",");
    if (sortParam) url += `&sort=${sortParam}`;

    return url;
  });

  const fetchData = async () => {
    if (!props.serverSide) return;
    const response = await fetch(computedApiUrl.value);
    const json = await response.json();
    tableData.value = json.data;
    totalItems.value = json.total;
  };

  watch([computedApiUrl, currentPage, pageSize, sorting], fetchData, { immediate: true });

  watch(() => props.data, (newData) => {
    tableData.value = newData;
  });

  const paginatedData = computed(() => {
    if (props.serverSide) return tableData.value;
    const sorted = [...tableData.value].sort((a, b) => {
      if (sorting.value.length === 0) return 0;
      const { id, desc } = sorting.value[0];
      return desc ? (b[id] > a[id] ? 1 : -1) : (a[id] > b[id] ? 1 : -1);
    });
    totalItems.value = sorted.length;
    return sorted.slice((currentPage.value - 1) * pageSize.value, currentPage.value * pageSize.value);
  });

  const table = useVueTable({
    columns: props.columns.map(col => ({
      accessorKey: col.key,
      header: col.label,
      sortingFn: col.sortable ? "auto" : undefined,
    })),
    data: paginatedData,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    state: {
      sorting,
    },
  });
</script>

<style>
  .table-container {
    overflow-x: auto;
    max-width: 100%;
  }

    .table-container table {
      width: 100%;
      border-collapse: collapse;
      table-layout: fixed;
    }

    .table-container th, .table-container td {
      border: 1px solid #ddd;
      padding: 8px;
      text-align: left;
      white-space: nowrap;
    }

    .table-container th {
      background-color: #f4f4f4;
      cursor: pointer;
    }

    .table-container td {
      border: 1px solid #ddd;
      padding: 8px;
      text-align: left;
      white-space: nowrap; /* Zatrzymuje tekst w jednej linii */
      overflow: hidden; /* Ukrywa nadmiarowy tekst */
      text-overflow: ellipsis; /* Dodaje '...' na końcu, gdy tekst jest za długi */
    }

  .pagination {
    display: flex;
    justify-content: center;
    margin-top: 10px;
    gap: 10px;
  }
</style>
