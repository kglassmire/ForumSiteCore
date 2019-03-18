
<template>
    <div class="forum-grid-container" v-cloak>
        <div class="forum-header">
            <h2>
                f/{{forumPostListing.forum.name}}
            </h2>
            <!-- Hot -->
            <a type="button" v-bind:href="'/f/' + forumPostListing.forum.name + '/hot/'" class="btn btn-secondary">Hot</a>
            <!-- New -->
            <a type="button" v-bind:href="'/f/' + forumPostListing.forum.name + '/new/'" class="btn btn-secondary">New</a>
            <!-- Top -->
            <div class="btn-group">
                <a type="button" v-bind:href="'/f/' + forumPostListing.forum.name + '/top/'" class="btn btn-secondary">Top</a>
                <button type="button" class="btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    <span class="sr-only">Toggle Dropdown</span>
                </button>
                <div class="dropdown-menu">
                    <a class="dropdown-item" href="#">Past Hour</a>
                    <a class="dropdown-item" href="#">Past Day</a>
                    <a class="dropdown-item" href="#">Past Year</a>
                    <a class="dropdown-item" href="#">All Time</a>
                </div>
            </div>
            <!-- Controversial -->
            <div class="btn-group">
                <a type="button" v-bind:href="'/f/' + forumPostListing.forum.name + '/controversial/'" class="btn btn-secondary">Controversial</a>
                <button type="button" class="btn btn-secondary dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    <span class="sr-only">Toggle Dropdown</span>
                </button>
                <div class="dropdown-menu">
                    <a class="dropdown-item" href="#">Past Hour</a>
                    <a class="dropdown-item" href="#">Past Day</a>
                    <a class="dropdown-item" href="#">Past Year</a>
                    <a class="dropdown-item" href="#">All Time</a>
                </div>
            </div>
        </div>
        <div class="forum-content">
            <ul class="list-unstyled">
                <post-card v-for="post in forumPostListing.posts" v-bind:key="post.id" v-bind:post="post"></post-card>
            </ul>
            <div v-if="noMorePosts" class="alert alert-primary" role="alert">
                No more posts!
            </div>
        </div>
        <div class="forum-menu">
            <div><h2>f/{{forumPostListing.forum.name}}</h2></div>
            <button v-if="forumCanBeSubscribed" v-on:click="saveForum" v-bind:class="forumSubscribedButtonClass">{{forumSubscribed ? 'Subscribed' : 'Subscribe'}}</button>
            <div v-html="convertedMarkdown"></div>
        </div>
    </div>
</template>

<script>

    import PostCardComponent from '../common/post-card.vue';
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
            forumSubscribed() {
                return this.forumPostListing.forum.userSaved;
            },
            forumSubscribedButtonClass() {
                return this.forumSubscribed ? 'btn btn-secondary btn-lg btn-block' : 'btn btn-danger btn-lg btn-block';
            },
            convertedMarkdown() {
                return this.converter.makeHtml(this.forumPostListing.forum.description);
            },
            forumCanBeSubscribed() {
                return this.forumPostListing.forum.id !== 0 && this.forumPostListing.forum.id !== -1;
            }
        },
        methods: {
            saveForum() {
                forumService.save(this.forumPostListing.forum)
                    .then(response => {
                        console.log(response.data.message);
                        this.forumPostListing.forum.userSaved = response.data.saved;
                    })
                    .catch(error => console.log(error));
            },
            scroll() {
                window.onscroll = () => {
                    let bottomOfWindow = (window.innerHeight + window.pageYOffset) >= document.body.offsetHeight - 2;

                    if (bottomOfWindow) {
                        this.retrievePosts(this.forumPostListing.forum.name);
                    }
                };
            },
            retrievePosts(name) {                
                forumService.getPosts(name, this.forumPostListing.forumListingType, this.forumPostListing.posts[this.forumPostListing.posts.length - 1].hotScore)
                    .then(response => {                 
                        if (response.data.posts.length === 0) {
                            this.noMorePosts = true;
                        }
                        for (let i = 0; i < response.data.posts.length; i++) {
                            if (!_.find(this.forumPostListing.posts, { id: response.data.posts[i].id })) {
                                this.forumPostListing.posts.push(response.data.posts[i]);
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