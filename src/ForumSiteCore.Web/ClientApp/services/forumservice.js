import axios from 'axios';
import timeService from '../services/timeservice';
export class ForumService {

  getUrlString(type, lookback) {

    let url = `/f/${this.forumPostListing.forum.name}/${type}`;
    if (lookback) {
      url += `?dtstart=${timeService.getLookbackDate(lookback).format()}`;
    }

    return url;
  }

  search(searchTerms) {
    console.log('searching for: ' + searchTerms);
    return axios.get('/api/forums/search/' + searchTerms);
  }

  getPosts(forumPostListing) {
    let urlParams = new URLSearchParams(window.location.search);
    let begintime = urlParams.get('begintime');

    let lastPost = forumPostListing.posts[forumPostListing.posts.length - 1];
    let url = '/api/forums/' + forumPostListing.forum.name + '/' + forumPostListing.forumListingType;
    let fullUrl = url;

    // hot is default
    if (forumPostListing.forumListingType === 'hot') {
      fullUrl += '?ceiling=' + lastPost.hotScore; 
      if (begintime)
        fullUrl += '&begintime=' + begintime;
    }

    if (forumPostListing.forumListingType === 'top') {
      fullUrl += '?ceiling=' + lastPost.totalScore;
    }
    
    if (forumPostListing.forumListingType === 'new') {
      let ceiling = lastPost.created;
      fullUrl += '?ceiling=' + ceiling;
    }

    if (forumPostListing.forumListingType === 'controversial') {
      fullUrl += '?ceiling=' + lastPost.controversyScore;
    }
    
    console.log(fullUrl);
    return axios.get(fullUrl);
  }

  save(forum) {
    console.log(`ForumSave is currently ${forum.userSaved} -- attempting to set to ${!forum.userSaved}`);
    var forumSaveVM = { forumId: forum.id, saved: !forum.userSaved };
    return axios.post('/api/forums/save/', forumSaveVM);
  }
}

const forumService = new ForumService();
export default forumService;