<template>
  <div class="blog-item-detail">
    <button class="close-button" @click="close">X</button>
    <img :src="image" alt="Blog Image" class="blog-image" />
    <h2>{{ title }}</h2>
    <p>{{ text }}</p>
  </div>
</template>

<script setup lang="ts">
const props = defineProps({
  image: String,
  text: String,
  title: String
})

const emit = defineEmits(['close'])

const close = () => {
  emit('close')
}
</script>

<style scoped>
.blog-item-detail {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.9); /* Darker background */
  color: white;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-start; /* Align content at the top */
  padding: 2rem;
  box-sizing: border-box;
  overflow: auto; /* Enable scrolling */
}

.blog-image {
  max-width: 60%;
  height: auto;
  border-radius: 8px;
  margin-bottom: 1rem;
}

.close-button {
  position: fixed; /* Change to fixed */
  top: 1rem;
  right: 2rem; /* Move a little more to the left */
  background: white; /* White circle background */
  border: none;
  color: black; /* Change text color to black for contrast */
  font-size: 1.5rem;
  cursor: pointer;
  border-radius: 50%; /* Make it a circle */
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

p {
  max-width: 80%;
  text-align: center;
  color: white;
  z-index: 1;
}
@media (min-width: 1024px) {
  h1 {
    font-size: 2rem;
  }
  h2 {
    font-size: 1.5rem;
  }
  .blog-item-detail {
    padding: 4rem;
  }

  .blog-image {
    max-width: 40%;
  }

  p {
    max-width: 60%;
  }
}
</style>
