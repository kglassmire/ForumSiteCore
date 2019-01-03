
<template>
    <div class="forum-grid-container" v-cloak>
        <div class="forum-header">
            <h2>
                f/{{forumPostListing.forum.name}}
            </h2>
        </div>
        <div class="forum-content">
            <ul class="list-unstyled">
                <post-card v-for="post in forumPostListing.posts" v-bind:key="post.id" v-bind:post="post"></post-card>
            </ul>
            <div v-if="noMorePosts" class="alert alert-primary" role="alert">
                No more posts!
            </div>
        </div>
        <div class="forum-menu" v-html="convertedMarkdown"></div>
    </div>
</template>

<script>

    import PostCardComponent from './post-card.vue';
    import forumService from '../../services/forumservice.js';
    export default {
        data() {
            return {
                forumPostListing: window.__INITIAL_STATE__,                
                message: '',
                converter: new showdown.Converter(),
                bottom: false,
                noMorePosts: false

            }
        },
        computed: {
            convertedMarkdown() {

                return this.converter.makeHtml(this.forumPostListing.forum.description);
            }
        },
        methods: {
            scroll() {
                window.onscroll = () => {
                    let bottomOfWindow = (window.innerHeight + window.pageYOffset) >= document.body.offsetHeight - 2;
                    console.log('window.innerHeight + window.pageYOffset: ' + (window.innerHeight + window.pageYOffset));
                    console.log('document.body.offsetHeight: ' + document.body.offsetHeight);
                    if (bottomOfWindow) {
                        this.retrievePosts(this.forumPostListing.forum.name);
                    }
                };
            },
            retrievePosts(name) {                
                forumService.getPosts(name, this.forumPostListing.forumListingType, this.forumPostListing.posts[this.forumPostListing.posts.length - 1].hotScore)
                    .then(response => {                 
                        if (response.data.data.posts.length === 0) {
                            this.noMorePosts = true;
                        }
                        for (let i = 0; i < response.data.data.posts.length; i++) {
                            if (!_.find(this.forumPostListing.posts, { id: response.data.data.posts[i].id })) {
                                this.forumPostListing.posts.push(response.data.data.posts[i]);
                            } else {
                                console.log('duplicate found -- skipping');
                            }
                        }
                    })
                    .catch((error) => {
                        console.log(error)
                    });                
            }
        },
        mounted() {
            this.scroll();
        },
        components: {
            'post-card': PostCardComponent
        }
    }
</script>

            <!--retrievePosts: function (name) {
                let self = this;
                fetch('api/forums/' + name + '/hot')
                    .then(responseStream => responseStream.json())
                    .then(function (response) {
                        console.log(self.forumPostListing.posts.length);
                        for (i = 0; i < response.data.posts.length; i++) {
                            self.forumPostListing.posts.push(response.data.posts[i]);
                        }
                    })
                    .catch(error => console.log(error))-->