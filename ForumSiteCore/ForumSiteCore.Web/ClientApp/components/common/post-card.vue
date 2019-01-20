<template>
    <li class="post-card media border rounded p-2 m-1">
        <div class="d-inline-flex justify-content-between flex-column mr-3">
            <div class="text-center upvote">
                <i title="upvote" v-on:click="vote(1)" class="fas fa-arrow-up fa-lg" v-bind:class="upvoted"></i>
            </div>            
            <h4 class="text-center" v-bind:class="voteCountClassObject">
                <strong>{{ post.upvotes - post.downvotes }}</strong>
            </h4>
            <div class="text-center downvote">
                <i title="downvote" v-on:click="vote(-1)" class="fas fa-arrow-down fa-lg" v-bind:class="downvoted"></i>
            </div>
        </div>
        <img class="align-self-start mr-3" alt="image">
        <div class="media-body mr-4">
            <h5>
                <a v-bind:href="post.url">{{ post.name }}</a>
                <button v-if="post.hasDescription" v-on:click="togglePostDescription" class="btn btn-light btn-sm" title="show description">
                    <i class="fas fa-expand"></i>
                </button>
            </h5>
            <p v-if="showPostDescription">
                <span v-html="convertedMarkdown"></span>
            </p>
            <div class="d-flex flex-row mb-1 mt-1">
                Created {{ createdText }} by {{ post.userName }}
            </div>

            <div class="d-flex flex-row mb-1 mt-1">
                <h6 class="mr-3"><i class="far fa-comment-alt"></i> {{ post.commentsCount }} comments</h6>
                <a v-on:click="savePost"><h6><i v-bind:class="post.userSaved === false ? 'far fa-bookmark': 'fas fa-bookmark'"></i> {{post.userSaved ? "saved" : "save"}}</h6></a>
            </div>
        </div>
    

    </li>
</template>
<script>
    import postService from '../../services/postservice.js';
    export default {
        props: ['post'],
        data() {
            return {
                counter: 0,
                converter: new showdown.Converter(),
                showPostDescription: false
            };
        },
        computed: {
            totalScoreTitle() {
                return `${this.post.upvotes - this.post.downvotes} (${this.post.upvotes}|${this.post.downvotes})`;
            },
            showForumName() {
                return true;
            },
            convertedMarkdown() {
                // `this` points to the vm instance
                var self = this;
                return self.converter.makeHtml(self.post.description);
            },
            upvoted() {
                return { 'upvoted': this.post.userVote === 1 }
            },
            downvoted() {
                return { 'downvoted': this.post.userVote === -1 }
            },
            voteCountClassObject() {
                return {
                    'upvoted': this.post.userVote === 1,
                    'downvoted': this.post.userVote === -1
                }
            },
            createdText() {
                return agofromnow(new Date(this.post.created));
            }
        },
        methods: {
            savePost() {
                postService.save(this.post.id)
                    .then(response => {
                        console.log(response.data.message);
                        this.post.userSaved = response.data.saved;
                    })
                    .catch(error => console.log(error));
            },
            vote(voteType) {                

                var oldVoteType = this.post.userVote;
                var newVoteType = voteType;
                if (voteType === this.post.userVote) {

                    newVoteType = 0;
                }                                
                postService.vote(this.post.id, newVoteType)
                    .then(response => {
                        console.log(response.data.message);
                        this.post.userVote = newVoteType;
                        postService.updateCounts(this.post, oldVoteType, newVoteType);
                    })
                    .catch(error => console.log(error));
            },
            togglePostDescription() {
                return this.showPostDescription = !this.showPostDescription;
            }
        }
    }
</script>