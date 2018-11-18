<template>
    <div class="form-row align-items-center">
        <div class="col-auto">
            <label class="sr-only" for="inlineFormInput">Search</label>
            <div class="input-group mr-sm-2">
                <div class="input-group-prepend">
                    <div class="input-group-text">
                        <i class="fas fa-search"></i>
                    </div>
                </div>                
                <input type="text" class="form-control mr-sm-2 dropdown" placeholder="Search" v-model="searchTerms" v-on:keydown="onKeydown" v-on:blur="onBlur" v-on:input="search" aria-label="Search">
                <div v-if="hasResults" class="dropdown-menu bd-search-results" role="menu">
                    <a v-bind:key="item" v-bind:href="'/f/' + item + '/hot'" on-keyup:enter="goToLink" :class="['dropdown-item', index===menuCursor ? 'active' : '']" v-for="(item, index) in searchResults">{{ item }}</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>

    export default {
        data: function() {
            return {
                searchTerms: '',
                searchResults: [],
                menuCursor: null
            };
        },
        computed: {
            hasResults: function () {
                return this.searchResults.length > 0;
            }
        },
        methods: {
            search: _.debounce(function () {
                self = this;
                if (self.searchTerms.length > 0) {
                    fetch('/api/forums/search/' + self.searchTerms)
                        .then(responseStream => responseStream.json())
                        .then(function (response) {
                            self.searchResults = [];
                            self.menuCursor = null;
                            for (var i = 0; i < response.data.length; i++) {
                                self.searchResults.push(response.data[i]);
                            }
                        })
                        .catch(error => console.log(error));
                }
                else {
                    self.searchResults = [];
                }
            }, 500),
                onKeydown: function (event) {
                    let keysToProcess = ['ArrowDown', 'ArrowUp', 'ArrowLeft', 'ArrowRight', 'Escape', 'Enter'];
                    if (keysToProcess.includes(event.key)) {
                        if (this.hasResults) {
                            if (event.key === 'Escape') {
                                this.searchTerms = '';
                                this.searchResults = [];
                                return;
                            }
                            if (event.key === 'Enter') {
                                window.location.href = `/f/${this.searchResults[this.menuCursor]}/hot`;
                                return;
                            }
                            if (this.menuCursor === null) {
                                this.menuCursor = 0;
                            } else {
                                if (event.key === 'ArrowDown' || event.key === 'ArrowLeft') {
                                    if (this.menuCursor + 1 <= (this.searchResults.length - 1)) {
                                        this.menuCursor++;
                                    }
                                }
                                if (event.key === 'ArrowUp' || event.key === 'ArrowRight') {
                                    if (this.menuCursor - 1 >= 0) {
                                        this.menuCursor--;
                                    }
                                }
                            }
                        }
                    }
                },
            onBlur: function () {
                if (this.searchTerms.length == 0) {
                    this.searchResults = [];
                }
            }
        }
    }

</script>