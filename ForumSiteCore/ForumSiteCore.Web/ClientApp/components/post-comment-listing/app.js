import Vue from 'vue';
import PostCommentListing from './post-comment-listing.vue';
new Vue({
  el: '#post-comment-list',
  render: h => h(PostCommentListing)
});