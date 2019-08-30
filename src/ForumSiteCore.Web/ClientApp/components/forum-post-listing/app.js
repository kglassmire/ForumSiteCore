import Vue from 'vue';
import ForumPostListing from './forum-post-listing.vue';
new Vue({
  el: '#forum-post-list',
  render: h => h(ForumPostListing)
});