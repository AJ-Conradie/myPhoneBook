<template>
  <div id="entry-form">
    <form @submit.prevent="handleSubmit">
      <md-field>
        <label>Name</label>
        <md-input
          id="firstinput"
          type="text"
          :class="{ 'has-error': submitting && invalidName }"
          v-model="entry.name"
          @focus="clearStatus"
          @keypress="clearStatus" 
          maxlength=20></md-input>
        <span class="md-helper-text">Please enter the name</span>
      </md-field>
      <md-field>
        <label>Phone number</label>
        <md-input
          type="text"
          :class="{ 'has-error': submitting && invalidPhone }"
          v-model="entry.phone"
          @focus="clearStatus" 
          maxlength=20></md-input>
        <span class="md-helper-text">Please enter the phone number</span>
      </md-field>
      <p
        v-if="error && submitting"
        class="error-message"
      >❗Please fill out all required fields</p>
      <p
        v-if="success"
        class="success-message"
      >✅ Entry successfully added</p>
      <md-button type="submit" class="md-dense md-raised md-primary">Click to Add entry</md-button>
    </form>
  </div>
</template>

<script>
export default {
  name: 'entry-form',
  data() {
    return {
      error: false,
      submitting: false,
      success: false,
      entry: {
        name: '',
        phone: '',
      }
    }
  },
  computed: {
    invalidName() {
      return this.entry.name === '' || this.entry.name.length > 20
    },

    invalidPhone() {
      return this.entry.phone === ''|| this.entry.phone.length > 20
    },
  },
  methods: {
    handleSubmit() {
      this.clearStatus()
      this.submitting = true

      if (this.invalidName || this.invalidPhone) {
        this.error = true
        return
      }

      this.$emit('add:entry', this.entry)
      //this.$refs.firstinput.focus()
      this.entry = {
        name: '',
        phone: '',
      }
      this.success = true
      this.error = false
      this.submitting = false
    },

    clearStatus() {
      this.success = false
      this.error = false
    }
  }}
</script>

<style scoped>
form {
  margin-bottom: 2rem;
}

[class*="-message"] {
  font-weight: 500;
}

.error-message {
  color: #d33c40;
}

.success-message {
  color: #32a95d;
}
</style>
