import axios from 'axios';

export class PostService {
  save(id) {
    return axios.post('/api/posts/save/' + id);
  }
  vote(id, direction) {

  }
}

const postService = new PostService();
export default postService;