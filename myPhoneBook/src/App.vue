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
      <div>
        <img :src="'/images/reverse-telephone-directory.png'" alt="Phonebook" />
      </div>
      <br />
      <div>
        <span class="headerName">{{this.phonebook.name}}</span>
      </div>
      <h3>Phonebook entries: {{this.phonebook.entries.length}}</h3>
      <hr />
      <div id="phonebook-view">
        <md-autocomplete v-model="this.filteredData" :md-options="this.phonebook.entries">
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
        <hr />
        <md-field md-clearable>
          <label>Filter Phonebook</label>
          <md-input v-model="this.filterString" maxlength="20" @keyup="filterList()" />
          <span class="md-helper-text">Filter is case-sensitive</span>
        </md-field>
        <hr />
        <p v-if="this.phonebook.entries.length < 1" class="empty-table">No entries</p>
        <md-list v-else class="md-double-line">
          <md-subheader>Showing {{this.filteredData.length}} of {{this.phonebook.entries.length}}</md-subheader>
          <md-list-item :key="entry.id" v-for="entry in this.filteredData">
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
      <md-button class="md-fab md-fixed md-fab-top-right addentry" @click="showDialog = true">
        <md-icon>add</md-icon>
      </md-button>
      <md-dialog :md-active.sync="showDialog">
        <md-dialog-title>Settings</md-dialog-title>
        <md-tabs md-dynamic-height>
          <md-tab md-label="New">
            <entry-form @add:entry="addEntry" />
          </md-tab>
          <md-tab md-label="Stats">
            <div class="centered">
              <h3>TODO</h3>
            </div>
          </md-tab>
        </md-tabs>
        <md-dialog-actions>
          <md-button class="md-primary" @click="showDialog = false">Close</md-button>
        </md-dialog-actions>
      </md-dialog>
      <md-snackbar
        :md-position="position"
        :md-duration="isInfinity ? Infinity : duration"
        :md-active.sync="showSnackbar"
        md-persistent
        class="snackbar"
      >
        <span class="errormessage">{{errormessage}}</span>
        <div class="snackbar-buttons">
          <md-button class="md-primary" @click="showSnackbar = false">Dismiss</md-button>
        </div>
      </md-snackbar>
    </div>
  </body>
</html>
</template>

<script>
import Vue from "vue";
import VueMaterial from "vue-material";
import "vue-material/dist/vue-material.min.css";
import axios from "axios";

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
        id: "",
        name: "Local",
        entries: []
      },
      searchString: "",
      filterString: "",
      filteredData: [],
      message: "",
      errormessage: "",
      showDialog: false,
      showSnackbar: false,
      position: "center",
      isInfinity: true,
      retryQueue: [],
      errorQueue: []
    };
  },
  mounted() {
    this.loadLocal();
    this.loadRetryQueue();
    this.getPhoneBook();
  },
  computed: {},
  methods: {
    async getPhoneBook() {
      var vm = this;
      axios
        .get("http://localhost:7071/api/phonebook")
        .then(function(response) {
          const data = response;
          vm.phonebook.id = data.id;
          vm.phonebook.name = data.name;
          vm.phonebook.entries = data.entries;
          this.filterList();
        })
        .catch(function(error) {
          vm.displayError("Could not load Phonebook from Server.");
          vm.message = error;
        });
    },
    filterList() {
      var vm = this;
      vm.filteredData = vm.phonebook.entries.filter(vm.checkName);
    },
    async addEntry(entry) {
      var vm = this;
      try {
        vm.phonebook.entries = [...vm.phonebook.entries, entry];
        vm.saveEntries();
        const response = await fetch("http://localhost:7071/api/addentry", {
          method: "POST",
          body: JSON.stringify(entry),
          headers: { "Content-type": "application/json; charset=UTF-8" }
        });
        await response.json();
        //TODO: do something with reponse - check state etc ..
        // set ui state = success / failure / retry
      } catch (error) {
        vm.addEntryToRetryQueue(entry);
        vm.displayError("Could not save Entry.");
      }
    },
    saveEntries() {
      const parsed = JSON.stringify(this.phonebook);
      localStorage.setItem("phonebook", parsed);
    },
    checkName(entry) {
      var s = this.filterString.toLowerCase();
      if (s == "") return true;
      var contains =
        entry.name.toLowerCase().indexOf(s) != -1 ||
        entry.phone.toLowerCase().indexOf(s) != -1;
      return contains;
    },
    displayError(error) {
      this.errormessage = error;
      if (this.showSnackbar == false) this.showSnackbar = true;
    },
    addEntryToRetryQueue(entry) {
      var vm = this;
      vm.retryQueue = [...vm.retryQueue, entry];
      const parsed = JSON.stringify(vm.retryQueue);
      localStorage.setItem("retryqueue", parsed);
    },
    async loadLocal() {
      if (localStorage.getItem("phonebook")) {
        try {
          this.phonebook = JSON.parse(localStorage.getItem("phonebook"));
        } catch (e) {
          localStorage.removeItem("phonebook");
        }
      }
    },
    async loadRetryQueue() {
      if (localStorage.getItem("phonebook")) {
        try {
          this.phonebook = JSON.parse(localStorage.getItem("phonebook"));
        } catch (e) {
          localStorage.removeItem("phonebook");
        }
      }
    },
    addToErrorQueue(error) {
      var vm = this;
      vm.errorQueue = [...vm.errorQueue, error];
      const parsed = JSON.stringify(vm.retryQueue);
      localStorage.setItem("errorqueue", parsed);
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
  padding-top: 30px;
  padding-right: 10px;
  padding-left: 5px;
  padding-bottom: 30px;
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

.centered {
  text-align: center;
  padding: 10px 10px 10px 10px;
}

.headerText {
  font-size: x-large;
  font-weight: bolder;
}

.headerName {
  font-size: x-large;
  font-weight: bolder;
  font-style: italic;
  color: navy;
}

.snackbar {
  text-align: center;
  background-color: whitesmoke;
}
.snackbar-buttons {
  text-align: center;
  padding: 10px 10px 10px 10px;
}
.errormessage {
  text-align: center;
}

.addentry .md-ripple {
  background-color: saddlebrown;
}
</style>
