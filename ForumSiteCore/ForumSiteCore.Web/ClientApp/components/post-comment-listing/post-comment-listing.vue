
<template>
    <div class="post-grid-container">
        <div class="post-header">
            <post-card v-bind:key="postCommentListing.post.id" v-bind:post="postCommentListing.post" v-bind:showPostDescription="true"></post-card>
            <!-- Hot -->
            <a type="button" v-bind:href="getUrlString('best')" class="btn btn-secondary">Best</a>
            <!-- New -->
            <a type="button" v-bind:href="getUrlString('new')" class="btn btn-secondary">New</a>
            <!-- Top -->
            <a type="button" v-bind:href="getUrlString('top')" class="btn btn-secondary">Top</a>
            <!-- Controversial -->
            <a type="button" v-bind:href="getUrlString('controversial')" class="btn btn-secondary">Controversial</a>

        </div>
        <div class="post-content">            
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
            getUrlString(type) {
                let url = `/f/${this.postCommentListing.post.forumName}/${this.postCommentListing.post.id}/comments/${type}`;
                return url;
            }
        },
        components: {
            'comment-card': CommentCardComponent,
            'post-card': PostCardComponent
        }
    }
</script>