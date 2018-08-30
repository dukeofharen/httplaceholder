var path = require('path')

module.exports = {
  lintOnSave: false,
  configureWebpack: config => {
    config.resolve.alias.urls = process.env.NODE_ENV === 'production' ? path.join(__dirname, 'urls.prod') : path.join(__dirname, 'urls.dev')
  }
}