const path = require('path');
const { VueLoaderPlugin } = require('vue-loader');

module.exports = (env = {}, argv = {}) => {

  const isProd = argv.mode === 'production';
  const config = {
    mode: argv.mode || 'development', // we default to development when no 'mode' arg is passed
    entry: {
      main: './components/typeahead/app.js',
      forum: './components/forum-post-listing/app.js'
    },
    output: {
      filename: '[name].js',
      path: path.resolve(__dirname, '../wwwroot/dist'),
      publicPath: "/dist/"
    },
    optimization: {
        splitChunks: {
        chunks: 'all'
      }
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
  }

  return config;
};