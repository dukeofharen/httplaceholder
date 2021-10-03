module.exports = {
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
