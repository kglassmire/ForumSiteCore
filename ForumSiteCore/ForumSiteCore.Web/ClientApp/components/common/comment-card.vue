<template>    
    <li v-if="comment.parentId == null || isRecursive === true" class="comment-card media border rounded p-2 m-1">        
        <div class="d-inline-flex justify-content-between flex-column mr-3">
            <div class="text-center upvote">
                <i title="upvote" v-on:click="upvote" class="fas fa-arrow-up fa-lg" v-bind:class="upvoted"></i>
            </div>            
            <h4 class="text-center" v-bind:class="voteCountClassObject">
                <strong>{{ comment.upvotes - comment.downvotes }}</strong>
            </h4>
            <div class="text-center downvote">
                <i title="downvote" v-on:click="downvote" class="fas fa-arrow-down fa-lg" v-bind:class="downvoted"></i>
            </div>
        </div>
        <!--<img class="align-self-start mr-3" alt="image">-->
        <div class="media-body mr-4">
            <span v-html="convertedMarkdown"></span>
            <div class="d-flex flex-row mb-1 mt-1">
                Created {{ createdText }} by {{ comment.userName }}
            </div>
            <ul class="list-unstyled">
                <comment-card v-for="comment in comment.children" v-bind:key="comment.id" v-bind:comment="comment" v-bind:isRecursive="true"></comment-card>
            </ul>
        </div>        
    </li>
</template>
<script>
    import postService from '../../services/postservice.js';
    export default {
        props: {
            comment: Object,
            isRecursive: Boolean
        },
        name: 'comment-card',
        data() {
            return {
                counter: 0,
                converter: new showdown.Converter(),
            };
        },
        computed: {
            totalScoreTitle() {
                return `${this.comment.upvotes - this.comment.downvotes} (${this.comment.upvotes}|${this.comment.downvotes})`;
            },
            convertedMarkdown() {
                // `this` points to the vm instance
                
                return this.converter.makeHtml(this.comment.description);
            },
            upvoted() {
                return { 'upvoted': this.comment.userVote === 1 }
            },
            downvoted() {
                return { 'downvoted': this.comment.userVote === -1 }
            },
            voteCountClassObject() {
                return {
                    'upvoted': this.comment.userVote === 1,
                    'downvoted': this.comment.userVote === -1
                }
            },
            createdText() {
                return agofromnow(new Date(this.comment.created));
            }
        },
        methods: {
            savePost() {
                console.log('saved');
            },
            upvote() {
                console.log('upvoted');
            },
            downvote() {
                console.log('downvoted');
            }
        }
    }
</script>