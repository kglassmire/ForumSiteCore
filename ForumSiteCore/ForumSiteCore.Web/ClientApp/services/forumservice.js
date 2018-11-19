import axios from 'axios';

export class ForumService {

    search(searchTerms) {
        return axios.get('/api/forums/search/' + searchTerms)
        .then((response) => {
            return response.data;
        })
        .catch( (error) => {
            return error;
        });
    }
}

const forumService = new ForumService();
export default forumService;