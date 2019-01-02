import axios from 'axios';

export class PostService {

  search(searchTerms) {
    return axios.get('/api/forums/search/' + searchTerms)
      .then((response) => {
        return response.data;
      })
      .catch((error) => {
        return error;
      });
  }
}

const postService = new PostService();
export default postService;