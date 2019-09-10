<template>
<html>
  <head>
    <meta charset="utf-8">
    <meta content="width=device-width,initial-scale=1,minimal-ui" name="viewport">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700,400italic|Material+Icons">
    <link rel="stylesheet" href="https://unpkg.com/vue-material/dist/vue-material.min.css">
    <link rel="stylesheet" href="https://unpkg.com/vue-material/dist/theme/default.css">
  </head>
  <body>
  <div id="app" class="small-container">
    <h2>Phonebook: {{phonebook.name}}</h2> 
    <h3>Phonebook entries: {{phonebook.entries.length}}</h3> 
    <entry-form @add:entry="addEntry" />
    <phonebook-table :entries="phonebook.entries" />
  </div>
  </body>
</html>
</template>

<script>

import Vue from 'vue'
import VueMaterial from 'vue-material'
import 'vue-material/dist/vue-material.min.css'

Vue.use(VueMaterial)

import PhonebookTable from '@/components/PhonebookTable.vue'
import EntryForm from '@/components/EntryForm.vue'

export default {
  name: "app",
  components: {
    PhonebookTable,
    EntryForm,
  },
  data() {
    return {
      phonebook: {
        name: 'Local',
        entries: []
      }
    }
  },
  mounted() {
    if (localStorage.getItem('phonebook')) {
      try {
        this.phonebook = JSON.parse(localStorage.getItem('phonebook'));
      } catch(e) {
        localStorage.removeItem('phonebook');
      }
    }
    this.getPhoneBook()
  },

  methods: {
    async getPhoneBook() {
      try {
        const response = await fetch('http://localhost:7071/api/phonebook')
        const data = await response.json()
        this.phonebook.name = data.name
        this.phonebook.entries = data.entries
        this.saveEntries()
      } catch (error) {
        //console.log(error)
      }
    },

    async addEntry(entry) {
      try {
        this.phonebook.entries = [...this.phonebook.entries, entry]
        this.saveEntries()
        const response = await fetch('http://localhost:7071/api/addentry', {
          method: 'POST',
          body: JSON.stringify(entry),
          headers: { "Content-type": "application/json; charset=UTF-8" }
        })
        await response.json()
        //TODO: do something with reponse - check state etc ..
        // set ui state = success / failure / retry
      } catch (error) {
        //console.log(error)
      }
    },
    saveEntries() {
      const parsed = JSON.stringify(this.phonebook);
      localStorage.setItem('phonebook', parsed);
    }
  
  },
}
</script>

<style>

body {
  padding-left: 10px;
}
button {
  background: whitesmoke;
  border: 1px solid #009435;
}

button:hover,
button:active,
button:focus {
  background: #32a95d;
  border: 1px solid #32a95d;
}

.small-container {
  margin: 10px 10px 10px 10px;
  max-width: 680px;
}
</style>
