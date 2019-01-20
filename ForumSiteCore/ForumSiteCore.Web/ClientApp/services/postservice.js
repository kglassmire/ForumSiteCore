import axios from 'axios';

export class PostService {
  save(id) {
    var postSaveVM = { postId: id };
    return axios.post('/api/posts/save/', postSaveVM);
  }
  vote(id, voteType) {
    var postVoteVM = { postId: id, voteType: voteType };
    return axios.post('/api/posts/vote/', postVoteVM);
  }
  updateCounts(post, oldVoteType, newVoteType) {
    if (oldVoteType === 0) {
      if (newVoteType === 1) {
        console.log(`post ${post.id} went from no vote to upvoted`);
        post.upvotes++;
      }
      if (newVoteType === -1) {
        console.log(`post ${post.id} went from no vote to downvoted`);
        post.downvotes++;
      }
    }
    if (oldVoteType === -1) {
      post.downvotes--;      
      if (newVoteType === 0) {
        console.log(`post ${post.id} went from downvoted to no vote`);
      }
      if (newVoteType === 1) {
        console.log(`post ${post.id} went from downvoted to upvoted`);
        post.upvotes++;
      }
    }
    if (oldVoteType === 1) {
      post.upvotes--;
      if (newVoteType === 0) {
        console.log(`post ${post.id} went from upvoted to no vote`);
      }
      if (newVoteType === -1) {
        console.log(`post ${post.id} went from upvoted to downvoted`);
        post.downvotes++;
      }
    }
  }
}

const postService = new PostService();
export default postService;