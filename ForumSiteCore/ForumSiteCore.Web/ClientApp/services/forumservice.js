import axios from 'axios';

export class ForumService {

  search(searchTerms) {
    console.log('searching for: ' + searchTerms);
    return axios.get('/api/forums/search/' + searchTerms);
  }

  getPosts(name, forumListingType) {
    return axios.get('/api/forums/' + name + '/' + forumListingType);
  }
}

const forumService = new ForumService();
export default forumService;