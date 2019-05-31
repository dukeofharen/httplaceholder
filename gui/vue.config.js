var path = require('path')

module.exports = {
  lintOnSave: false,
  baseUrl: '',
  configureWebpack: config => {
    config.resolve.alias.urls = process.env.NODE_ENV === 'production' ? path.join(__dirname, 'urls.prod') : path.join(__dirname, 'urls.dev')
  },
  devServer: {
    proxy: {
      '/ph-api': {
        target: 'http://localhost:5000'
      },
      '/requestHub': {
        target: 'http://localhost:5000',
        ws: true
      }
    }
  }
}