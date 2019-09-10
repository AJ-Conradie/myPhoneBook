<template>
  <div id="phonebook-table">
     <md-autocomplete v-model="filteredData" :md-options="entries">
      <label>Auto dropdown list of phonebook entries</label>
      <span class="md-helper-text">Search is non-case sensitive and "fuzzy"</span>
      <template slot="md-autocomplete-item" slot-scope="{ item, term }">
        <md-highlight-text :md-term="term">{{ item.name }}, {{ item.phone }}</md-highlight-text>
      </template>
      <template slot="md-autocomplete-empty" slot-scope="{ term }">
        No entries matching "{{ term }}" were found.
      </template>
    </md-autocomplete>
    <md-field>
      <label>Search Phonebook</label>
      <md-input v-model="searchString"
        v-on:keyup='filter'></md-input>
      <span class="md-helper-text">Search is case-sensitive</span>
    </md-field>
    <hr>
    <p
      v-if="entries.length < 1"
      class="empty-table"
    >
      No entries
    </p>
   <md-list v-else class="md-double-line">
      <md-subheader>Phone Book {{filteredData.length}} of {{entries.length}}</md-subheader>
      <md-list-item
       :key="entry.name"
          v-for="entry in filteredData">
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
</template>

<script>



export default {
  name: 'phonebook-table',
  props: {
    entries: Array,
    filter1: function() {
        this.filteredData = this.entries.filter(this.checkName)
    }
  },
  data: function () {
      return {
        searchString: '',
        filteredData: []
    }
  },
  mount() {
    this.filteredData = this.entries
  },
  methods: {
    filter: function() {
        this.filteredData = this.entries.filter(this.checkName)
    },
    checkName: function(entry) {
      var s = this.searchString
      if(s=='') return true;
      var contains = entry.name.indexOf(s)!=-1 || entry.phone.indexOf(s)!=-1
      return contains;
    }
  }
}


</script>

<style scoped>
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
