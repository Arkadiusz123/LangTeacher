<script setup>
  import { ref, computed, watch, onMounted } from "vue";
  import DataTable from "primevue/datatable";
  import Column from "primevue/column";
  import Paginator from "primevue/paginator";
  import { useWindowSize } from "@vueuse/core";

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

  const { width } = useWindowSize();
  const isMobile = computed(() => width.value < 768);

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

  const onPageChange = (event) => {
    currentPage.value = event.page + 1; // PrimeVue używa 0-based index
  };
</script>

<template>
  <div>

    <!-- Widok tabeli -->
    <DataTable v-if="!isMobile" :value="paginatedData" :paginator="false" :rows="pageSize" :totalRecords="totalItems">
      <Column v-for="col in props.columns" :key="col.key" :field="col.key" :header="col.label" :sortable="col.sortable" />
    </DataTable>

    <!-- Widok kart na telefonach -->
    <div v-else class="grid grid-cols-1 gap-4">
      <div v-for="row in paginatedData" :key="row.id" class="card p-4 border rounded-lg shadow-md">
        <div v-for="col in props.columns" :key="col.key">
          <strong>{{ col.label }}:</strong> {{ row[col.key] }}
        </div>
      </div>
    </div>

    <!-- Paginacja -->
    <Paginator :rows="pageSize"
               :totalRecords="totalItems"
               :first="(currentPage - 1) * pageSize"
               @page="onPageChange" />
  </div>
</template>

<style>
  .card {
    background: white;
    border: 1px solid #ddd;
  }
</style>
