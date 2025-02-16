<template>
  <div id="app">
    <MainNavbar />
    <div class="container d-flex justify-content-center align-items-start mt-5">
      <main class="w-100 text-center">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script>
  import MainNavbar from './components/MainNavbar.vue';

  export default {
    name: 'App',
    components: {
      MainNavbar, // Rejestracja komponentu Navbar
    },
  };
</script>

<style>
  body {
    margin: 0;
    font-family: Arial, sans-serif;
  }

  #app {
    height: 100vh; /* Ustawienie pełnej wysokości widoku */
    display: flex;
    flex-direction: column; /* Ustawienie, żeby navbar był na górze */
  }
</style>
