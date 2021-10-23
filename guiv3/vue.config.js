module.exports = {
  publicPath: process.env.NODE_ENV === "production" ? "" : "/",
  chainWebpack: (config) => {
    config.plugin("html").tap((args) => {
      args[0].title = "HttPlaceholder";
      return args;
    });
  },
  devServer: {
    proxy: {
      "/ph-api": {
        target: "http://localhost:5000",
      },
      "/requestHub": {
        target: "http://localhost:5000",
        ws: true,
      },
    },
  },
};
