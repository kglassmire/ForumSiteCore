
<template>
    <div class="post-grid-container">
        <div class="post-header">
            <h1>Header Placeholder</h1>
        </div>
        <div class="post-content">
            <post-card v-bind:key="postCommentListing.post.id" v-bind:post="postCommentListing.post" v-bind:showPostDescription="true"></post-card>
            <ul v-if="postCommentListing.comments !== null" class="list-unstyled">
                <comment-card v-for="comment in postCommentListing.comments" v-bind:key="comment.id" v-bind:comment="comment"></comment-card>
            </ul>
            <h5 v-else>No comments here! Have anything to say?</h5>
        </div>
        <div class="post-menu">            
            
        </div>
    </div>
</template>

<script>

    import CommentCardComponent from '../common/comment-card.vue';
    import PostCardComponent from '../common/post-card.vue';
    import forumService from '../../services/postservice.js';
    export default {
        data() {
            return {
                postCommentListing: window.__INITIAL_STATE__,                
                message: '',
                converter: new showdown.Converter(),
                bottom: false,
                noMorePosts: false
            }
        },
        computed: {
            convertedMarkdown() {
                return this.converter.makeHtml(this.postCommentListing.post.description);
            },
            postSubscribed() {
                return this.commentPostListing.post.userSaved;
            }
        },
        methods: {     
        },
        components: {
            'comment-card': CommentCardComponent,
            'post-card': PostCardComponent
        }
    }
</script>