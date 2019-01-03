import axios from 'axios';

export class ForumService {

  search(searchTerms) {
    console.log('searching for: ' + searchTerms);
    return axios.get('/api/forums/search/' + searchTerms);
  }

  getPosts(name, forumListingType, after) {
    return axios.get('/api/forums/' + name + '/' + forumListingType + '?after=' + after);
  }
}

const forumService = new ForumService();
export default forumService;