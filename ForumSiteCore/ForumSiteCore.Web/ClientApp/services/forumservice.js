import axios from 'axios';

export class ForumService {

  search(searchTerms) {
    console.log('searching for: ' + searchTerms);
    return axios.get('/api/forums/search/' + searchTerms);
  }

  getPosts(name, forumListingType, after) {
    return axios.get('/api/forums/' + name + '/' + forumListingType + '?after=' + after);
  }

  save(forum) {
    console.log(`ForumSave is currently ${forum.userSaved} -- attempting to set to ${!forum.userSaved}`);
    var forumSaveVM = { forumId: forum.id, saved: !forum.userSaved };
    return axios.post('/api/forums/save/', forumSaveVM);
  }
}

const forumService = new ForumService();
export default forumService;