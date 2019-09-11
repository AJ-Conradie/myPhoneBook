<template>
<html>
  <head>
    <meta charset="utf-8" />
    <meta content="width=device-width,initial-scale=1,minimal-ui" name="viewport" />
    <link
      rel="stylesheet"
      href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700,400italic|Material+Icons"
    />
    <link rel="stylesheet" href="https://unpkg.com/vue-material/dist/vue-material.min.css" />
    <link rel="stylesheet" href="https://unpkg.com/vue-material/dist/theme/default.css" />
  </head>
  <body>
    <div id="app" class="small-container">
      <h2>Phonebook: {{this.phonebook.name}}</h2>
      <h3>Phonebook entries: {{this.phonebook.entries.length}}</h3>
      <div id="phonebook-view">
        <md-autocomplete v-model="this.phonebook.entries" :md-options="this.phonebook.entries" :value="">
          <label>Search Phonebook entries</label>
          <span class="md-helper-text">Search is non-case sensitive and "fuzzy"</span>
          <template slot="md-autocomplete-item" slot-scope="{ item, term }">
            <md-icon>phone</md-icon>
            <md-highlight-text :md-term="term">{{ item.name }}, {{ item.phone }}</md-highlight-text>
            <md-icon>sms</md-icon>
          </template>
          <template
            slot="md-autocomplete-empty"
            slot-scope="{ term }"
          >No entries matching "{{ term }}" were found.</template>
        </md-autocomplete>
        <md-field>
          <label>Filter Phonebook</label>
          <md-input v-model="this.searchString" v-on:input="filterList()" />
          <span class="md-helper-text">Filter is case-sensitive</span>
        </md-field>
        <hr />
        <p v-if="this.phonebook.entries.length < 1" class="empty-table">No entries</p>
        <md-list v-else class="md-double-line">
          <md-subheader>Phone Book {{this.filteredData.length}} of {{this.phonebook.entries.length}}</md-subheader>
          <md-list-item :key="entry.name" v-for="entry in this.filteredData">
            <md-icon class="md-primary">phone</md-icon>
            <div class="md-list-item-text">
              <span>{{entry.phone}}</span>
              <span>{{entry.name}}</span>
            </div>
            <md-button class="md-icon-button md-list-action">
              <md-icon>sms</md-icon>
            </md-button>
            <md-divider></md-divider>
          </md-list-item>
        </md-list>
      </div>
      <entry-form @add:entry="addEntry" />
      <div>{{this.message}}</div>
    </div>
  </body>
</html>
</template>

<script>

import Vue from "vue";
import VueMaterial from "vue-material";
import "vue-material/dist/vue-material.min.css";
import axios from 'axios';

import EntryForm from "@/components/EntryForm.vue";

Vue.use(VueMaterial);

export default {
  name: "app",
  components: {
    EntryForm
  },
  data() {
    return {
      phonebook: {
        name: "Local",
        entries: []
      },
      searchString: "",
      filteredData: [],
      message: ""
    };
  },
  mounted() {
    if (localStorage.getItem("phonebook")) {
      try {
        this.phonebook = JSON.parse(localStorage.getItem("phonebook"));
      } catch (e) {
        localStorage.removeItem("phonebook");
      }
    }
    this.getPhoneBook();
   },
  computed: {
  },
  methods: {
    async getPhoneBook() {
      var vm = this;
       axios.get('http://localhost:7071/api/phonebook')
        .then(function (response) {
          const data = response;
          vm.phonebook.name = data.name;
          vm.phonebook.entries = data.entries;
          this.filterList();
        })
        .catch(function (error) {
          vm.message = 'Error! Could not reach the API. ' + error
        })
    },
    filterList() {
      this.filteredData = this.phonebook.entries.filter(this.checkName);
    },
    async addEntry(entry) {
      try {
        this.phonebook.entries = [...this.phonebook.entries, entry];
        this.saveEntries();
        const response = await fetch("http://localhost:7071/api/addentry", {
          method: "POST",
          body: JSON.stringify(entry),
          headers: { "Content-type": "application/json; charset=UTF-8" }
        });
        await response.json();
        //TODO: do something with reponse - check state etc ..
        // set ui state = success / failure / retry
      } catch (error) {
        //console.log(error)
      }
    },
    saveEntries() {
      const parsed = JSON.stringify(this.phonebook);
      localStorage.setItem("phonebook", parsed);
    },
    checkName(entry) {
      var s = this.searchString.toLowerCase();
      if (s == "") return true;
      var contains = entry.name.toLowerCase().indexOf(s) != -1 || entry.phone.toLowerCase().indexOf(s) != -1;
      return contains;
    }
  }
};
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
button {
  margin: 0 0.5rem 0 0;
}

input {
  margin: 0;
}

.empty-table {
  text-align: center;
}
</style>
