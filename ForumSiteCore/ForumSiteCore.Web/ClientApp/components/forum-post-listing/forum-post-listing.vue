
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
        </div>
        <div class="forum-menu" v-html="convertedMarkdown"></div>
    </div>
</template>

<script>

    import PostCardComponent from './post-card.vue';
    export default { 
        data: function () {
            return {
                forumPostListing: window.__INITIAL_STATE__,
                message: '',
                converter: new showdown.Converter()
            }
        },
        computed: {
            convertedMarkdown: function () {
                // `this` points to the vm instance
                var self = this;
                return self.converter.makeHtml(self.forumPostListing.forum.description);
            }
        },
        methods: {
            retrievePosts: function (name) {
                let self = this;
                fetch('api/forums/' + name + '/hot')
                    .then(responseStream => responseStream.json())
                    .then(function (response) {
                        console.log(self.forumPostListing.posts.length);
                        for (i = 0; i < response.data.posts.length; i++) {
                            self.forumPostListing.posts.push(response.data.posts[i]);
                        }
                    })
                    .catch(error => console.log(error))
            }
        },
        components: {
            'post-card': PostCardComponent
        }
    }
</script>