const path = require('path');
const { VueLoaderPlugin } = require('vue-loader');

module.exports = (env = {}, argv = {}) => {

  const isProd = argv.mode === 'production';
  const config = {
    mode: argv.mode || 'development', // we default to development when no 'mode' arg is passed
    devtool: 'eval-source-map',
    entry: {
      main: './components/typeahead/app.js',
      forum: './components/forum-post-listing/app.js',
      post: './components/post-comment-listing/app.js',
      site: './site/site.js'
    },
    output: {
      filename: '[name].js',
      path: path.resolve(__dirname, '../wwwroot/dist'),
      publicPath: "/dist/"
    },
    module: {
      rules: [
        {
          test: /\.css$/,
          use: [
            'style-loader',
            'css-loader'
          ]
        },
        {
          test: /\.vue$/,
          use: 'vue-loader'
        }
      ]
    },
    plugins: [
      new VueLoaderPlugin()
    ]
  };

  return config;
};